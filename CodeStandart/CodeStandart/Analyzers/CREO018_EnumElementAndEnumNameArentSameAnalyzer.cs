using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeStandart.Extensions;
using CodeStandart.Utilities;

namespace CodeStandart.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO018_EnumElementAndEnumNameArentSameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO018";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EnumMemberDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var enumMember = context.Node as EnumMemberDeclarationSyntax;

            var enumDeclaration = enumMember.Parent as EnumDeclarationSyntax;
            var enumName = enumDeclaration.Identifier;

            if (enumMember.Identifier.ToString() == enumName.ToString())
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, enumMember.GetLocation(), enumMember));
            }
        }
    }
}
