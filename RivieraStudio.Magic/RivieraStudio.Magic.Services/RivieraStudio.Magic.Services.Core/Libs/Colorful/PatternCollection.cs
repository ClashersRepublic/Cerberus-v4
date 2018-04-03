﻿namespace RivieraStudio.Magic.Services.Core.Libs.Colorful
{
    using System.Collections.Generic;

    /// <summary>
    ///     Represents a collection of Pattern objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PatternCollection<T> : IPrototypable<PatternCollection<T>>
    {
        protected List<Pattern<T>> patterns = new List<Pattern<T>>();

        public PatternCollection<T> Prototype()
        {
            return this.PrototypeCore();
        }

        protected abstract PatternCollection<T> PrototypeCore();

        /// <summary>
        ///     Attempts to match any of the PatternCollection's member Patterns against a string input.
        /// </summary>
        /// <param name="input">The input against which Patterns will potentially be matched.</param>
        /// <returns>
        ///     Returns 'true' if any of the PatternCollection's member Patterns matches against
        ///     the input string.
        /// </returns>
        public abstract bool MatchFound(string input);
    }
}