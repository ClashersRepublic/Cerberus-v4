namespace RivieraStudio.Magic.Services.Core
{
    using System;
    using System.IO;
    using System.Text;

    public class ConsoleOut : TextWriter
    {
        private readonly TextWriter _original;

        /// <summary>
        ///     Gets the encoding type.
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleOut" /> class.
        /// </summary>
        public ConsoleOut()
        {
            this._original = Console.Out;
            this._original.WriteLine(Environment.NewLine);
        }

        /// <summary>
        ///     Write the specified string.
        /// </summary>
        public override void Write(string message)
        {
            this._original.Write(message);
        }

        /// <summary>
        ///     Write a empty line.
        /// </summary>
        public override void WriteLine()
        {
            this._original.WriteLine();
        }

        /// <summary>
        ///     Writes a line.
        /// </summary>
        public override void WriteLine(string message)
        {
            if (message != null)
            {
                if (message.Length <= Console.WindowWidth)
                {
                    Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
                }

                this._original.WriteLine("{0}", message);
            }
            else
            {
                this._original.WriteLine("(null)");
            }
        }
    }
}