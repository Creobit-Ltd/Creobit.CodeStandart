using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO005_NewLineAfterUsingBlockAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO005";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeOperation, SyntaxKind.UsingDirective);

        private static void AnalyzeOperation(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = context.Node;

            var allNodes = usingDirective.Parent.ChildNodes().ToList();

            if (allNodes.Last() == usingDirective)
                return;

            var nextNode = allNodes[allNodes.IndexOf(usingDirective) + 1];

            if (AnalyzerUtility.CheckIfHaveEmptyStringBefore(nextNode)) return;

            var countOfUsingsAfter = allNodes.Count(node =>
                node.Kind() == SyntaxKind.UsingDirective &&
                node != usingDirective &&
                allNodes.IndexOf(node) > allNodes.IndexOf(usingDirective));

            if (countOfUsingsAfter == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, usingDirective.GetLocation(), usingDirective));
            }
        }
    }
}
