using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart.Code
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO014_DontUseInnerNameSpaceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO014";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.NamespaceDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var namespaceNode = context.Node as NamespaceDeclarationSyntax;

            if (namespaceNode.Parent.Kind() == SyntaxKind.NamespaceDeclaration)
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, namespaceNode.GetLocation(), namespaceNode.Name));
            }
        }
    }
}