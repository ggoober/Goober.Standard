using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Goober.Base.Extensions
{
    public static class XmlExtensions
    {
        public static string SerializeToXml<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var xmlserializer = new XmlSerializer(value.GetType());
            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, value);
                return stringWriter.ToString();
            }
        }

        public static T DeserializeFromXml<T>(this string value)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var readStream = new MemoryStream(Encoding.Unicode.GetBytes(value ?? "")))
            {
                var result = (T) serializer.Deserialize(readStream);

                return result;
            }
        }
    }
}