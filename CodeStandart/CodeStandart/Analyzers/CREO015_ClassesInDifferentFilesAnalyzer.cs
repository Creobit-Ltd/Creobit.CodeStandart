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
    public class CREO015_ClassesInDifferentFilesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO015";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Warning);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxTreeAction(Analyze);

        private static void Analyze(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot();

            var childClasses = root.DescendantNodes().Where(
                node => (node.Kind() == SyntaxKind.ClassDeclaration ||
                         node.Kind() == SyntaxKind.InterfaceDeclaration)
                         &&
                        (node.Parent == root ||
                         node.Parent.Kind() == SyntaxKind.NamespaceDeclaration))
                .ToList();

            if (childClasses.Count() > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, root.GetLocation()));
            }
        }
    }
}
