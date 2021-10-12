using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Labeling
{
    class CsvParser
    {
        private static string FilePath = "";
        public CsvParser(string path)
        {
            FilePath = path;
        }

        public IEnumerable<string[]> GetLine()
        {
            string separator = ",";
            Encoding encoding = Encoding.GetEncoding("shift-jis");
            using (Stream stream = new System.IO.FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (TextFieldParser parser = new TextFieldParser(stream, encoding ?? Encoding.UTF8, true, false))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.Delimiters = new[] { separator };
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;
                while (parser.EndOfData == false)
                {
                    string[] fields = parser.ReadFields();
                    yield return fields;
                }
            }
        }
    }
}
