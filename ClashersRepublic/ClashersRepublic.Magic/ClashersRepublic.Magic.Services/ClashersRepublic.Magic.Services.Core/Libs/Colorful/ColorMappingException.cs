namespace ClashersRepublic.Magic.Services.Core.Libs.Colorful
{
    using System;

    /// <summary>
    ///     Encapsulates information relating to exceptions thrown during color mapping.
    /// </summary>
    public sealed class ColorMappingException : Exception
    {
        /// <summary>
        ///     The underlying Win32 error code associated with the exception that
        ///     has been trapped.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        ///     Encapsulates information relating to exceptions thrown during color mapping.
        /// </summary>
        /// <param name="errorCode">
        ///     The underlying Win32 error code associated with the exception that
        ///     has been trapped.
        /// </param>
        public ColorMappingException(int errorCode)
            : base(string.Format("Color conversion failed with system error code {0}!", errorCode))
        {
            this.ErrorCode = errorCode;
        }
    }
}