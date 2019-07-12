using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeStandart.Extensions;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Creo006_EnumElementsInPascalCaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO006";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EnumMemberDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var member = context.Node as EnumMemberDeclarationSyntax;

            if (!member.IsPascal())
            {
                var parent = member.Parent as EnumDeclarationSyntax;

                context.ReportDiagnostic(Diagnostic.Create(_rule, member.GetLocation(), member.Identifier, parent.Identifier));
            }
        }
    }
}
