using System.Linq;
using System;
using System.Collections.Generic;
using DDTool.Structures;

namespace DDTool.Generators
{
    static public class GenFieldTags
    {
        static public string Generate(List<DDField> fields)
        {
            var lines = new List<string>();

            lines.Add("// This is a generated file.  Don't edit it directly!");
            lines.Add("");
            lines.Add("using System;");
            lines.Add("");
            lines.Add("namespace QuickFix.Fields");
            lines.Add("{");
            lines.Add("    /// <summary>");
            lines.Add("    /// FIX Field Tag Values");
            lines.Add("    /// </summary>");
            lines.Add("    public static class Tags");
            lines.Add("    {");

            foreach (var fld in fields)
                lines.Add($"        public const int {fld.Name} = {fld.Tag};");

            lines.Add("    }");
            lines.Add("}");

            return string.Join("\r\n", lines);
        }
    }
}
