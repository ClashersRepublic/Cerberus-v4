namespace LineageSoft.Magic.Titan.Math
{
    using LineageSoft.Magic.Titan.DataStream;

    public class LogicVector2
    {
        public int X;
        public int Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicVector2" /> class.
        /// </summary>
        public LogicVector2()
        {
            // LogicVector2.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicVector2" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public LogicVector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        ///     Adds the specified <see cref="LogicVector2" />.
        /// </summary>
        /// <param name="vector2">The vector2.</param>
        public void Add(LogicVector2 vector2)
        {
            this.X += vector2.X;
            this.Y += vector2.Y;
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        public LogicVector2 Clone()
        {
            return new LogicVector2(this.X, this.Y);
        }

        /// <summary>
        ///     Dot Product of two vectors.
        /// </summary>
        public int Dot(LogicVector2 vector2)
        {
            return this.X * vector2.X + this.Y * vector2.Y;
        }

        /// <summary>
        ///     Returns the unsigned angle in degrees between from and to.
        /// </summary>
        public int GetAngle()
        {
            return LogicMath.GetAngle(this.X, this.Y);
        }

        /// <summary>
        ///     Returns the unsigned angle in degrees between from and to.
        /// </summary>
        public int GetAngleBetween(int x, int y)
        {
            return LogicMath.GetAngleBetween(LogicMath.GetAngle(this.X, this.Y), LogicMath.GetAngle(x, y));
        }

        /// <summary>
        ///     Returns the distance between this vector and specified vector.
        /// </summary>
        public int GetDistance(LogicVector2 vector2)
        {
            int x = this.X - vector2.X;
            int distance = 0x7FFFFFFF;

            if (x + 46340 <= 0x16A08)
            {
                int y = this.Y - vector2.Y;

                if (y + 46340 <= 0x16A08)
                {
                    distance = x * x + y * y;
                }
            }

            return LogicMath.Sqrt(distance);
        }

        /// <summary>
        ///     Returns the distance between this vector and specified vector.
        /// </summary>
        public int GetDistanceSquared(LogicVector2 vector2)
        {
            int x = this.X - vector2.X;
            int distance = 0x7FFFFFFF;

            if (x + 46340 <= 0x16A08)
            {
                int y = this.Y - vector2.Y;

                if (y + 46340 <= 0x16A08)
                {
                    distance = x * x + y * y;
                }
            }

            return distance;
        }

        /// <summary>
        ///     Returns the distance between this vector and specified position.
        /// </summary>
        public int GetDistanceSquaredTo(int x, int y)
        {
            int distance = 0x7FFFFFFF;

            x += this.X;

            if (x + 46340 <= 0x16A08)
            {
                y += this.Y;

                if (y + 46340 <= 0x16A08)
                {
                    distance = x * x + y * y;
                }
            }

            return distance;
        }

        /// <summary>
        ///     Calculates the length of the vector.
        /// </summary>
        public int GetLength()
        {
            int length = 0x7FFFFFFF;

            if (46340 - this.X <= 0x16A08)
            {
                if (46340 - this.Y <= 0x16A08)
                {
                    length = this.X * this.X + this.Y * this.Y;
                }
            }

            return LogicMath.Sqrt(length);
        }

        /// <summary>
        ///     Calculates the length of the vector.
        /// </summary>
        public int GetLengthSquared()
        {
            int length = 0x7FFFFFFF;

            if (46340 - this.X <= 0x16A08)
            {
                if (46340 - this.Y <= 0x16A08)
                {
                    length = this.X * this.X + this.Y * this.Y;
                }
            }

            return length;
        }

        /// <summary>
        ///     Returns if the given vector is exactly equal to this vector.
        /// </summary>
        public bool IsEqual(LogicVector2 vector2)
        {
            if (vector2 != null)
            {
                return this.X == vector2.X && this.Y == vector2.Y;
            }

            return false;
        }

        /// <summary>
        ///     Returns if the vector is int area.
        /// </summary>
        public bool IsInArea(int minX, int minY, int maxX, int maxY)
        {
            if (this.X >= minX && this.Y >= minY)
            {
                return this.X < minX + maxX && this.Y < maxY + minY;
            }

            return false;
        }

        /// <summary>
        ///     Multiplies the components of two vectors by one another.
        /// </summary>
        public void Multiply(LogicVector2 vector2)
        {
            this.X *= vector2.X;
            this.Y *= vector2.Y;
        }

        /// <summary>
        ///     Turns the current vector into a unit vector. The result is a vector one unit in length pointing in the same
        ///     direction as the original vector.
        /// </summary>
        public void Normalize(int value)
        {
            int length = this.GetLengthSquared();

            if (length > 0)
            {
                this.X = this.X * value / length;
                this.Y = this.Y * value / length;
            }
        }

        /// <summary>
        ///     Rotates the Vector2 by the given angle, counter-clockwise assuming the y-axis points up.
        /// </summary>
        public void Rotate(int degrees)
        {
            this.X = LogicMath.GetRotatedX(this.X, this.Y, degrees);
            this.Y = LogicMath.GetRotatedY(this.X, this.Y, degrees);
        }

        /// <summary>
        ///     Sets this vector position.
        /// </summary>
        public void Set(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        ///     Subtracts a vector from a vector.
        /// </summary>
        public void Substract(LogicVector2 vector2)
        {
            this.X -= vector2.X;
            this.Y -= vector2.Y;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this.X = stream.ReadInt();
            this.Y = stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder stream)
        {
            stream.WriteInt(this.X);
            stream.WriteInt(this.Y);
        }

        /// <summary>
        ///     Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return "LogicVector2(" + this.X + "," + this.Y + ")";
        }
    }
}