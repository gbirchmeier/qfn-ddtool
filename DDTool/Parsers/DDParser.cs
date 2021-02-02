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
                ParseMessages(doc, dd);
                /*
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

        private static DDField CreateField(XmlNode node)
        {
            String tagstr = node.Attributes["number"].Value;
            String name = node.Attributes["name"].Value;
            String fldType = node.Attributes["type"].Value;
            int tag = int.Parse(tagstr);
            Dictionary<String, String> enums = new Dictionary<String, String>();
            if (node.HasChildNodes)
            {
                foreach (XmlNode enumEl in node.SelectNodes(".//value"))
                {
                    string description = String.Empty;
                    if (enumEl.Attributes["description"] != null)
                        description = enumEl.Attributes["description"].Value;
                    enums[enumEl.Attributes["enum"].Value] = description;
                }
            }
            return new DDField(tag, name, enums, fldType);
        }

        public static void ParseMessages(XmlDocument doc, DataDictionary dd)
        {
            XmlNodeList nodeList = doc.SelectNodes("//messages/message");
            foreach (XmlNode msgNode in nodeList)
            {
                dd.AddMessage(CreateMessage(msgNode));
            }
        }

        private static DDMessage CreateMessage(XmlNode node)
        {
            var ddMsg = new DDMessage(
                node.Attributes["name"].Value,
                node.Attributes["msgtype"].Value,
                node.Attributes["msgcat"].Value);
                
            return ddMsg;
        }
    }
}