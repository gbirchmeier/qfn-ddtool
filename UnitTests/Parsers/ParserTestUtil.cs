using System;
using System.IO;
using System.Text;
using System.Xml;
using DDTool.Structures;

namespace UnitTests.Parsers
{
    public delegate void ParserFunc(XmlDocument doc, DataDictionary dd);

    public static class ParserTestUtil
    {
        public static DataDictionary ReadDD(string xml, ParserFunc parserFunc)
        {
            var dd = new DataDictionary();

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                using (var reader = XmlReader.Create(stream, readerSettings))
                {
                    var doc = new XmlDocument();
                    doc.Load(reader);

                    parserFunc(doc, dd);
                }
            }
            return dd;
        }
    }
}
