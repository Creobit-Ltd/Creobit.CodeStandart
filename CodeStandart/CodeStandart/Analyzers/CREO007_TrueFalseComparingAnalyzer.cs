using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Creo007_TrueFalseComparingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO007";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression);

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var literal = context.Node;

            var parent = literal.Parent;

            if (parent.Kind() != SyntaxKind.EqualsExpression &&
                parent.Kind() != SyntaxKind.NotEqualsExpression)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, literal.Parent.GetLocation(), literal.Parent, literal));
        }
    }
}
