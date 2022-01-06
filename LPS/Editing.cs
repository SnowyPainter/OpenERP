using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPS
{
    public class Line
    {
        public int Key { get; set; }
        public string Description { get; set; }
        public string FormatText { get; set; }

        public Line(int key, string description, string text)
        {
            Key = key; Description = description; FormatText = text;
        }
    }
    public static class LineExtension
    {
        public static string ToString(this Line line, string splitCode)
        {
            return $"{line.Key}{splitCode}{line.Description}{splitCode}{line.FormatText}";
        }
    }
    /// <summary>
    /// Recommended, The reason of setting LangLines public is implement Command Pattern what you want.
    /// </summary>
    public class Editing
    {
        public int KeyPosition { get; set; } = 0;
        public int DescriptionPosition { get; set; } = 1;
        public int TextPosition { get; set; } = 2;

        public List<Line> LangLines;

        public Editing()
        {
            LangLines = new List<Line>();
        }
        public void Load(FileInfo lpfile, string splitCode)
        {
            if (lpfile == null || LangLines == null) return;
            if (lpfile.Extension != "lp" || splitCode.Length == 0) return;

            foreach (var line in File.ReadAllLines(lpfile.FullName))
            {
                var splited = line.Split(splitCode);
                int key;
                if (!int.TryParse(splited[KeyPosition], out key)) continue;
                LangLines.Add(new Line(key, splited[DescriptionPosition], splited[TextPosition]));
            }
        }
        public Dictionary<int, string> Save()
        {
            var lp = new Dictionary<int, string>();
            foreach(var l in LangLines)
            {
                lp.Add(l.Key, l.FormatText);
            }
            return lp;
        }
        /// <summary>
        /// Overwrites the file and fill with texts
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename, string splitCode)
        {
            List<string> lines = new List<string>();
            LangLines.ForEach(l => lines.Add(l.ToString(splitCode)));
            File.WriteAllLines(filename, lines);
        }
    }
}
