using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DDTool.Exceptions;
using DDTool.Structures;

namespace DDTool.Parsers
{
    public static class DDParser
    {
        public static DataDictionary ParseFile(string path)
        {
            var dd = new DataDictionary();

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;

            using (var reader = XmlReader.Create(path, readerSettings))
            {
                var doc = new XmlDocument();
                doc.Load(reader);

                SetVersionInfo(doc, dd);
                /*
                ParseFields(RootDoc);
                ParseMessages(RootDoc);
                ParseHeader(RootDoc);
                ParseTrailer(RootDoc)
                */
            }

            return dd;
        }

        private static void SetVersionInfo(XmlDocument doc, DataDictionary dd)
        {
            dd.MajorVersion = doc.SelectSingleNode("/fix/@major").Value;
            dd.MinorVersion = doc.SelectSingleNode("/fix/@minor").Value;

            XmlNode node = doc.SelectSingleNode("/fix/@type");
            if (node != null)
            {
                if (new[] { "FIX", "FIXT" }.Contains(node.Value) == false)
                    throw new ParsingException($"Unsupported /fix/type value: '{node.Value}' (expected FIX or FIXT)");

                if (node.Value == "FIXT")
                    dd.IsFIXT = true;
            }

            node = doc.SelectSingleNode("/fix/@servicepack");
            if (node != null && dd.MajorVersion=="5" && !string.IsNullOrWhiteSpace(node.Value))
                dd.ServicePack = node.Value;
        }
    }
}