using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessEngine.IO
{
    public static class XML {
        public static void Serialize<T>(this T value, string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamWriter wr = new StreamWriter(file))
            {
                xs.Serialize(wr, value);
            }
        }
        public static string Serialize<T>(T serialisableObject)
        {
            var xmlSerializer = new XmlSerializer(serialisableObject.GetType());

            using (var ms = new MemoryStream())
            {
                using (var xw = XmlWriter.Create(ms,
                    new XmlWriterSettings()
                    {
                        Encoding = Encoding.UTF8,
                        Indent = true,
                    }))
                {
                    xmlSerializer.Serialize(xw, serialisableObject);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        public static T Load<T>(string fileName)
        {
            using (var stream = System.IO.File.OpenRead(fileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }
        }
        public static void ToFile(this string text, string filepath)
        {
            File.WriteAllText(filepath, text, Encoding.UTF8);
        }
    }
}
