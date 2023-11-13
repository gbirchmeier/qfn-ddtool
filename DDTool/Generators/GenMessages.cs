using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DDTool.Structures;

namespace DDTool.Generators;

public static class GenMessages {

    /// <summary>
    ///
    /// </summary>
    /// <param name="baseDir"></param>
    /// <param name="dd"></param>
    /// <returns>List of filenames that were written</returns>
    public static List<string> WriteFilesForDD(string baseDir, DataDictionary dd) {
        List<string> rv = new();
        rv.Add(WriteBaseMessageFile(baseDir, dd));
//        foreach (var msg in dd.Messages.Values) {
//            rv.Add(WriteFile(baseDir, msg, dd));
//        }

        rv.Sort();
        return rv;
    }

    private static string WriteBaseMessageFile(string baseDir, DataDictionary dd) {
        var beginString = dd.IdentifierNoDots.Contains("FIX50") ? "FIXT11" : dd.IdentifierNoDots;
        string filePath = Path.Join($"{baseDir}", "Messages", dd.IdentifierNoDots, "Message.cs");

        var lines = new List<string>
        {
            "// This is a generated file.  Don't edit it directly!",
            "",
            "namespace QuickFix",
            "{",
            $"    namespace {dd.IdentifierNoDots}",
            "    {",
            "        public abstract class Message : QuickFix.Message",
            "        {",
            "            public Message()",
            "                : base()",
            "            {",
            $"                this.Header.SetField(new QuickFix.Fields.BeginString(QuickFix.FixValues.BeginString.{beginString}));",
            "            }",
            "        }",
            "    }",
            "}"
        };

        File.WriteAllText(filePath, string.Join("\r\n", lines) + "\n");

        return filePath;
    }
}
