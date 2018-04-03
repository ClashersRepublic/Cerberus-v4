// LzmaEncoder.cs

namespace LineageSoft.Magic.Services.Core.Libs.Lzma.Compression.LZMA
{
    using System;
    using System.IO;
    using LineageSoft.Magic.Services.Core.Libs.Lzma.Compression.LZ;
    using LineageSoft.Magic.Services.Core.Libs.Lzma.Compression.RangeCoder;

    public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
    {
        private enum EMatchFinderType
        {
            BT2,
            BT4
        }

        private const uint kIfinityPrice = 0xFFFFFFF;

        private static readonly byte[] g_FastPos = new byte[1 << 11];

        static Encoder()
        {
            const byte kFastSlots = 22;
            int c = 2;
            Encoder.g_FastPos[0] = 0;
            Encoder.g_FastPos[1] = 1;
            for (byte slotFast = 2; slotFast < kFastSlots; slotFast++)
            {
                uint k = (uint) 1 << ((slotFast >> 1) - 1);
                for (uint j = 0; j < k; j++, c++)
                {
                    Encoder.g_FastPos[c] = slotFast;
                }
            }
        }

        private static uint GetPosSlot(uint pos)
        {
            if (pos < 1 << 11)
            {
                return Encoder.g_FastPos[pos];
            }

            if (pos < 1 << 21)
            {
                return (uint) (Encoder.g_FastPos[pos >> 10] + 20);
            }

            return (uint) (Encoder.g_FastPos[pos >> 20] + 40);
        }

        private static uint GetPosSlot2(uint pos)
        {
            if (pos < 1 << 17)
            {
                return (uint) (Encoder.g_FastPos[pos >> 6] + 12);
            }

            if (pos < 1 << 27)
            {
                return (uint) (Encoder.g_FastPos[pos >> 16] + 32);
            }

            return (uint) (Encoder.g_FastPos[pos >> 26] + 52);
        }

        private Base.State _state = new Base.State();
        private byte _previousByte;
        private readonly uint[] _repDistances = new uint[Base.kNumRepDistances];

        private void BaseInit()
        {
            this._state.Init();
            this._previousByte = 0;
            for (uint i = 0; i < Base.kNumRepDistances; i++)
            {
                this._repDistances[i] = 0;
            }
        }

        private const int kDefaultDictionaryLogSize = 22;
        private const uint kNumFastBytesDefault = 0x20;

        private class LiteralEncoder
        {
            public struct Encoder2
            {
                private BitEncoder[] m_Encoders;

                public void Create()
                {
                    this.m_Encoders = new BitEncoder[0x300];
                }

                public void Init()
                {
                    for (int i = 0; i < 0x300; i++)
                    {
                        this.m_Encoders[i].Init();
                    }
                }

                public void Encode(RangeCoder.Encoder rangeEncoder, byte symbol)
                {
                    uint context = 1;
                    for (int i = 7; i >= 0; i--)
                    {
                        uint bit = (uint) ((symbol >> i) & 1);
                        this.m_Encoders[context].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public void EncodeMatched(RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
                {
                    uint context = 1;
                    bool same = true;
                    for (int i = 7; i >= 0; i--)
                    {
                        uint bit = (uint) ((symbol >> i) & 1);
                        uint state = context;
                        if (same)
                        {
                            uint matchBit = (uint) ((matchByte >> i) & 1);
                            state += (1 + matchBit) << 8;
                            same = matchBit == bit;
                        }

                        this.m_Encoders[state].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
                {
                    uint price = 0;
                    uint context = 1;
                    int i = 7;
                    if (matchMode)
                    {
                        for (; i >= 0; i--)
                        {
                            uint matchBit = (uint) (matchByte >> i) & 1;
                            uint bit = (uint) (symbol >> i) & 1;
                            price += this.m_Encoders[((1 + matchBit) << 8) + context].GetPrice(bit);
                            context = (context << 1) | bit;
                            if (matchBit != bit)
                            {
                                i--;
                                break;
                            }
                        }
                    }

                    for (; i >= 0; i--)
                    {
                        uint bit = (uint) (symbol >> i) & 1;
                        price += this.m_Encoders[context].GetPrice(bit);
                        context = (context << 1) | bit;
                    }

                    return price;
                }
            }

            private Encoder2[] m_Coders;
            private int m_NumPrevBits;
            private int m_NumPosBits;
            private uint m_PosMask;

            public void Create(int numPosBits, int numPrevBits)
            {
                if (this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits)
                {
                    return;
                }

                this.m_NumPosBits = numPosBits;
                this.m_PosMask = ((uint) 1 << numPosBits) - 1;
                this.m_NumPrevBits = numPrevBits;
                uint numStates = (uint) 1 << (this.m_NumPrevBits + this.m_NumPosBits);
                this.m_Coders = new Encoder2[numStates];
                for (uint i = 0; i < numStates; i++)
                {
                    this.m_Coders[i].Create();
                }
            }

            public void Init()
            {
                uint numStates = (uint) 1 << (this.m_NumPrevBits + this.m_NumPosBits);
                for (uint i = 0; i < numStates; i++)
                {
                    this.m_Coders[i].Init();
                }
            }

            public Encoder2 GetSubCoder(uint pos, byte prevByte)
            {
                return this.m_Coders[((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint) (prevByte >> (8 - this.m_NumPrevBits))];
            }
        }

        private class LenEncoder
        {
            private BitEncoder _choice = new BitEncoder();
            private BitEncoder _choice2 = new BitEncoder();
            private readonly BitTreeEncoder[] _lowCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];
            private readonly BitTreeEncoder[] _midCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];
            private BitTreeEncoder _highCoder = new BitTreeEncoder(Base.kNumHighLenBits);

            public LenEncoder()
            {
                for (uint posState = 0; posState < Base.kNumPosStatesEncodingMax; posState++)
                {
                    this._lowCoder[posState] = new BitTreeEncoder(Base.kNumLowLenBits);
                    this._midCoder[posState] = new BitTreeEncoder(Base.kNumMidLenBits);
                }
            }

            public void Init(uint numPosStates)
            {
                this._choice.Init();
                this._choice2.Init();
                for (uint posState = 0; posState < numPosStates; posState++)
                {
                    this._lowCoder[posState].Init();
                    this._midCoder[posState].Init();
                }

                this._highCoder.Init();
            }

            public void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
            {
                if (symbol < Base.kNumLowLenSymbols)
                {
                    this._choice.Encode(rangeEncoder, 0);
                    this._lowCoder[posState].Encode(rangeEncoder, symbol);
                }
                else
                {
                    symbol -= Base.kNumLowLenSymbols;
                    this._choice.Encode(rangeEncoder, 1);
                    if (symbol < Base.kNumMidLenSymbols)
                    {
                        this._choice2.Encode(rangeEncoder, 0);
                        this._midCoder[posState].Encode(rangeEncoder, symbol);
                    }
                    else
                    {
                        this._choice2.Encode(rangeEncoder, 1);
                        this._highCoder.Encode(rangeEncoder, symbol - Base.kNumMidLenSymbols);
                    }
                }
            }

            public void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
            {
                uint a0 = this._choice.GetPrice0();
                uint a1 = this._choice.GetPrice1();
                uint b0 = a1 + this._choice2.GetPrice0();
                uint b1 = a1 + this._choice2.GetPrice1();
                uint i = 0;
                for (i = 0; i < Base.kNumLowLenSymbols; i++)
                {
                    if (i >= numSymbols)
                    {
                        return;
                    }

                    prices[st + i] = a0 + this._lowCoder[posState].GetPrice(i);
                }

                for (; i < Base.kNumLowLenSymbols + Base.kNumMidLenSymbols; i++)
                {
                    if (i >= numSymbols)
                    {
                        return;
                    }

                    prices[st + i] = b0 + this._midCoder[posState].GetPrice(i - Base.kNumLowLenSymbols);
                }

                for (; i < numSymbols; i++)
                {
                    prices[st + i] = b1 + this._highCoder.GetPrice(i - Base.kNumLowLenSymbols - Base.kNumMidLenSymbols);
                }
            }
        }

        private const uint kNumLenSpecSymbols = Base.kNumLowLenSymbols + Base.kNumMidLenSymbols;

        private class LenPriceTableEncoder : LenEncoder
        {
            private readonly uint[] _prices = new uint[Base.kNumLenSymbols << Base.kNumPosStatesBitsEncodingMax];
            private uint _tableSize;
            private readonly uint[] _counters = new uint[Base.kNumPosStatesEncodingMax];

            public void SetTableSize(uint tableSize)
            {
                this._tableSize = tableSize;
            }

            public uint GetPrice(uint symbol, uint posState)
            {
                return this._prices[posState * Base.kNumLenSymbols + symbol];
            }

            private void UpdateTable(uint posState)
            {
                this.SetPrices(posState, this._tableSize, this._prices, posState * Base.kNumLenSymbols);
                this._counters[posState] = this._tableSize;
            }

            public void UpdateTables(uint numPosStates)
            {
                for (uint posState = 0; posState < numPosStates; posState++)
                {
                    this.UpdateTable(posState);
                }
            }

            public new void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
            {
                base.Encode(rangeEncoder, symbol, posState);
                if (--this._counters[posState] == 0)
                {
                    this.UpdateTable(posState);
                }
            }
        }

        private const uint kNumOpts = 1 << 12;

        private class Optimal
        {
            public Base.State State;

            public bool Prev1IsChar;
            public bool Prev2;

            public uint PosPrev2;
            public uint BackPrev2;

            public uint Price;
            public uint PosPrev;
            public uint BackPrev;

            public uint Backs0;
            public uint Backs1;
            public uint Backs2;
            public uint Backs3;

            public void MakeAsChar()
            {
                this.BackPrev = 0xFFFFFFFF;
                this.Prev1IsChar = false;
            }

            public void MakeAsShortRep()
            {
                this.BackPrev = 0;
                ;
                this.Prev1IsChar = false;
            }

            public bool IsShortRep()
            {
                return this.BackPrev == 0;
            }
        }

        private readonly Optimal[] _optimum = new Optimal[Encoder.kNumOpts];
        private IMatchFinder _matchFinder;
        private readonly RangeCoder.Encoder _rangeEncoder = new RangeCoder.Encoder();

        private readonly BitEncoder[] _isMatch = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];
        private readonly BitEncoder[] _isRep = new BitEncoder[Base.kNumStates];
        private readonly BitEncoder[] _isRepG0 = new BitEncoder[Base.kNumStates];
        private readonly BitEncoder[] _isRepG1 = new BitEncoder[Base.kNumStates];
        private readonly BitEncoder[] _isRepG2 = new BitEncoder[Base.kNumStates];
        private readonly BitEncoder[] _isRep0Long = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

        private readonly BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[Base.kNumLenToPosStates];

        private readonly BitEncoder[] _posEncoders = new BitEncoder[Base.kNumFullDistances - Base.kEndPosModelIndex];
        private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(Base.kNumAlignBits);

        private readonly LenPriceTableEncoder _lenEncoder = new LenPriceTableEncoder();
        private readonly LenPriceTableEncoder _repMatchLenEncoder = new LenPriceTableEncoder();

        private readonly LiteralEncoder _literalEncoder = new LiteralEncoder();

        private readonly uint[] _matchDistances = new uint[Base.kMatchMaxLen * 2 + 2];

        private uint _numFastBytes = Encoder.kNumFastBytesDefault;
        private uint _longestMatchLength;
        private uint _numDistancePairs;

        private uint _additionalOffset;

        private uint _optimumEndIndex;
        private uint _optimumCurrentIndex;

        private bool _longestMatchWasFound;

        private readonly uint[] _posSlotPrices = new uint[1 << (Base.kNumPosSlotBits + Base.kNumLenToPosStatesBits)];
        private readonly uint[] _distancesPrices = new uint[Base.kNumFullDistances << Base.kNumLenToPosStatesBits];
        private readonly uint[] _alignPrices = new uint[Base.kAlignTableSize];
        private uint _alignPriceCount;

        private uint _distTableSize = Encoder.kDefaultDictionaryLogSize * 2;

        private int _posStateBits = 2;
        private uint _posStateMask = 4 - 1;
        private int _numLiteralPosStateBits;
        private int _numLiteralContextBits = 3;

        private uint _dictionarySize = 1 << Encoder.kDefaultDictionaryLogSize;
        private uint _dictionarySizePrev = 0xFFFFFFFF;
        private uint _numFastBytesPrev = 0xFFFFFFFF;

        private long nowPos64;
        private bool _finished;
        private Stream _inStream;

        private EMatchFinderType _matchFinderType = EMatchFinderType.BT4;
        private bool _writeEndMark;

        private bool _needReleaseMFStream;

        private void Create()
        {
            if (this._matchFinder == null)
            {
                BinTree bt = new BinTree();
                int numHashBytes = 4;
                if (this._matchFinderType == EMatchFinderType.BT2)
                {
                    numHashBytes = 2;
                }

                bt.SetType(numHashBytes);
                this._matchFinder = bt;
            }

            this._literalEncoder.Create(this._numLiteralPosStateBits, this._numLiteralContextBits);

            if (this._dictionarySize == this._dictionarySizePrev && this._numFastBytesPrev == this._numFastBytes)
            {
                return;
            }

            this._matchFinder.Create(this._dictionarySize, Encoder.kNumOpts, this._numFastBytes, Base.kMatchMaxLen + 1);
            this._dictionarySizePrev = this._dictionarySize;
            this._numFastBytesPrev = this._numFastBytes;
        }

        public Encoder()
        {
            for (int i = 0; i < Encoder.kNumOpts; i++)
            {
                this._optimum[i] = new Optimal();
            }

            for (int i = 0; i < Base.kNumLenToPosStates; i++)
            {
                this._posSlotEncoder[i] = new BitTreeEncoder(Base.kNumPosSlotBits);
            }
        }

        private void SetWriteEndMarkerMode(bool writeEndMarker)
        {
            this._writeEndMark = writeEndMarker;
        }

        private void Init()
        {
            this.BaseInit();
            this._rangeEncoder.Init();

            uint i;
            for (i = 0; i < Base.kNumStates; i++)
            {
                for (uint j = 0; j <= this._posStateMask; j++)
                {
                    uint complexState = (i << Base.kNumPosStatesBitsMax) + j;
                    this._isMatch[complexState].Init();
                    this._isRep0Long[complexState].Init();
                }

                this._isRep[i].Init();
                this._isRepG0[i].Init();
                this._isRepG1[i].Init();
                this._isRepG2[i].Init();
            }

            this._literalEncoder.Init();
            for (i = 0; i < Base.kNumLenToPosStates; i++)
            {
                this._posSlotEncoder[i].Init();
            }

            for (i = 0; i < Base.kNumFullDistances - Base.kEndPosModelIndex; i++)
            {
                this._posEncoders[i].Init();
            }

            this._lenEncoder.Init((uint) 1 << this._posStateBits);
            this._repMatchLenEncoder.Init((uint) 1 << this._posStateBits);

            this._posAlignEncoder.Init();

            this._longestMatchWasFound = false;
            this._optimumEndIndex = 0;
            this._optimumCurrentIndex = 0;
            this._additionalOffset = 0;
        }

        private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
        {
            lenRes = 0;
            numDistancePairs = this._matchFinder.GetMatches(this._matchDistances);
            if (numDistancePairs > 0)
            {
                lenRes = this._matchDistances[numDistancePairs - 2];
                if (lenRes == this._numFastBytes)
                {
                    lenRes += this._matchFinder.GetMatchLen((int) lenRes - 1, this._matchDistances[numDistancePairs - 1],
                        Base.kMatchMaxLen - lenRes);
                }
            }

            this._additionalOffset++;
        }


        private void MovePos(uint num)
        {
            if (num > 0)
            {
                this._matchFinder.Skip(num);
                this._additionalOffset += num;
            }
        }

        private uint GetRepLen1Price(Base.State state, uint posState)
        {
            return this._isRepG0[state.Index].GetPrice0() + this._isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0();
        }

        private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
        {
            uint price;
            if (repIndex == 0)
            {
                price = this._isRepG0[state.Index].GetPrice0();
                price += this._isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            }
            else
            {
                price = this._isRepG0[state.Index].GetPrice1();
                if (repIndex == 1)
                {
                    price += this._isRepG1[state.Index].GetPrice0();
                }
                else
                {
                    price += this._isRepG1[state.Index].GetPrice1();
                    price += this._isRepG2[state.Index].GetPrice(repIndex - 2);
                }
            }

            return price;
        }

        private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
        {
            uint price = this._repMatchLenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
            return price + this.GetPureRepPrice(repIndex, state, posState);
        }

        private uint GetPosLenPrice(uint pos, uint len, uint posState)
        {
            uint price;
            uint lenToPosState = Base.GetLenToPosState(len);
            if (pos < Base.kNumFullDistances)
            {
                price = this._distancesPrices[lenToPosState * Base.kNumFullDistances + pos];
            }
            else
            {
                price = this._posSlotPrices[(lenToPosState << Base.kNumPosSlotBits) + Encoder.GetPosSlot2(pos)] + this._alignPrices[pos & Base.kAlignMask];
            }

            return price + this._lenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
        }

        private uint Backward(out uint backRes, uint cur)
        {
            this._optimumEndIndex = cur;
            uint posMem = this._optimum[cur].PosPrev;
            uint backMem = this._optimum[cur].BackPrev;
            do
            {
                if (this._optimum[cur].Prev1IsChar)
                {
                    this._optimum[posMem].MakeAsChar();
                    this._optimum[posMem].PosPrev = posMem - 1;
                    if (this._optimum[cur].Prev2)
                    {
                        this._optimum[posMem - 1].Prev1IsChar = false;
                        this._optimum[posMem - 1].PosPrev = this._optimum[cur].PosPrev2;
                        this._optimum[posMem - 1].BackPrev = this._optimum[cur].BackPrev2;
                    }
                }

                uint posPrev = posMem;
                uint backCur = backMem;

                backMem = this._optimum[posPrev].BackPrev;
                posMem = this._optimum[posPrev].PosPrev;

                this._optimum[posPrev].BackPrev = backCur;
                this._optimum[posPrev].PosPrev = cur;
                cur = posPrev;
            } while (cur > 0);

            backRes = this._optimum[0].BackPrev;
            this._optimumCurrentIndex = this._optimum[0].PosPrev;
            return this._optimumCurrentIndex;
        }

        private readonly uint[] reps = new uint[Base.kNumRepDistances];
        private readonly uint[] repLens = new uint[Base.kNumRepDistances];


        private uint GetOptimum(uint position, out uint backRes)
        {
            if (this._optimumEndIndex != this._optimumCurrentIndex)
            {
                uint lenRes = this._optimum[this._optimumCurrentIndex].PosPrev - this._optimumCurrentIndex;
                backRes = this._optimum[this._optimumCurrentIndex].BackPrev;
                this._optimumCurrentIndex = this._optimum[this._optimumCurrentIndex].PosPrev;
                return lenRes;
            }

            this._optimumCurrentIndex = this._optimumEndIndex = 0;

            uint lenMain, numDistancePairs;
            if (!this._longestMatchWasFound)
            {
                this.ReadMatchDistances(out lenMain, out numDistancePairs);
            }
            else
            {
                lenMain = this._longestMatchLength;
                numDistancePairs = this._numDistancePairs;
                this._longestMatchWasFound = false;
            }

            uint numAvailableBytes = this._matchFinder.GetNumAvailableBytes() + 1;
            if (numAvailableBytes < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }

            if (numAvailableBytes > Base.kMatchMaxLen)
            {
                numAvailableBytes = Base.kMatchMaxLen;
            }

            uint repMaxIndex = 0;
            uint i;
            for (i = 0; i < Base.kNumRepDistances; i++)
            {
                this.reps[i] = this._repDistances[i];
                this.repLens[i] = this._matchFinder.GetMatchLen(0 - 1, this.reps[i], Base.kMatchMaxLen);
                if (this.repLens[i] > this.repLens[repMaxIndex])
                {
                    repMaxIndex = i;
                }
            }

            if (this.repLens[repMaxIndex] >= this._numFastBytes)
            {
                backRes = repMaxIndex;
                uint lenRes = this.repLens[repMaxIndex];
                this.MovePos(lenRes - 1);
                return lenRes;
            }

            if (lenMain >= this._numFastBytes)
            {
                backRes = this._matchDistances[numDistancePairs - 1] + Base.kNumRepDistances;
                this.MovePos(lenMain - 1);
                return lenMain;
            }

            byte currentByte = this._matchFinder.GetIndexByte(0 - 1);
            byte matchByte = this._matchFinder.GetIndexByte((int) (0 - this._repDistances[0] - 1 - 1));

            if (lenMain < 2 && currentByte != matchByte && this.repLens[repMaxIndex] < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }

            this._optimum[0].State = this._state;

            uint posState = position & this._posStateMask;

            this._optimum[1].Price = this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
                                     this._literalEncoder.GetSubCoder(position, this._previousByte).GetPrice(!this._state.IsCharState(), matchByte, currentByte);
            this._optimum[1].MakeAsChar();

            uint matchPrice = this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            uint repMatchPrice = matchPrice + this._isRep[this._state.Index].GetPrice1();

            if (matchByte == currentByte)
            {
                uint shortRepPrice = repMatchPrice + this.GetRepLen1Price(this._state, posState);
                if (shortRepPrice < this._optimum[1].Price)
                {
                    this._optimum[1].Price = shortRepPrice;
                    this._optimum[1].MakeAsShortRep();
                }
            }

            uint lenEnd = lenMain >= this.repLens[repMaxIndex] ? lenMain : this.repLens[repMaxIndex];

            if (lenEnd < 2)
            {
                backRes = this._optimum[1].BackPrev;
                return 1;
            }

            this._optimum[1].PosPrev = 0;

            this._optimum[0].Backs0 = this.reps[0];
            this._optimum[0].Backs1 = this.reps[1];
            this._optimum[0].Backs2 = this.reps[2];
            this._optimum[0].Backs3 = this.reps[3];

            uint len = lenEnd;
            do
            {
                this._optimum[len--].Price = Encoder.kIfinityPrice;
            } while (len >= 2);

            for (i = 0; i < Base.kNumRepDistances; i++)
            {
                uint repLen = this.repLens[i];
                if (repLen < 2)
                {
                    continue;
                }

                uint price = repMatchPrice + this.GetPureRepPrice(i, this._state, posState);
                do
                {
                    uint curAndLenPrice = price + this._repMatchLenEncoder.GetPrice(repLen - 2, posState);
                    Optimal optimum = this._optimum[repLen];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = i;
                        optimum.Prev1IsChar = false;
                    }
                } while (--repLen >= 2);
            }

            uint normalMatchPrice = matchPrice + this._isRep[this._state.Index].GetPrice0();

            len = this.repLens[0] >= 2 ? this.repLens[0] + 1 : 2;
            if (len <= lenMain)
            {
                uint offs = 0;
                while (len > this._matchDistances[offs])
                {
                    offs += 2;
                }

                for (;; len++)
                {
                    uint distance = this._matchDistances[offs + 1];
                    uint curAndLenPrice = normalMatchPrice + this.GetPosLenPrice(distance, len, posState);
                    Optimal optimum = this._optimum[len];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = distance + Base.kNumRepDistances;
                        optimum.Prev1IsChar = false;
                    }

                    if (len == this._matchDistances[offs])
                    {
                        offs += 2;
                        if (offs == numDistancePairs)
                        {
                            break;
                        }
                    }
                }
            }

            uint cur = 0;

            while (true)
            {
                cur++;
                if (cur == lenEnd)
                {
                    return this.Backward(out backRes, cur);
                }

                uint newLen;
                this.ReadMatchDistances(out newLen, out numDistancePairs);
                if (newLen >= this._numFastBytes)
                {
                    this._numDistancePairs = numDistancePairs;
                    this._longestMatchLength = newLen;
                    this._longestMatchWasFound = true;
                    return this.Backward(out backRes, cur);
                }

                position++;
                uint posPrev = this._optimum[cur].PosPrev;
                Base.State state;
                if (this._optimum[cur].Prev1IsChar)
                {
                    posPrev--;
                    if (this._optimum[cur].Prev2)
                    {
                        state = this._optimum[this._optimum[cur].PosPrev2].State;
                        if (this._optimum[cur].BackPrev2 < Base.kNumRepDistances)
                        {
                            state.UpdateRep();
                        }
                        else
                        {
                            state.UpdateMatch();
                        }
                    }
                    else
                    {
                        state = this._optimum[posPrev].State;
                    }

                    state.UpdateChar();
                }
                else
                {
                    state = this._optimum[posPrev].State;
                }

                if (posPrev == cur - 1)
                {
                    if (this._optimum[cur].IsShortRep())
                    {
                        state.UpdateShortRep();
                    }
                    else
                    {
                        state.UpdateChar();
                    }
                }
                else
                {
                    uint pos;
                    if (this._optimum[cur].Prev1IsChar && this._optimum[cur].Prev2)
                    {
                        posPrev = this._optimum[cur].PosPrev2;
                        pos = this._optimum[cur].BackPrev2;
                        state.UpdateRep();
                    }
                    else
                    {
                        pos = this._optimum[cur].BackPrev;
                        if (pos < Base.kNumRepDistances)
                        {
                            state.UpdateRep();
                        }
                        else
                        {
                            state.UpdateMatch();
                        }
                    }

                    Optimal opt = this._optimum[posPrev];
                    if (pos < Base.kNumRepDistances)
                    {
                        if (pos == 0)
                        {
                            this.reps[0] = opt.Backs0;
                            this.reps[1] = opt.Backs1;
                            this.reps[2] = opt.Backs2;
                            this.reps[3] = opt.Backs3;
                        }
                        else if (pos == 1)
                        {
                            this.reps[0] = opt.Backs1;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs2;
                            this.reps[3] = opt.Backs3;
                        }
                        else if (pos == 2)
                        {
                            this.reps[0] = opt.Backs2;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs1;
                            this.reps[3] = opt.Backs3;
                        }
                        else
                        {
                            this.reps[0] = opt.Backs3;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs1;
                            this.reps[3] = opt.Backs2;
                        }
                    }
                    else
                    {
                        this.reps[0] = pos - Base.kNumRepDistances;
                        this.reps[1] = opt.Backs0;
                        this.reps[2] = opt.Backs1;
                        this.reps[3] = opt.Backs2;
                    }
                }

                this._optimum[cur].State = state;
                this._optimum[cur].Backs0 = this.reps[0];
                this._optimum[cur].Backs1 = this.reps[1];
                this._optimum[cur].Backs2 = this.reps[2];
                this._optimum[cur].Backs3 = this.reps[3];
                uint curPrice = this._optimum[cur].Price;

                currentByte = this._matchFinder.GetIndexByte(0 - 1);
                matchByte = this._matchFinder.GetIndexByte((int) (0 - this.reps[0] - 1 - 1));

                posState = position & this._posStateMask;

                uint curAnd1Price = curPrice + this._isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
                                    this._literalEncoder.GetSubCoder(position, this._matchFinder.GetIndexByte(0 - 2)).GetPrice(!state.IsCharState(), matchByte, currentByte);

                Optimal nextOptimum = this._optimum[cur + 1];

                bool nextIsChar = false;
                if (curAnd1Price < nextOptimum.Price)
                {
                    nextOptimum.Price = curAnd1Price;
                    nextOptimum.PosPrev = cur;
                    nextOptimum.MakeAsChar();
                    nextIsChar = true;
                }

                matchPrice = curPrice + this._isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
                repMatchPrice = matchPrice + this._isRep[state.Index].GetPrice1();

                if (matchByte == currentByte &&
                    !(nextOptimum.PosPrev < cur && nextOptimum.BackPrev == 0))
                {
                    uint shortRepPrice = repMatchPrice + this.GetRepLen1Price(state, posState);
                    if (shortRepPrice <= nextOptimum.Price)
                    {
                        nextOptimum.Price = shortRepPrice;
                        nextOptimum.PosPrev = cur;
                        nextOptimum.MakeAsShortRep();
                        nextIsChar = true;
                    }
                }

                uint numAvailableBytesFull = this._matchFinder.GetNumAvailableBytes() + 1;
                numAvailableBytesFull = Math.Min(Encoder.kNumOpts - 1 - cur, numAvailableBytesFull);
                numAvailableBytes = numAvailableBytesFull;

                if (numAvailableBytes < 2)
                {
                    continue;
                }

                if (numAvailableBytes > this._numFastBytes)
                {
                    numAvailableBytes = this._numFastBytes;
                }

                if (!nextIsChar && matchByte != currentByte)
                {
                    // try Literal + rep0
                    uint t = Math.Min(numAvailableBytesFull - 1, this._numFastBytes);
                    uint lenTest2 = this._matchFinder.GetMatchLen(0, this.reps[0], t);
                    if (lenTest2 >= 2)
                    {
                        Base.State state2 = state;
                        state2.UpdateChar();
                        uint posStateNext = (position + 1) & this._posStateMask;
                        uint nextRepMatchPrice = curAnd1Price + this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1() +
                                                 this._isRep[state2.Index].GetPrice1();
                        {
                            uint offset = cur + 1 + lenTest2;
                            while (lenEnd < offset)
                            {
                                this._optimum[++lenEnd].Price = Encoder.kIfinityPrice;
                            }

                            uint curAndLenPrice = nextRepMatchPrice + this.GetRepPrice(
                                                      0, lenTest2, state2, posStateNext);
                            Optimal optimum = this._optimum[offset];
                            if (curAndLenPrice < optimum.Price)
                            {
                                optimum.Price = curAndLenPrice;
                                optimum.PosPrev = cur + 1;
                                optimum.BackPrev = 0;
                                optimum.Prev1IsChar = true;
                                optimum.Prev2 = false;
                            }
                        }
                    }
                }

                uint startLen = 2; // speed optimization 

                for (uint repIndex = 0; repIndex < Base.kNumRepDistances; repIndex++)
                {
                    uint lenTest = this._matchFinder.GetMatchLen(0 - 1, this.reps[repIndex], numAvailableBytes);
                    if (lenTest < 2)
                    {
                        continue;
                    }

                    uint lenTestTemp = lenTest;
                    do
                    {
                        while (lenEnd < cur + lenTest)
                        {
                            this._optimum[++lenEnd].Price = Encoder.kIfinityPrice;
                        }

                        uint curAndLenPrice = repMatchPrice + this.GetRepPrice(repIndex, lenTest, state, posState);
                        Optimal optimum = this._optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = repIndex;
                            optimum.Prev1IsChar = false;
                        }
                    } while (--lenTest >= 2);

                    lenTest = lenTestTemp;

                    if (repIndex == 0)
                    {
                        startLen = lenTest + 1;
                    }

                    // if (_maxMode)
                    if (lenTest < numAvailableBytesFull)
                    {
                        uint t = Math.Min(numAvailableBytesFull - 1 - lenTest, this._numFastBytes);
                        uint lenTest2 = this._matchFinder.GetMatchLen((int) lenTest, this.reps[repIndex], t);
                        if (lenTest2 >= 2)
                        {
                            Base.State state2 = state;
                            state2.UpdateRep();
                            uint posStateNext = (position + lenTest) & this._posStateMask;
                            uint curAndLenCharPrice =
                                repMatchPrice + this.GetRepPrice(repIndex, lenTest, state, posState) +
                                this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice0() + this
                                                                                                                        ._literalEncoder
                                                                                                                        .GetSubCoder(position + lenTest,
                                                                                                                            this._matchFinder.GetIndexByte((int) lenTest - 1 - 1))
                                                                                                                        .GetPrice(true,
                                                                                                                            this._matchFinder.GetIndexByte(
                                                                                                                                (int) lenTest - 1 -
                                                                                                                                (int) (this.reps[repIndex] + 1)),
                                                                                                                            this._matchFinder.GetIndexByte((int) lenTest - 1));
                            state2.UpdateChar();
                            posStateNext = (position + lenTest + 1) & this._posStateMask;
                            uint nextMatchPrice = curAndLenCharPrice + this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1();
                            uint nextRepMatchPrice = nextMatchPrice + this._isRep[state2.Index].GetPrice1();

                            // for(; lenTest2 >= 2; lenTest2--)
                            {
                                uint offset = lenTest + 1 + lenTest2;
                                while (lenEnd < cur + offset)
                                {
                                    this._optimum[++lenEnd].Price = Encoder.kIfinityPrice;
                                }

                                uint curAndLenPrice = nextRepMatchPrice + this.GetRepPrice(0, lenTest2, state2, posStateNext);
                                Optimal optimum = this._optimum[cur + offset];
                                if (curAndLenPrice < optimum.Price)
                                {
                                    optimum.Price = curAndLenPrice;
                                    optimum.PosPrev = cur + lenTest + 1;
                                    optimum.BackPrev = 0;
                                    optimum.Prev1IsChar = true;
                                    optimum.Prev2 = true;
                                    optimum.PosPrev2 = cur;
                                    optimum.BackPrev2 = repIndex;
                                }
                            }
                        }
                    }
                }

                if (newLen > numAvailableBytes)
                {
                    newLen = numAvailableBytes;
                    for (numDistancePairs = 0; newLen > this._matchDistances[numDistancePairs]; numDistancePairs += 2)
                    {
                        ;
                    }

                    this._matchDistances[numDistancePairs] = newLen;
                    numDistancePairs += 2;
                }

                if (newLen >= startLen)
                {
                    normalMatchPrice = matchPrice + this._isRep[state.Index].GetPrice0();
                    while (lenEnd < cur + newLen)
                    {
                        this._optimum[++lenEnd].Price = Encoder.kIfinityPrice;
                    }

                    uint offs = 0;
                    while (startLen > this._matchDistances[offs])
                    {
                        offs += 2;
                    }

                    for (uint lenTest = startLen;; lenTest++)
                    {
                        uint curBack = this._matchDistances[offs + 1];
                        uint curAndLenPrice = normalMatchPrice + this.GetPosLenPrice(curBack, lenTest, posState);
                        Optimal optimum = this._optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = curBack + Base.kNumRepDistances;
                            optimum.Prev1IsChar = false;
                        }

                        if (lenTest == this._matchDistances[offs])
                        {
                            if (lenTest < numAvailableBytesFull)
                            {
                                uint t = Math.Min(numAvailableBytesFull - 1 - lenTest, this._numFastBytes);
                                uint lenTest2 = this._matchFinder.GetMatchLen((int) lenTest, curBack, t);
                                if (lenTest2 >= 2)
                                {
                                    Base.State state2 = state;
                                    state2.UpdateMatch();
                                    uint posStateNext = (position + lenTest) & this._posStateMask;
                                    uint curAndLenCharPrice = curAndLenPrice + this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice0() + this
                                                                                                                                                                       ._literalEncoder
                                                                                                                                                                       .GetSubCoder(
                                                                                                                                                                           position +
                                                                                                                                                                           lenTest,
                                                                                                                                                                           this
                                                                                                                                                                               ._matchFinder
                                                                                                                                                                               .GetIndexByte(
                                                                                                                                                                                   (int
                                                                                                                                                                                   ) lenTest -
                                                                                                                                                                                   1 -
                                                                                                                                                                                   1))
                                                                                                                                                                       .GetPrice(
                                                                                                                                                                           true,
                                                                                                                                                                           this
                                                                                                                                                                               ._matchFinder
                                                                                                                                                                               .GetIndexByte(
                                                                                                                                                                                   (int
                                                                                                                                                                                   ) lenTest -
                                                                                                                                                                                   (int
                                                                                                                                                                                   ) (
                                                                                                                                                                                       curBack +
                                                                                                                                                                                       1) -
                                                                                                                                                                                   1),
                                                                                                                                                                           this
                                                                                                                                                                               ._matchFinder
                                                                                                                                                                               .GetIndexByte(
                                                                                                                                                                                   (int
                                                                                                                                                                                   ) lenTest -
                                                                                                                                                                                   1));
                                    state2.UpdateChar();
                                    posStateNext = (position + lenTest + 1) & this._posStateMask;
                                    uint nextMatchPrice = curAndLenCharPrice + this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1();
                                    uint nextRepMatchPrice = nextMatchPrice + this._isRep[state2.Index].GetPrice1();

                                    uint offset = lenTest + 1 + lenTest2;
                                    while (lenEnd < cur + offset)
                                    {
                                        this._optimum[++lenEnd].Price = Encoder.kIfinityPrice;
                                    }

                                    curAndLenPrice = nextRepMatchPrice + this.GetRepPrice(0, lenTest2, state2, posStateNext);
                                    optimum = this._optimum[cur + offset];
                                    if (curAndLenPrice < optimum.Price)
                                    {
                                        optimum.Price = curAndLenPrice;
                                        optimum.PosPrev = cur + lenTest + 1;
                                        optimum.BackPrev = 0;
                                        optimum.Prev1IsChar = true;
                                        optimum.Prev2 = true;
                                        optimum.PosPrev2 = cur;
                                        optimum.BackPrev2 = curBack + Base.kNumRepDistances;
                                    }
                                }
                            }

                            offs += 2;
                            if (offs == numDistancePairs)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool ChangePair(uint smallDist, uint bigDist)
        {
            const int kDif = 7;
            return smallDist < (uint) 1 << (32 - kDif) && bigDist >= smallDist << kDif;
        }

        private void WriteEndMarker(uint posState)
        {
            if (!this._writeEndMark)
            {
                return;
            }

            this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(this._rangeEncoder, 1);
            this._isRep[this._state.Index].Encode(this._rangeEncoder, 0);
            this._state.UpdateMatch();
            uint len = Base.kMatchMinLen;
            this._lenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
            uint posSlot = (1 << Base.kNumPosSlotBits) - 1;
            uint lenToPosState = Base.GetLenToPosState(len);
            this._posSlotEncoder[lenToPosState].Encode(this._rangeEncoder, posSlot);
            int footerBits = 30;
            uint posReduced = ((uint) 1 << footerBits) - 1;
            this._rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
            this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & Base.kAlignMask);
        }

        private void Flush(uint nowPos)
        {
            this.ReleaseMFStream();
            this.WriteEndMarker(nowPos & this._posStateMask);
            this._rangeEncoder.FlushData();
            this._rangeEncoder.FlushStream();
        }

        public void CodeOneBlock(out long inSize, out long outSize, out bool finished)
        {
            inSize = 0;
            outSize = 0;
            finished = true;

            if (this._inStream != null)
            {
                this._matchFinder.SetStream(this._inStream);
                this._matchFinder.Init();
                this._needReleaseMFStream = true;
                this._inStream = null;
                if (this._trainSize > 0)
                {
                    this._matchFinder.Skip(this._trainSize);
                }
            }

            if (this._finished)
            {
                return;
            }

            this._finished = true;


            long progressPosValuePrev = this.nowPos64;
            if (this.nowPos64 == 0)
            {
                if (this._matchFinder.GetNumAvailableBytes() == 0)
                {
                    this.Flush((uint) this.nowPos64);
                    return;
                }

                uint len, numDistancePairs; // it's not used
                this.ReadMatchDistances(out len, out numDistancePairs);
                uint posState = (uint) this.nowPos64 & this._posStateMask;
                this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(this._rangeEncoder, 0);
                this._state.UpdateChar();
                byte curByte = this._matchFinder.GetIndexByte((int) (0 - this._additionalOffset));
                this._literalEncoder.GetSubCoder((uint) this.nowPos64, this._previousByte).Encode(this._rangeEncoder, curByte);
                this._previousByte = curByte;
                this._additionalOffset--;
                this.nowPos64++;
            }

            if (this._matchFinder.GetNumAvailableBytes() == 0)
            {
                this.Flush((uint) this.nowPos64);
                return;
            }

            while (true)
            {
                uint pos;
                uint len = this.GetOptimum((uint) this.nowPos64, out pos);

                uint posState = (uint) this.nowPos64 & this._posStateMask;
                uint complexState = (this._state.Index << Base.kNumPosStatesBitsMax) + posState;
                if (len == 1 && pos == 0xFFFFFFFF)
                {
                    this._isMatch[complexState].Encode(this._rangeEncoder, 0);
                    byte curByte = this._matchFinder.GetIndexByte((int) (0 - this._additionalOffset));
                    LiteralEncoder.Encoder2 subCoder = this._literalEncoder.GetSubCoder((uint) this.nowPos64, this._previousByte);
                    if (!this._state.IsCharState())
                    {
                        byte matchByte = this._matchFinder.GetIndexByte((int) (0 - this._repDistances[0] - 1 - this._additionalOffset));
                        subCoder.EncodeMatched(this._rangeEncoder, matchByte, curByte);
                    }
                    else
                    {
                        subCoder.Encode(this._rangeEncoder, curByte);
                    }

                    this._previousByte = curByte;
                    this._state.UpdateChar();
                }
                else
                {
                    this._isMatch[complexState].Encode(this._rangeEncoder, 1);
                    if (pos < Base.kNumRepDistances)
                    {
                        this._isRep[this._state.Index].Encode(this._rangeEncoder, 1);
                        if (pos == 0)
                        {
                            this._isRepG0[this._state.Index].Encode(this._rangeEncoder, 0);
                            if (len == 1)
                            {
                                this._isRep0Long[complexState].Encode(this._rangeEncoder, 0);
                            }
                            else
                            {
                                this._isRep0Long[complexState].Encode(this._rangeEncoder, 1);
                            }
                        }
                        else
                        {
                            this._isRepG0[this._state.Index].Encode(this._rangeEncoder, 1);
                            if (pos == 1)
                            {
                                this._isRepG1[this._state.Index].Encode(this._rangeEncoder, 0);
                            }
                            else
                            {
                                this._isRepG1[this._state.Index].Encode(this._rangeEncoder, 1);
                                this._isRepG2[this._state.Index].Encode(this._rangeEncoder, pos - 2);
                            }
                        }

                        if (len == 1)
                        {
                            this._state.UpdateShortRep();
                        }
                        else
                        {
                            this._repMatchLenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
                            this._state.UpdateRep();
                        }

                        uint distance = this._repDistances[pos];
                        if (pos != 0)
                        {
                            for (uint i = pos; i >= 1; i--)
                            {
                                this._repDistances[i] = this._repDistances[i - 1];
                            }

                            this._repDistances[0] = distance;
                        }
                    }
                    else
                    {
                        this._isRep[this._state.Index].Encode(this._rangeEncoder, 0);
                        this._state.UpdateMatch();
                        this._lenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
                        pos -= Base.kNumRepDistances;
                        uint posSlot = Encoder.GetPosSlot(pos);
                        uint lenToPosState = Base.GetLenToPosState(len);
                        this._posSlotEncoder[lenToPosState].Encode(this._rangeEncoder, posSlot);

                        if (posSlot >= Base.kStartPosModelIndex)
                        {
                            int footerBits = (int) ((posSlot >> 1) - 1);
                            uint baseVal = (2 | (posSlot & 1)) << footerBits;
                            uint posReduced = pos - baseVal;

                            if (posSlot < Base.kEndPosModelIndex)
                            {
                                BitTreeEncoder.ReverseEncode(this._posEncoders,
                                    baseVal - posSlot - 1, this._rangeEncoder, footerBits, posReduced);
                            }
                            else
                            {
                                this._rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
                                this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & Base.kAlignMask);
                                this._alignPriceCount++;
                            }
                        }

                        uint distance = pos;
                        for (uint i = Base.kNumRepDistances - 1; i >= 1; i--)
                        {
                            this._repDistances[i] = this._repDistances[i - 1];
                        }

                        this._repDistances[0] = distance;
                        this._matchPriceCount++;
                    }

                    this._previousByte = this._matchFinder.GetIndexByte((int) (len - 1 - this._additionalOffset));
                }

                this._additionalOffset -= len;
                this.nowPos64 += len;
                if (this._additionalOffset == 0)
                {
                    // if (!_fastMode)
                    if (this._matchPriceCount >= 1 << 7)
                    {
                        this.FillDistancesPrices();
                    }

                    if (this._alignPriceCount >= Base.kAlignTableSize)
                    {
                        this.FillAlignPrices();
                    }

                    inSize = this.nowPos64;
                    outSize = this._rangeEncoder.GetProcessedSizeAdd();
                    if (this._matchFinder.GetNumAvailableBytes() == 0)
                    {
                        this.Flush((uint) this.nowPos64);
                        return;
                    }

                    if (this.nowPos64 - progressPosValuePrev >= 1 << 12)
                    {
                        this._finished = false;
                        finished = false;
                        return;
                    }
                }
            }
        }

        private void ReleaseMFStream()
        {
            if (this._matchFinder != null && this._needReleaseMFStream)
            {
                this._matchFinder.ReleaseStream();
                this._needReleaseMFStream = false;
            }
        }

        private void SetOutStream(Stream outStream)
        {
            this._rangeEncoder.SetStream(outStream);
        }

        private void ReleaseOutStream()
        {
            this._rangeEncoder.ReleaseStream();
        }

        private void ReleaseStreams()
        {
            this.ReleaseMFStream();
            this.ReleaseOutStream();
        }

        private void SetStreams(Stream inStream, Stream outStream,
            long inSize, long outSize)
        {
            this._inStream = inStream;
            this._finished = false;
            this.Create();
            this.SetOutStream(outStream);
            this.Init();

            // if (!_fastMode)
            {
                this.FillDistancesPrices();
                this.FillAlignPrices();
            }

            this._lenEncoder.SetTableSize(this._numFastBytes + 1 - Base.kMatchMinLen);
            this._lenEncoder.UpdateTables((uint) 1 << this._posStateBits);
            this._repMatchLenEncoder.SetTableSize(this._numFastBytes + 1 - Base.kMatchMinLen);
            this._repMatchLenEncoder.UpdateTables((uint) 1 << this._posStateBits);

            this.nowPos64 = 0;
        }


        public void Code(Stream inStream, Stream outStream,
            long inSize, long outSize, ICodeProgress progress)
        {
            this._needReleaseMFStream = false;
            try
            {
                this.SetStreams(inStream, outStream, inSize, outSize);
                while (true)
                {
                    long processedInSize;
                    long processedOutSize;
                    bool finished;
                    this.CodeOneBlock(out processedInSize, out processedOutSize, out finished);
                    if (finished)
                    {
                        return;
                    }

                    if (progress != null)
                    {
                        progress.SetProgress(processedInSize, processedOutSize);
                    }
                }
            }
            finally
            {
                this.ReleaseStreams();
            }
        }

        private const int kPropSize = 5;
        private readonly byte[] properties = new byte[Encoder.kPropSize];

        public void WriteCoderProperties(Stream outStream)
        {
            this.properties[0] = (byte) ((this._posStateBits * 5 + this._numLiteralPosStateBits) * 9 + this._numLiteralContextBits);
            for (int i = 0; i < 4; i++)
            {
                this.properties[1 + i] = (byte) ((this._dictionarySize >> (8 * i)) & 0xFF);
            }

            outStream.Write(this.properties, 0, Encoder.kPropSize);
        }

        private readonly uint[] tempPrices = new uint[Base.kNumFullDistances];
        private uint _matchPriceCount;

        private void FillDistancesPrices()
        {
            for (uint i = Base.kStartPosModelIndex; i < Base.kNumFullDistances; i++)
            {
                uint posSlot = Encoder.GetPosSlot(i);
                int footerBits = (int) ((posSlot >> 1) - 1);
                uint baseVal = (2 | (posSlot & 1)) << footerBits;
                this.tempPrices[i] = BitTreeEncoder.ReverseGetPrice(this._posEncoders,
                    baseVal - posSlot - 1, footerBits, i - baseVal);
            }

            for (uint lenToPosState = 0; lenToPosState < Base.kNumLenToPosStates; lenToPosState++)
            {
                uint posSlot;
                BitTreeEncoder encoder = this._posSlotEncoder[lenToPosState];

                uint st = lenToPosState << Base.kNumPosSlotBits;
                for (posSlot = 0; posSlot < this._distTableSize; posSlot++)
                {
                    this._posSlotPrices[st + posSlot] = encoder.GetPrice(posSlot);
                }

                for (posSlot = Base.kEndPosModelIndex; posSlot < this._distTableSize; posSlot++)
                {
                    this._posSlotPrices[st + posSlot] += ((posSlot >> 1) - 1 - Base.kNumAlignBits) << BitEncoder.kNumBitPriceShiftBits;
                }

                uint st2 = lenToPosState * Base.kNumFullDistances;
                uint i;
                for (i = 0; i < Base.kStartPosModelIndex; i++)
                {
                    this._distancesPrices[st2 + i] = this._posSlotPrices[st + i];
                }

                for (; i < Base.kNumFullDistances; i++)
                {
                    this._distancesPrices[st2 + i] = this._posSlotPrices[st + Encoder.GetPosSlot(i)] + this.tempPrices[i];
                }
            }

            this._matchPriceCount = 0;
        }

        private void FillAlignPrices()
        {
            for (uint i = 0; i < Base.kAlignTableSize; i++)
            {
                this._alignPrices[i] = this._posAlignEncoder.ReverseGetPrice(i);
            }

            this._alignPriceCount = 0;
        }


        private static readonly string[] kMatchFinderIDs =
        {
            "BT2",
            "BT4"
        };

        private static int FindMatchFinder(string s)
        {
            for (int m = 0; m < Encoder.kMatchFinderIDs.Length; m++)
            {
                if (s == Encoder.kMatchFinderIDs[m])
                {
                    return m;
                }
            }

            return -1;
        }

        public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
        {
            for (uint i = 0; i < properties.Length; i++)
            {
                object prop = properties[i];
                switch (propIDs[i])
                {
                    case CoderPropID.NumFastBytes:
                    {
                        if (!(prop is int))
                        {
                            throw new InvalidParamException();
                        }

                        int numFastBytes = (int) prop;
                        if (numFastBytes < 5 || numFastBytes > Base.kMatchMaxLen)
                        {
                            throw new InvalidParamException();
                        }

                        this._numFastBytes = (uint) numFastBytes;
                        break;
                    }
                    case CoderPropID.Algorithm:
                    {
                        /*
                        if (!(prop is Int32))
                            throw new InvalidParamException();
                        Int32 maximize = (Int32)prop;
                        _fastMode = (maximize == 0);
                        _maxMode = (maximize >= 2);
                        */
                        break;
                    }
                    case CoderPropID.MatchFinder:
                    {
                        if (!(prop is string))
                        {
                            throw new InvalidParamException();
                        }

                        EMatchFinderType matchFinderIndexPrev = this._matchFinderType;
                        int m = Encoder.FindMatchFinder(((string) prop).ToUpper());
                        if (m < 0)
                        {
                            throw new InvalidParamException();
                        }

                        this._matchFinderType = (EMatchFinderType) m;
                        if (this._matchFinder != null && matchFinderIndexPrev != this._matchFinderType)
                        {
                            this._dictionarySizePrev = 0xFFFFFFFF;
                            this._matchFinder = null;
                        }

                        break;
                    }
                    case CoderPropID.DictionarySize:
                    {
                        const int kDicLogSizeMaxCompress = 30;
                        if (!(prop is int))
                        {
                            throw new InvalidParamException();
                        }

                        ;
                        int dictionarySize = (int) prop;
                        if (dictionarySize < (uint) (1 << Base.kDicLogSizeMin) ||
                            dictionarySize > (uint) (1 << kDicLogSizeMaxCompress))
                        {
                            throw new InvalidParamException();
                        }

                        this._dictionarySize = (uint) dictionarySize;
                        int dicLogSize;
                        for (dicLogSize = 0; dicLogSize < (uint) kDicLogSizeMaxCompress; dicLogSize++)
                        {
                            if (dictionarySize <= (uint) 1 << dicLogSize)
                            {
                                break;
                            }
                        }

                        this._distTableSize = (uint) dicLogSize * 2;
                        break;
                    }
                    case CoderPropID.PosStateBits:
                    {
                        if (!(prop is int))
                        {
                            throw new InvalidParamException();
                        }

                        int v = (int) prop;
                        if (v < 0 || v > (uint) Base.kNumPosStatesBitsEncodingMax)
                        {
                            throw new InvalidParamException();
                        }

                        this._posStateBits = v;
                        this._posStateMask = ((uint) 1 << this._posStateBits) - 1;
                        break;
                    }
                    case CoderPropID.LitPosBits:
                    {
                        if (!(prop is int))
                        {
                            throw new InvalidParamException();
                        }

                        int v = (int) prop;
                        if (v < 0 || v > Base.kNumLitPosStatesBitsEncodingMax)
                        {
                            throw new InvalidParamException();
                        }

                        this._numLiteralPosStateBits = v;
                        break;
                    }
                    case CoderPropID.LitContextBits:
                    {
                        if (!(prop is int))
                        {
                            throw new InvalidParamException();
                        }

                        int v = (int) prop;
                        if (v < 0 || v > Base.kNumLitContextBitsMax)
                        {
                            throw new InvalidParamException();
                        }

                        ;
                        this._numLiteralContextBits = v;
                        break;
                    }
                    case CoderPropID.EndMarker:
                    {
                        if (!(prop is bool))
                        {
                            throw new InvalidParamException();
                        }

                        this.SetWriteEndMarkerMode((bool) prop);
                        break;
                    }
                    default:
                        throw new InvalidParamException();
                }
            }
        }

        private uint _trainSize;

        public void SetTrainSize(uint trainSize)
        {
            this._trainSize = trainSize;
        }
    }
}