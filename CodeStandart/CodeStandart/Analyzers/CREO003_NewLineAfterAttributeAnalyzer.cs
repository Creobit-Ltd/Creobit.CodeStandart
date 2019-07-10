using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO003_NewLineAfterAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO003";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);
        
        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AttributeList);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var attribute = context.Node;

            var closingBracket = attribute.GetLastToken();

            if (closingBracket.TrailingTrivia.Any() &&
                closingBracket.TrailingTrivia.First().Kind() == SyntaxKind.EndOfLineTrivia)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, context.Node.GetLocation()));
        }        
    }
}
