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
            else
            {
                Console.WriteLine($"If codegen was supported, I'd write it to: {options.OutputDir}");
            }

            var errors = new List<string>();

            var dds = ParseDDs(options);
            foreach (DataDictionary dd in dds)
            {
                List<string> ddErrors = DDTool.Validations.FieldValidator.Check(dd);
                foreach(string dde in ddErrors)
                    errors.Add($"{dd.SourceFile}: {dde}");
            }

            if (errors.Count > 0)
            {
                Console.WriteLine("============================");
                Console.WriteLine("Errors found.  Code generation (if commanded) will not run.");
                foreach(var err in errors)
                    Console.WriteLine($"* {err}");
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

/*
        static List<DDField> AggregateFields(List<DataDictionary> dds)
        {
            var fields = new List<DDField>();
            return fields;
        }
*/
    }
}
