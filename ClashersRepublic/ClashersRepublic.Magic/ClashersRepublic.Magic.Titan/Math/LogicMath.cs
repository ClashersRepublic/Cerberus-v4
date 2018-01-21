namespace ClashersRepublic.Magic.Titan.Math
{
    public static class LogicMath
    {
        private const int FIXED_SHIFT = 10;

        private static readonly byte[] SQRT_TABLE =
        {
            0x00, 0x10, 0x16, 0x1B, 0x20, 0x23, 0x27, 0x2A, 0x2D,
            0x30, 0x32, 0x35, 0x37, 0x39, 0x3B, 0x3D, 0x40, 0x41,
            0x43, 0x45, 0x47, 0x49, 0x4B, 0x4C, 0x4E, 0x50, 0x51,
            0x53, 0x54, 0x56, 0x57, 0x59, 0x5A, 0x5B, 0x5D, 0x5E,
            0x60, 0x61, 0x62, 0x63, 0x65, 0x66, 0x67, 0x68, 0x6A,
            0x6B, 0x6C, 0x6D, 0x6E, 0x70, 0x71, 0x72, 0x73, 0x74,
            0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D,
            0x7E, 0x80, 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86,
            0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
            0x90, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x96,
            0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9B, 0x9C, 0x9D, 0x9E,
            0x9F, 0xA0, 0xA0, 0xA1, 0xA2, 0xA3, 0xA3, 0xA4, 0xA5,
            0xA6, 0xA7, 0xA7, 0xA8, 0xA9, 0xAA, 0xAA, 0xAB, 0xAC,
            0xAD, 0xAD, 0xAE, 0xAF, 0xB0, 0xB0, 0xB1, 0xB2, 0xB2,
            0xB3, 0xB4, 0xB5, 0xB5, 0xB6, 0xB7, 0xB7, 0xB8, 0xB9,
            0xB9, 0xBA, 0xBB, 0xBB, 0xBC, 0xBD, 0xBD, 0xBE, 0xBF,
            0xC0, 0xC0, 0xC1, 0xC1, 0xC2, 0xC3, 0xC3, 0xC4, 0xC5,
            0xC5, 0xC6, 0xC7, 0xC7, 0xC8, 0xC9, 0xC9, 0xCA, 0xCB,
            0xCB, 0xCC, 0xCC, 0xCD, 0xCE, 0xCE, 0xCF, 0xD0, 0xD0,
            0xD1, 0xD1, 0xD2, 0xD3, 0xD3, 0xD4, 0xD4, 0xD5, 0xD6,
            0xD6, 0xD7, 0xD7, 0xD8, 0xD9, 0xD9, 0xDA, 0xDA, 0xDB,
            0xDB, 0xDC, 0xDD, 0xDD, 0xDE, 0xDE, 0xDF, 0xE0, 0xE0,
            0xE1, 0xE1, 0xE2, 0xE2, 0xE3, 0xE3, 0xE4, 0xE5, 0xE5,
            0xE6, 0xE6, 0xE7, 0xE7, 0xE8, 0xE8, 0xE9, 0xEA, 0xEA,
            0xEB, 0xEB, 0xEC, 0xEC, 0xED, 0xED, 0xEE, 0xEE, 0xEF,
            0xF0, 0xF0, 0xF1, 0xF1, 0xF2, 0xF2, 0xF3, 0xF3, 0xF4,
            0xF4, 0xF5, 0xF5, 0xF6, 0xF6, 0xF7, 0xF7, 0xF8, 0xF8,
            0xF9, 0xF9, 0xFA, 0xFA, 0xFB, 0xFB, 0xFC, 0xFC, 0xFD,
            0xFD, 0xFE, 0xFE, 0xFF
        };

        private static readonly byte[] ATAN_TABLE =
        {
            0x00, 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x03, 0x04,
            0x04, 0x04, 0x05, 0x05, 0x06, 0x06, 0x07, 0x07, 0x08,
            0x08, 0x08, 0x09, 0x09, 0x0A, 0x0A, 0x0B, 0x0B, 0x0B,
            0x0C, 0x0C, 0x0D, 0x0D, 0x0E, 0x0E, 0x0E, 0x0F, 0x0F,
            0x10, 0x10, 0x11, 0x11, 0x11, 0x12, 0x12, 0x13, 0x13,
            0x13, 0x14, 0x14, 0x15, 0x15, 0x15, 0x16, 0x16, 0x16,
            0x17, 0x17, 0x18, 0x18, 0x18, 0x19, 0x19, 0x19, 0x1A,
            0x1A, 0x1B, 0x1B, 0x1B, 0x1C, 0x1C, 0x1C, 0x1D, 0x1D,
            0x1D, 0x1E, 0x1E, 0x1E, 0x1F, 0x1F, 0x1F, 0x20, 0x20,
            0x20, 0x21, 0x21, 0x21, 0x22, 0x22, 0x22, 0x23, 0x23,
            0x23, 0x23, 0x24, 0x24, 0x24, 0x25, 0x25, 0x25, 0x25,
            0x26, 0x26, 0x26, 0x27, 0x27, 0x27, 0x27, 0x28, 0x28,
            0x28, 0x28, 0x29, 0x29, 0x29, 0x29, 0x2A, 0x2A, 0x2A,
            0x2A, 0x2B, 0x2B, 0x2B, 0x2B, 0x2C, 0x2C, 0x2C, 0x2C,
            0x2D, 0x2D, 0x2D
        };

        private static readonly byte[] SIN_TABLE =
        {
            0x00, 0x10, 0x16, 0x1B, 0x20, 0x23, 0x27, 0x2A, 0x2D,
            0x30, 0x32, 0x35, 0x37, 0x39, 0x3B, 0x3D, 0x40, 0x41,
            0x43, 0x45, 0x47, 0x49, 0x4B, 0x4C, 0x4E, 0x50, 0x51,
            0x53, 0x54, 0x56, 0x57, 0x59, 0x5A, 0x5B, 0x5D, 0x5E,
            0x60, 0x61, 0x62, 0x63, 0x65, 0x66, 0x67, 0x68, 0x6A,
            0x6B, 0x6C, 0x6D, 0x6E, 0x70, 0x71, 0x72, 0x73, 0x74,
            0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D,
            0x7E, 0x80, 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86,
            0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
            0x90, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x96,
            0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9B, 0x9C, 0x9D, 0x9E,
            0x9F, 0xA0, 0xA0, 0xA1, 0xA2, 0xA3, 0xA3, 0xA4, 0xA5,
            0xA6, 0xA7, 0xA7, 0xA8, 0xA9, 0xAA, 0xAA, 0xAB, 0xAC,
            0xAD, 0xAD, 0xAE, 0xAF, 0xB0, 0xB0, 0xB1, 0xB2, 0xB2,
            0xB3, 0xB4, 0xB5, 0xB5, 0xB6, 0xB7, 0xB7, 0xB8, 0xB9,
            0xB9, 0xBA, 0xBB, 0xBB, 0xBC, 0xBD, 0xBD, 0xBE, 0xBF,
            0xC0, 0xC0, 0xC1, 0xC1, 0xC2, 0xC3, 0xC3, 0xC4, 0xC5,
            0xC5, 0xC6, 0xC7, 0xC7, 0xC8, 0xC9, 0xC9, 0xCA, 0xCB,
            0xCB, 0xCC, 0xCC, 0xCD, 0xCE, 0xCE, 0xCF, 0xD0, 0xD0,
            0xD1, 0xD1, 0xD2, 0xD3, 0xD3, 0xD4, 0xD4, 0xD5, 0xD6,
            0xD6, 0xD7, 0xD7, 0xD8, 0xD9, 0xD9, 0xDA, 0xDA, 0xDB,
            0xDB, 0xDC, 0xDD, 0xDD, 0xDE, 0xDE, 0xDF, 0xE0, 0xE0,
            0xE1, 0xE1, 0xE2, 0xE2, 0xE3, 0xE3, 0xE4, 0xE5, 0xE5,
            0xE6, 0xE6, 0xE7, 0xE7, 0xE8, 0xE8, 0xE9, 0xEA, 0xEA,
            0xEB, 0xEB, 0xEC, 0xEC, 0xED, 0xED, 0xEE, 0xEE, 0xEF,
            0xF0, 0xF0, 0xF1, 0xF1, 0xF2, 0xF2, 0xF3, 0xF3, 0xF4,
            0xF4, 0xF5, 0xF5, 0xF6, 0xF6, 0xF7, 0xF7, 0xF8, 0xF8,
            0xF9, 0xF9, 0xFA, 0xFA, 0xFB, 0xFB, 0xFC, 0xFC, 0xFD,
            0xFD, 0xFE, 0xFE, 0xFF
        };

        private static readonly int[] DAYS_IN_MONTH =
        {
            31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
        };

        /// <summary>
        ///     Returns the absolute value of valueA int value.
        /// </summary>
        public static int Abs(int value)
        {
            if (value < 0)
            {
                return -value;
            }

            return value;
        }

        /// <summary>
        ///     Returns the trigonometric cosine of an angle.
        /// </summary>
        public static int Cos(int angle)
        {
            return LogicMath.Sin(angle + 90);
        }

        /// <summary>
        ///     Gets the angle with x and y.
        /// </summary>
        public static int GetAngle(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return 0;
            }

            if (x > 0 && y >= 0)
            {
                if (y < x)
                {
                    return LogicMath.ATAN_TABLE[(y << 7) / x];
                }

                return -LogicMath.ATAN_TABLE[(x << 7) / y] + 90;
            }

            int num = LogicMath.Abs(x);

            if (x <= 0 && y > 0)
            {
                if (num < y)
                {
                    return LogicMath.ATAN_TABLE[(num << 7) / y] + 90;
                }

                return -LogicMath.ATAN_TABLE[(y << 7) / num] + 180;
            }

            int num2 = LogicMath.Abs(y);

            if (x < 0 && y <= 0)
            {
                if (num2 < num)
                {
                    return LogicMath.ATAN_TABLE[(num2 << 7) / num] + 180;
                }

                return -LogicMath.ATAN_TABLE[(num << 7) / num2] + 270;
            }

            if (num < num2)
            {
                return LogicMath.ATAN_TABLE[(num << 7) / num2] + 270;
            }

            return LogicMath.NormalizeAngle360(-LogicMath.ATAN_TABLE[(num2 << 7) / num] + 360);
        }

        /// <summary>
        ///     Gets the angle between the two angles.
        /// </summary>
        public static int GetAngleBetween(int angle1, int angle2)
        {
            return LogicMath.Abs(LogicMath.NormalizeAngle180(angle1 - angle2));
        }

        /// <summary>
        ///     Gets the rotated x.
        /// </summary>
        public static int GetRotatedX(int x, int y, int angle)
        {
            x = x * LogicMath.Cos(angle) - y * LogicMath.Sin(angle);
            return x >> LogicMath.FIXED_SHIFT;
        }

        /// <summary>
        ///     Gets the rotated y.
        /// </summary>
        public static int GetRotatedY(int x, int y, int angle)
        {
            y = x * LogicMath.Sin(angle) + y * LogicMath.Cos(angle);
            return y >> LogicMath.FIXED_SHIFT;
        }

        /// <summary>
        ///     Normalizes valueA 180 angle.
        /// </summary>
        public static int NormalizeAngle180(int angle)
        {
            angle = LogicMath.NormalizeAngle360(angle);

            if (angle >= 180)
            {
                return angle - 360;
            }

            return angle;
        }

        /// <summary>
        ///     Normalizes value 360 angle.
        /// </summary>
        public static int NormalizeAngle360(int angle)
        {
            angle %= 360;

            if (angle < 0)
            {
                return angle + 360;
            }

            return angle;
        }

        /// <summary>
        ///     Returns the trigonometric sine of an angle.
        /// </summary>
        public static int Sin(int angle)
        {
            angle = LogicMath.NormalizeAngle360(angle);

            if (angle < 180)
            {
                if (angle > 90)
                {
                    angle = 180 - angle;
                }

                return LogicMath.SIN_TABLE[angle];
            }

            angle -= 180;

            if (angle > 90)
            {
                angle = 180 - angle;
            }

            return -LogicMath.SIN_TABLE[angle];
        }

        /// <summary>
        ///     Returns the square root of the specified value.
        /// </summary>
        public static int Sqrt(int value)
        {
            if (value >= 0x10000)
            {
                int num;

                if (value >= 0x1000000)
                {
                    if (value >= 0x10000000)
                    {
                        if (value >= 0x40000000)
                        {
                            if (value >= 0x7FFFFFFF)
                            {
                                return 0xFFFF;
                            }

                            num = LogicMath.SQRT_TABLE[value >> 24] << 8;
                        }
                        else
                        {
                            num = LogicMath.SQRT_TABLE[value >> 22] << 7;
                        }
                    }
                    else if (value >= 0x4000000)
                    {
                        num = LogicMath.SQRT_TABLE[value >> 20] << 6;
                    }
                    else
                    {
                        num = LogicMath.SQRT_TABLE[value >> 18] << 5;
                    }

                    num = (num + 1 + value / num) >> 1;
                    num = (num + 1 + value / num) >> 1;

                    return num * num <= value ? num : num - 1;
                }

                if (value >= 0x100000)
                {
                    if (value >= 0x400000)
                    {
                        num = LogicMath.SQRT_TABLE[value >> 16] << 4;
                    }
                    else
                    {
                        num = LogicMath.SQRT_TABLE[value >> 14] << 3;
                    }
                }
                else if (value >= 0x40000)
                {
                    num = LogicMath.SQRT_TABLE[value >> 12] << 2;
                }
                else
                {
                    num = LogicMath.SQRT_TABLE[value >> 10] << 1;
                }

                num = (num + 1 + value / num) >> 1;

                return num * num <= value ? num : num - 1;
            }

            if (value >= 0x100)
            {
                int num;

                if (value >= 0x1000)
                {
                    if (value >= 0x4000)
                    {
                        num = LogicMath.SQRT_TABLE[value >> 8] + 1;
                    }
                    else
                    {
                        num = (LogicMath.SQRT_TABLE[value >> 6] >> 1) + 1;
                    }
                }
                else if (value >= 0x400)
                {
                    num = (LogicMath.SQRT_TABLE[value >> 4] >> 2) + 1;
                }
                else
                {
                    num = (LogicMath.SQRT_TABLE[value >> 2] >> 3) + 1;
                }

                return num * num <= value ? num : num - 1;
            }

            if (value >= 0)
            {
                return LogicMath.SQRT_TABLE[value] >> 4;
            }

            return -1;
        }

        /// <summary>
        ///     Clamps value between value minimum int and maximum int value.
        /// </summary>
        public static int Clamp(int clampValue, int minValue, int maxValue)
        {
            if (clampValue >= maxValue)
            {
                return maxValue;
            }

            if (clampValue <= minValue)
            {
                return minValue;
            }

            return clampValue;
        }

        /// <summary>
        ///     Returns the greater of two integer values.
        /// </summary>
        public static int Max(int valueA, int valueB)
        {
            if (valueA >= valueB)
            {
                return valueA;
            }

            return valueB;
        }

        /// <summary>
        ///     Returns the smaller of two integer values.
        /// </summary>
        public static int Min(int valueA, int valueB)
        {
            if (valueA <= valueB)
            {
                return valueA;
            }

            return valueB;
        }

        /// <summary>
        ///     Gets the number of days in month.
        /// </summary>
        public static int GetDaysInMonth(int month, bool bixestile)
        {
            if (bixestile)
            {
                return 29;
            }

            return LogicMath.DAYS_IN_MONTH[month];
        }
    }
}