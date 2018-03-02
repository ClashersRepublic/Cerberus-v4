namespace ClashersRepublic.Magic.Tools.Csv
{
    using System;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Debug;

    internal class Program
    {
        private static PluralizationService _pluralizationService;
        private static string _template;

        private static void Main(string[] args)
        {
            Program._template = File.ReadAllText("template.txt");
            Program._pluralizationService = PluralizationService.CreateService(new CultureInfo("en-us"));

            Directory.CreateDirectory("Output");

            Debugger.LogEvent += (sender, eventArgs) => Console.WriteLine("[LOG] " + eventArgs.FileName + ":" + eventArgs.Line + " : " + eventArgs.Text);
            Debugger.WarningEvent += (sender, eventArgs) => Console.WriteLine("[WARNING] " + eventArgs.FileName + ":" + eventArgs.Line + " : " + eventArgs.Text);
            Debugger.ErrorEvent += (sender, eventArgs) => Console.WriteLine("[ERROR] " + eventArgs.FileName + ":" + eventArgs.Line + " : " + eventArgs.Text);

            if (Directory.Exists("Assets"))
            {
                if (Directory.Exists("Assets/csv"))
                {
                    Program.ProcessDirectory("Assets/csv/");
                }

                if (Directory.Exists("Assets/logic"))
                {
                    Program.ProcessDirectory("Assets/logic/");
                }
            }
        }

        /// <summary>
        ///     Processes the specified directory.
        /// </summary>
        private static void ProcessDirectory(string directoryPath)
        {
            foreach (string filePath in Directory.GetFiles(directoryPath, "*.csv", SearchOption.TopDirectoryOnly))
            {
                Program.ProcessFile(filePath);
            }
        }

        /// <summary>
        ///     Processes the specified file.
        /// </summary>
        private static void ProcessFile(string filePath)
        {
            if (Path.GetFileName(filePath) == "animations.csv")
            {
                return;
            }

            CSVTable csvTable = Program.GetCSVTable(filePath);

            if (csvTable.GetColumnTypeCount() <= 1)
            {
                return;
            }
            
            StringBuilder memberBuilder = new StringBuilder();
            StringBuilder methodBuilder = new StringBuilder();

            string dataName = Program.GetDataName(filePath);
            string[] columnName = new string[csvTable.GetColumnTypeCount() - 1];
            string[] memberName = new string[csvTable.GetColumnTypeCount() - 1];
            string[] memberTypeName = new string[csvTable.GetColumnTypeCount() - 1];
            bool[] isArray = new bool[columnName.Length];

            for (int i = 1; i < csvTable.GetColumnTypeCount(); i++)
            {
                string name = csvTable.GetColumnName(i);

                if (name == "InfoTID" || name == "TID" || name == "InfoSWF" || name == "IconExportName")
                {
                    continue;
                }

                columnName[i - 1] = name;
                
                switch (csvTable.GetCSVColumn(i).ColumnType)
                {
                    case 0:
                    {
                        memberTypeName[i - 1] = "string";
                        break;
                    }

                    case 1:
                    {
                        memberTypeName[i - 1] = "int";
                        break;
                    }

                    case 2:
                    {
                        memberTypeName[i - 1] = "bool";
                        break;
                    }
                }

                for (int j = 0; j < csvTable.GetRowCount(); j++)
                {
                    if (csvTable.GetRowAt(j).GetBiggestArraySize(columnName[i - 1]) > 1)
                    {
                        isArray[i - 1] = true;
                        break;
                    }
                }
            }


            for (int i = 0; i < memberName.Length; i++)
            {
                if (!string.IsNullOrEmpty(columnName[i]))
                {
                    memberName[i] = Program.GetMemberName(columnName[i]);
                }
            }

            for (int i = 0; i < memberName.Length; i++)
            {
                if (memberName[i] != null)
                {
                    if (memberBuilder.Length > 0)
                    {
                        memberBuilder.AppendLine();
                        methodBuilder.AppendLine();
                    }

                    memberBuilder.Append("        private " + memberTypeName[i] + ' ' + memberName[i] + ";");

                    switch (memberTypeName[i])
                    {
                        case "bool":
                        {
                            memberBuilder.Append("        public " + memberTypeName[i] + ' ' + memberName[i] + "()");
                            break;
                        }
                    }
                }
            }

            File.WriteAllText("Output/" + dataName + ".cs", Program._template.Replace("#NAME#", dataName)
                                                                             .Replace("#MEMBERS#", memberBuilder.ToString())
                                                                             .Replace("#METHODS#", methodBuilder.ToString())); 
        }

        /// <summary>
        ///     Gets the data name.
        /// </summary>
        private static string GetDataName(string csvPath)
        {
            string[] words = Path.GetFileName(csvPath).Replace(".csv", string.Empty).Split('_');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] != "leagues2")
                {
                    words[i] = Program._pluralizationService.Singularize(words[i]);
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
                else
                {
                    words[i] = "league2";
                }
            }

            return "Logic" + string.Join("", words) + "Data";
        }

        /// <summary>
        ///     Gets the member name for specified column.
        /// </summary>
        private static string GetMemberName(string column)
        {
            return "_" + char.ToLower(column[0]) + column.Substring(1);
        }

        /// <summary>
        ///     Gets the csv table.
        /// </summary>
        private static CSVTable GetCSVTable(string csvPath)
        {
            return new CSVNode(File.ReadAllLines(csvPath), csvPath).GetTable();
        }
    }
}