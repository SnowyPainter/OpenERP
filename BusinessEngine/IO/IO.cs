using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessEngine.IO
{
    public static class XMLWriter {
        public static string Serialize<T>(T book)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, book);
                    xml = sww.ToString();
                }
            }
            return xml;
        }

        public static void ToFile(this string text, string filepath)
        {
            File.WriteAllText(filepath, text);
        }
    }
}
