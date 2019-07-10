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
    public class CREO004_NewLineBeforeDirectivesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO004_NewLineBeforeDirectives";

        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.CREO004_AnalyzerTitle),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.CREO004_AnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.CREO004_AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode,
            SyntaxKind.IfStatement,
            SyntaxKind.WhileStatement,
            SyntaxKind.ForEachStatement,
            SyntaxKind.ForStatement,
            SyntaxKind.BreakStatement,
            SyntaxKind.YieldReturnStatement,
            SyntaxKind.YieldBreakStatement,
            SyntaxKind.TryStatement,
            SyntaxKind.ContinueStatement,
            SyntaxKind.SwitchStatement,
            SyntaxKind.ReturnStatement);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (CSharpSyntaxNode)context.Node;

            if (CheckIfHaveEmptyStringBefore(node) ||
                CheckIfNodeFirstInBlock(node))
            {
                return;
            }

            var diagnosticLocation = node.HasLeadingTrivia ? node
                .GetLeadingTrivia()
                    .Last()
                        .GetLocation()
                : node.GetLocation();

            var messageArg = node.Kind();
            context.ReportDiagnostic(Diagnostic.Create(_rule, diagnosticLocation, messageArg));
        }

        private static bool CheckIfNodeFirstInBlock(CSharpSyntaxNode node)
        {
            var parent = node.Parent;

            return parent.ChildNodes().ElementAt(0) == node;
        }

        private static bool CheckIfHaveEmptyStringBefore(CSharpSyntaxNode node)
        {
            //8539 == SyntaxKind.EndOfLineTrivia
            var EndOfLineTrivias = node.GetLeadingTrivia()
                .Where(trivia => trivia.RawKind == 8539);

            return EndOfLineTrivias.Count() > 0 ? true : false;
        }
    }
}
