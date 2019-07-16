using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeStandart.Utilities;

namespace CodeStandart.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO011_EventArgsNotActionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO011";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Warning);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EventFieldDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = context.Node as EventFieldDeclarationSyntax;

            var typeIdentifierToken = eventDeclaration.DescendantTokens().FirstOrDefault(
                n => n.Kind() == SyntaxKind.IdentifierToken);

            if (typeIdentifierToken == default) return;

            if (typeIdentifierToken.Text == "Action")
            {
                var identifier = eventDeclaration
                    .Declaration.Variables
                        .FirstOrDefault().Identifier;

                context.ReportDiagnostic(Diagnostic.Create(_rule, eventDeclaration.GetLocation(), identifier.Text?? ""));
            }
        }
    }
}
