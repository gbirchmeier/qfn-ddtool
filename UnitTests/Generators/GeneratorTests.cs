using DDGenerators.CodeGenerators;
using DDTool.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Parsers;
using Xunit;

namespace UnitTests.Generators
{
    public class GeneratorTests
    {
        [Theory]
        [InlineData("UnitTests.Resources.Fields.Example.txt")]
        public async Task Check_GenerateFieldTags_GeneratesSyntaxTree(string resourceName)
        {
            var fields = await resourceName.GetResourceStringAsync<FieldParserTests>();
            var dd = ParserTestUtil.ReadDD(fields, ParserTask.Fields);
            var code = GenFieldTags.Generate(dd.FieldsByTag.Values);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            Assert.NotNull(syntaxTree);
        }

        [Theory]
        [InlineData("UnitTests.Resources.Fields.Example.txt")]
        public async Task Check_CodeGenerateFieldTags_GeneratesSyntaxTree(string resourceName)
        {
            var fields = await resourceName.GetResourceStringAsync<FieldParserTests>();
            var dd = ParserTestUtil.ReadDD(fields, ParserTask.Fields);
            var generator = new FieldTagsCodeGenerator();
            var nameSpaceDeclaration = generator.GenerateFieldTags("QuickFix.Fields", dd.FieldsByName.Values);
            var code = nameSpaceDeclaration.NormalizeWhitespace().ToFullString();
            Assert.NotNull(code);
        }

        [Theory]
        [InlineData("UnitTests.Resources.Fields.Example.txt")]
        [InlineData("UnitTests.Resources.Messages.JustFields.txt")]
        public async Task Check_GenerateFields_GeneratesSyntaxTree(string resourceName)
        {
            var fields = await resourceName.GetResourceStringAsync<FieldParserTests>();
            var dd = ParserTestUtil.ReadDD(fields, ParserTask.Fields);
            var code = GenFields.Generate(dd.FieldsByTag.Values);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            Assert.NotNull(syntaxTree);
        }
    }
}
