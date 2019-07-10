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
    public class CREO012_EventDelegates_EventHadlerSuffixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO012";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EventFieldDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = context.Node as EventFieldDeclarationSyntax;

            var delegateIdentifierToken = eventDeclaration.DescendantTokens().FirstOrDefault(
                n => n.Kind() == SyntaxKind.IdentifierToken);

            if (delegateIdentifierToken == null) return;

            if (!delegateIdentifierToken.Text.EndsWith("EventHandler"))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, eventDeclaration.GetLocation(), delegateIdentifierToken.Text));
            }
        }
    }
}
