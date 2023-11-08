using System;
using System.Xml;
using System.Linq;
using DDTool.Exceptions;
using DDTool.Structures;

namespace DDTool.Parsers;

public static class VersionParser
{
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
}
