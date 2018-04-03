namespace RivieraStudio.Magic.Services.Core.Exception
{
    using System;

    public class AssertionException : Exception
    {
        private readonly int _fileLine;
        private readonly string _filePath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AssertionException"/> class.
        /// </summary>
        public AssertionException(string filePath, int fileLine, string assertionMessage) : base(assertionMessage)
        {
            this._filePath = filePath;
            this._fileLine = fileLine;
        }

        /// <summary>
        ///     Gets the file path.
        /// </summary>
        public string GetFilePath()
        {
            return this._filePath;
        }

        /// <summary>
        ///     Gets the file line.
        /// </summary>
        public int GetFileLine()
        {
            return this._fileLine;
        }
    }
}