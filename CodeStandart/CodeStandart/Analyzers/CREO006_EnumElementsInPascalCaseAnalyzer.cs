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
    public class Creo006_EnumElementsInPascalCaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Creo006";

        private static readonly LocalizableString Title = new LocalizableResourceString(
             nameof(Resources.CREO006_AnalyzerTitle),
             Resources.ResourceManager,
             typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.CREO006_AnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.CREO006_AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EnumMemberDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var enumMember = (CSharpSyntaxNode)context.Node;

            var memberText = enumMember.GetFirstToken().ToString();

            var parent = (EnumDeclarationSyntax)enumMember.Parent;
            var parentid = parent.Identifier;

            if (memberText != String.Empty && char.IsLower(memberText[0]))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, enumMember.GetLocation(), enumMember, parentid));
            }
        }
    }
}
