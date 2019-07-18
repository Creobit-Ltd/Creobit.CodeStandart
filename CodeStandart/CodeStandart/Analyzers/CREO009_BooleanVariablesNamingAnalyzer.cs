using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeStandart.Utilities;

#pragma warning disable IDE1006
namespace CodeStandart.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO009_BooleanVariablesNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO009";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Warning);

        private static ImmutableArray<string> CorrectPrefixes = ImmutableArray.Create
            ("is", "has", "can", "do", "was", "_is", "_has", "_can", "_do", "_was");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var declaration = context.Node as VariableDeclarationSyntax;
            var declarator = declaration.Variables.First();

            var type = declaration.Type;

            var equalsValueClause = declarator.DescendantNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.EqualsValueClause);

            if (type.ChildTokens().Any(token => token.Kind() == SyntaxKind.BoolKeyword)
                ||
                (equalsValueClause is EqualsValueClauseSyntax
                &&
                (equalsValueClause.ChildNodes().Any(node => node.Kind() == SyntaxKind.FalseLiteralExpression)
                ||
                equalsValueClause.ChildNodes().Any(node => node.Kind() == SyntaxKind.TrueLiteralExpression))))
            {
                var name = declarator.Identifier.ToString();

                foreach (var prefix in CorrectPrefixes)
                {
                    if (name.ToLower().StartsWith(prefix.ToLower()))
                        return;
                }

                context.ReportDiagnostic(Diagnostic.Create(_rule, declarator.GetLocation(), name));
            }
            
          
        }
    }
}
