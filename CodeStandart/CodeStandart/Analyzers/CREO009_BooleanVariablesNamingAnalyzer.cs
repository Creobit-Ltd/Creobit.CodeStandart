using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

#pragma warning disable IDE1006
namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO009_BooleanVariablesNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO009";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Warning);

        private static ImmutableArray<string> CorrectPrefixes = ImmutableArray.Create
            ("is", "has", "can", "do", "_is", "_has", "_can", "_do");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var declaration = (VariableDeclarationSyntax)context.Node;
            var declarator = declaration.Variables.First();

            var type = declaration.Type;

            if (type.ChildTokens().FirstOrDefault(token => token.Kind() == SyntaxKind.BoolKeyword) == default)
                return;

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
