using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using CodeStandart.Utilities;

namespace CodeStandart.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO017_UnderscoresInIndentifiersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO017";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId,
            Category,
            DiagnosticSeverity.Warning);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, 
            SyntaxKind.VariableDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.EventFieldDeclaration,
            SyntaxKind.PropertyDeclaration);

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var identifier = node.DescendantTokens().FirstOrDefault(token =>
                token.Kind() == SyntaxKind.IdentifierToken);

            if (identifier == default) return;

            var identifierText = identifier.ValueText.TrimStart('_');

            if (identifierText.Count(c => c == '_') > 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, identifier.GetLocation(), identifier));
            }
        }

    }
}
