using System.Linq;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;
using DDTool.Parsers;
using DDTool.Structures;

namespace DDTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            var options = new Options(args);

            if (options.DDFiles.Count < 1)
            {
                Console.WriteLine("No input files.");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Thank you for using the QuickFIX/n DDTool 9000.");
                Console.WriteLine("I will attempt to read the following files:");
                foreach (var file in options.DDFiles)
                    Console.WriteLine($"* {file}");
            }

            if (string.IsNullOrWhiteSpace(options.OutputDir))
            {
                Console.WriteLine("No output dir was specified, so I won't generate anything.");
            }
            else if (!Directory.Exists(options.OutputDir))
            {
                Console.WriteLine($"OutputDir does not exist: {options.OutputDir}");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine($"Generated files will be written to: {options.OutputDir}");
            }

            var errors = new List<string>();

            var dds = ParseDDs(options);
            foreach (DataDictionary dd in dds)
            {
                List<string> ddErrors = DDTool.Validations.FieldValidator.Check(dd);
                foreach (string dde in ddErrors)
                    errors.Add($"{dd.SourceFile}: {dde}");
            }

            if (errors.Count > 0)
            {
                Console.WriteLine("============================");
                Console.WriteLine("Errors found.  Code generation (if commanded) will not run.");
                foreach (var err in errors)
                    Console.WriteLine($"* {err}");
            }

            if (!string.IsNullOrWhiteSpace(options.OutputDir))
            {
                var aggFields = AggregateFields(dds);


                Console.WriteLine("============================");
                Console.WriteLine("Writing files:");

                var fieldTagsPath = Path.Join(options.OutputDir, "QuickFIXn", "Fields", "FieldTags.cs");
                System.IO.File.WriteAllText(fieldTagsPath, DDTool.Generators.GenFieldTags.Generate(aggFields));
                Console.WriteLine($"* Wrote {fieldTagsPath}");
            }
        }

        static List<DataDictionary> ParseDDs(Options options)
        {
            var dds = new List<DataDictionary>();
            foreach (var path in options.DDFiles)
            {
                try
                {
                    var dd = DDParser.ParseFile(path);
                    Console.WriteLine($"Parsed! -- {dd.Identifier}");
                    dds.Add(dd);
                }
                catch (ApplicationException ex)
                {
                    Console.WriteLine($"Error parsing {path}:");
                    Console.WriteLine(ex.ToString());
                    Environment.Exit(1);
                }
            }
            return dds;
        }

        static List<DDField> AggregateFields(List<DataDictionary> dds)
        {
            // This impl is a little wierd, but it mirrors the weird Ruby design
            // and produces the same output.
            // Can probably write a cleaner version later.

            var rv = new List<DDField>();

            var fieldNames = new List<string>();
            foreach (var dd in dds.Where(x => !x.IsFIXT).OrderBy(x => x.Identifier))
            {
                foreach (var name in dd.FieldsByName.Keys)
                {
                    if(!fieldNames.Contains(name))
                        fieldNames.Add(name);
                }
            }

            foreach (var name in fieldNames)
                rv.Add(FindField(name, dds));

            return rv;
        }

        static DDField FindField(string name, List<DataDictionary> dds)
        {
            DDField rv = null;
            foreach (var dd in dds.Where(x => !x.IsFIXT).OrderByDescending(x => x.Identifier))
            {
                if(dd.FieldsByName.TryGetValue(name, out rv))
                    return rv;
            }
            throw new Exception($"couldn't find field: {name}");
        }
    }
}
