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
                ParseFields(doc, dd);
                /*
                ParseMessages(RootDoc);
                ParseHeader(RootDoc);
                ParseTrailer(RootDoc)
                */
            }

            return dd;
        }

        public static void SetVersionInfo(XmlDocument doc, DataDictionary dd)
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
            if (node != null && !string.IsNullOrWhiteSpace(node.Value))
                dd.ServicePack = node.Value;
        }

        public static void ParseFields(XmlDocument doc, DataDictionary dd)
        {
            XmlNodeList nodeList = doc.SelectNodes("//fields/field");
            foreach (XmlNode fldEl in nodeList)
            {
                dd.AddField(CreateField(fldEl));
            }
        }

        private static DDField CreateField(XmlNode fldEl)
        {
            String tagstr = fldEl.Attributes["number"].Value;
            String name = fldEl.Attributes["name"].Value;
            String fldType = fldEl.Attributes["type"].Value;
            int tag = int.Parse(tagstr);
            Dictionary<String, String> enums = new Dictionary<String, String>();
            if (fldEl.HasChildNodes)
            {
                foreach (XmlNode enumEl in fldEl.SelectNodes(".//value"))
                {
                    string description = String.Empty;
                    if (enumEl.Attributes["description"] != null)
                        description = enumEl.Attributes["description"].Value;
                    enums[enumEl.Attributes["enum"].Value] = description;
                }
            }
            return new DDField(tag, name, enums, fldType);
        }
    }
}