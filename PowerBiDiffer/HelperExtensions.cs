using System;
using System.IO;
using System.Xml;

namespace PowerBiDiffer
{
    internal static class HelperExtensions
    {
        internal static string PrettyPrintXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var writer = new StringWriter();
            doc.Save(writer);
            return writer.ToString();
        }

        internal static string ReadString(this Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream, true))
                return streamReader.ReadToEnd();
        }


        internal static MemoryStream ToMemoryStream(this byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes.Length);
            stream.Write((ReadOnlySpan<byte>)bytes);
            stream.Rewind();
            return stream;
        }

        internal static Stream Rewind(this Stream stream)
        {
            stream.Position = 0L;
            return stream;
        }

    }
}