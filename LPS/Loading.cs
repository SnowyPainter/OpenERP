using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LPS
{
    public static class Loading
    {
        /// <summary>
        /// The key code of formatted text when splited by code
        /// </summary>
        public static int KeyPosition = 0;
        /// <summary>
        /// Formatted Text position when splited by code
        /// </summary>
        public static int TextPosition = 2;
        private static List<string> readStringsFromFile(string file)
        {
            if (file == null) return null;
            if (file.Length == 0) return null;
            List<string> list = new List<string>();
            using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
            {
                string item;
                while ((item = streamReader.ReadLine()) != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }
        /// <summary>
        /// It makes new loading class instance
        /// </summary>
        /// <param name="lpFile">*.lp file</param>
        /// <param name="splitCode">file's spliting code</param>
        /// <param name="overwrite">overlapped key exists, then overwrite that one</param>
        /// <returns>Return Language Packed that indexed with Key</returns>
        public static Dictionary<int, string> Load(List<string> lpLines, string splitCode, bool overwrite = false)
        {
            if (lpLines == null) return null;
            Dictionary<int, string> langpack = new Dictionary<int, string>();

            foreach(var line in lpLines)
            {
                if (line == null) continue;
                var splitedByCode = line.Split(splitCode);
                int key;
                if (!int.TryParse(splitedByCode[KeyPosition], out key)) continue;

                string text = splitedByCode[TextPosition];

                if (langpack.ContainsKey(key)) langpack[key] = text;
                else langpack.Add(key, text);
            }
            return langpack;
        }

        /// <summary>
        /// Return format value filled string
        /// "hi {0}, I'm {1}".Fill("David", "Charlie");
        /// </summary>
        public static string Fill(this string formatted, params object[] values)
        {
            for(int i = 0;i < values.Length;i++)
            {
                formatted = formatted.Replace("{" + i + "}", values[i].ToString());
            }
            return formatted;
        }
    }
}
