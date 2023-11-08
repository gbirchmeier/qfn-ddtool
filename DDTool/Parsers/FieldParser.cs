using System;
using System.Collections.Generic;
using System.Xml;
using DDTool.Structures;

namespace DDTool.Parsers;

public static class FieldParser {
    public static void ParseFields(XmlDocument doc, DataDictionary dd) {
        XmlNodeList nodeList = doc.SelectNodes("//fields/field");
        foreach (XmlNode fldEl in nodeList) {
            dd.AddField(CreateField(fldEl));
        }
    }

    private static DDField CreateField(XmlNode node) {
        string tagstr = node.Attributes["number"].Value;
        string name = node.Attributes["name"].Value;
        string fldType = node.Attributes["type"].Value;
        int tag = int.Parse(tagstr);
        Dictionary<string, string> enums = new();

        if (node.HasChildNodes) {
            foreach (XmlNode enumEl in node.SelectNodes(".//value")) {
                string description = string.Empty;
                if (enumEl.Attributes["description"] != null)
                    description = enumEl.Attributes["description"].Value;
                enums[enumEl.Attributes["enum"].Value] = description;
            }
        }

        return new DDField(tag, name, enums, fldType);
    }
}
