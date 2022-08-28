using DDTool.Structures;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DDGenerators.CodeGenerators
{
    public class FieldTagsCodeGenerator 
    {
        private static string FieldTagsClass = "FieldTags";
        public NamespaceDeclarationSyntax GenerateFieldTags(string fieldTagNamespace, IEnumerable<DDField> fields)
        {
            var usingDirective = UsingDirective(IdentifierName("System"));
            var namespaceDeclaration = NamespaceDeclaration(ParseName(fieldTagNamespace)).AddUsings(usingDirective);
            var members = GenerateMemberDeclarations(FieldTagsClass, fields);
            namespaceDeclaration = namespaceDeclaration.AddMembers(members);
            return namespaceDeclaration;
        }

        private MemberDeclarationSyntax[] GenerateMemberDeclarations(string className, IEnumerable<DDField> fields)
        {
            var xmlComment = SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, "///<summary>\r\n    /// FIX Field Tag Values\r\n    /// </summary>/");
            var classTrivia = TriviaList(xmlComment);
            var fieldTags = GenerateFieldDeclarations(fields);
            var classDeclaration = ClassDeclaration(className)
                .AddModifiers(Token(classTrivia, SyntaxKind.PublicKeyword, TriviaList()), Token(SyntaxKind.StaticKeyword))
                .AddMembers(fieldTags);
            return new[] { classDeclaration };
        }

        private FieldDeclarationSyntax[] GenerateFieldDeclarations(IEnumerable<DDField> fields)
        {
            var declarations = new List<FieldDeclarationSyntax>();
            foreach (var field in fields)
            {
                var token = Literal(field.Tag);
                var literalExpression = LiteralExpression(SyntaxKind.NumericLiteralExpression, token);
                var variableDeclaration = VariableDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)))
                .AddVariables(VariableDeclarator(Identifier(field.Name), null, EqualsValueClause(Token(SyntaxKind.EqualsToken), literalExpression)));
                var declaration = FieldDeclaration(variableDeclaration)
                    .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword))
                    .AddDeclarationVariables();
                declarations.Add(declaration);
            }
            return declarations.ToArray();
        }
    }
}
