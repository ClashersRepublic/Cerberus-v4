namespace ClashersRepublic.Magic.Services.Home
{
    using System;
    using System.IO;
    using System.Text;

    internal class ConsoleWriter : TextWriter
    {
        internal static bool WriteMiddle = true;
        internal readonly TextWriter Original;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleWriter" /> class.
        /// </summary>
        internal ConsoleWriter()
        {
            this.Original = Console.Out;
            this.Original.WriteLine(Environment.NewLine);
        }

        /// <summary>
        ///     En cas de substitution dans une classe dérivée, retourne l'encodage de caractères dans lequel la sortie est écrite.
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return new UTF8Encoding();
            }
        }

        /// <summary>
        ///     Writes the line.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void WriteLine(string message)
        {
            if (message != null)
            {
                if (ConsoleWriter.WriteMiddle)
                {
                    if (message.Length <= Console.WindowWidth)
                    {
                        Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
                    }

                    this.Original.WriteLine("{0}", message);
                }
                else
                {
                    this.Original.WriteLine("   {0}", message);
                }
            }
            else
            {
                this.Original.WriteLine("(null)");
            }
        }

        /// <summary>
        ///     Écrit une marque de fin de ligne dans la chaîne ou le flux de texte.
        /// </summary>
        public override void WriteLine()
        {
            this.Original.WriteLine();
        }

        /// <summary>
        ///     Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Write(string message)
        {
            this.Original.Write(message);
        }
    }
}