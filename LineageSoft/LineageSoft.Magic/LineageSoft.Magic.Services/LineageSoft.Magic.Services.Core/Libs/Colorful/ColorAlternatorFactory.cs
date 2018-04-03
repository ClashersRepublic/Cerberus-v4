﻿namespace LineageSoft.Magic.Services.Core.Libs.Colorful
{
    using System.Drawing;

    public sealed class ColorAlternatorFactory
    {
        public ColorAlternator GetAlternator(string[] patterns, params Color[] colors)
        {
            return new PatternBasedColorAlternator<string>(new TextPatternCollection(patterns), colors);
        }

        public ColorAlternator GetAlternator(int frequency, params Color[] colors)
        {
            return new FrequencyBasedColorAlternator(frequency, colors);
        }
    }
}