using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeOperation, SyntaxKind.UsingDirective);

        private static void AnalyzeOperation(SyntaxNodeAnalysisContext context)
        {
            var node = (CSharpSyntaxNode)context.Node;

            var allNodes = node.Parent.ChildNodes().ToList();

            if (allNodes.Last() == node)
                return;

            var nextNode = allNodes[allNodes.IndexOf(node) + 1];

            if (CheckIfHaveEmptyStringBefore(nextNode))
                return;

            var countOfUsingsAfter = allNodes.Count(n => 
                n.GetType() == typeof(UsingDirectiveSyntax) &&
                n != node &&
                allNodes.IndexOf(n) > allNodes.IndexOf(node));

            if (countOfUsingsAfter > 0)
                return;

            context.ReportDiagnostic(Diagnostic.Create(_rule, node.GetLocation(), node));
        }

        private static bool CheckIfHaveEmptyStringBefore(SyntaxNode node)
        {
            //8539 == SyntaxKind.EndOfLineTrivia
            var EndOfLineTrivias = node.GetLeadingTrivia()
                .Where(trivia => trivia.RawKind == 8539);

            return EndOfLineTrivias.Count() > 0 ? true : false;
        }
    }
}
