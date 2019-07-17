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
    public class UndividedLocalDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO002";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var declaration = context.Node as VariableDeclarationSyntax;

            if (declaration.Parent is LocalDeclarationStatementSyntax)
            {
                if (declaration.DescendantNodes().Any(node => node.Kind() == SyntaxKind.InvocationExpression))
                {
                    return;
                }

                var declarationChild = declaration.ChildNodes();

                var declarators = declaration.Variables.ToList();

                if (declarators.Count > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, context.Node.GetLocation()));
                }
            }
        }
    }
}
