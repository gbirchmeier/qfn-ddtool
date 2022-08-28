using DDTool.Structures;
using DDTool.Validations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace DDTool.Generators
{
    public class GenFields
    {
        public static string Generate(IEnumerable<DDField> fields)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("// This is a generated file. Don't edit it directly!");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("namespace QuickFix.Fields");
            stringBuilder.AppendLine("{");
            foreach (var fieldDefinition in fields)
            {
                var field = GenerateField(fieldDefinition);
                stringBuilder.AppendLine(field);
            }
            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();
        }
        private static string GenerateField(DDField fieldDefinition)
        {
            var name = fieldDefinition.Name;
            var tagValue = fieldDefinition.Tag;
            var baseClass = FieldTypeInfo.GetBaseClassType(fieldDefinition);
            var fieldValueType = FieldTypeInfo.GetBaseType(fieldDefinition);
            var fieldClass = FieldTypeInfo.GetQfnFieldClass(fieldDefinition);
            var stringbuilder = new StringBuilder();
            stringbuilder.AppendLine("    /// <summary>");
            stringbuilder.AppendLine($"    /// {name} Field");
            stringbuilder.AppendLine("    /// </summary>/");
            stringbuilder.AppendLine($"    public sealed class {name} : {baseClass}");
            stringbuilder.AppendLine("    {");
            stringbuilder.AppendLine($"       public const int TAG = {tagValue};");
            stringbuilder.AppendLine("");
            stringbuilder.AppendLine(GenerateConstructors(fieldClass, name, fieldValueType));
            stringbuilder.AppendLine(GenerateEnumerations(fieldDefinition, fieldValueType));
            stringbuilder.AppendLine("    }");
            stringbuilder.AppendLine("");
            stringbuilder.AppendLine("");
            return stringbuilder.ToString();
        }

        private static string GenerateEnumerations(DDField fieldDefinition, string enumType)
        {
            var stringbuilder = new StringBuilder();
            if (fieldDefinition.EnumDict?.Count > 0)
            {
                var valueTypeLiteralToken = GetValueTypeLiteralToken(enumType);
                stringbuilder.AppendLine("");
                stringbuilder.AppendLine("        // Field Enumerations");
                foreach (var enumeration in fieldDefinition.EnumDict)
                {
                    var fieldName = enumeration.Value.ToUpper();
                    var literal = enumeration.Key;
                    stringbuilder.AppendLine($"        public const {enumType} {fieldName} = {valueTypeLiteralToken}{literal}{valueTypeLiteralToken};");
                }
            }
            return stringbuilder.ToString();
        }

        private static string GenerateConstructors(QfnFieldClass fieldClass, string name, string fieldValueType)
        {
            var stringbuilder = new StringBuilder();
            stringbuilder.AppendLine($"       public {name}()");
            stringbuilder.AppendLine($"            :base(Tags.{name}) {{}}");
            stringbuilder.AppendLine($"       public {name}({fieldValueType} val)");
            stringbuilder.AppendLine($"            :base(Tags.{name}, val) {{}}");
            if (fieldClass == QfnFieldClass.DateTimeField || fieldClass == QfnFieldClass.DateOnlyField || fieldClass == QfnFieldClass.TimeOnlyField)
            {
                stringbuilder.AppendLine($"       public {name}({fieldValueType} val, bool showMilliseconds)");
                stringbuilder.AppendLine($"            :base(Tags.{name}, val, showMilliseconds) {{}}");
                stringbuilder.AppendLine($"       public {name}({fieldValueType} val, Converters.TimeStampPrecision precision)");
                stringbuilder.AppendLine($"            :base(Tags.{name}, val, precision) {{}}");
            }
            return stringbuilder.ToString();
        }

        private static string GetValueTypeLiteralToken(string enumType)
        {
            return enumType switch
            {
                "char" => "'",
                "string" => "\"",
                _ => ""
            };
        }
    }
}
