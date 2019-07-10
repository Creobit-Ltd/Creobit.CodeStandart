using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CREO015_ClassesInDifferentFiles
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO015_ClassesInDifferentFilesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO015";

        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle), 
            Resources.ResourceManager, 
            typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormat), 
            Resources.ResourceManager, 
            typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));

        private const string Category = "Using";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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
                context.ReportDiagnostic(Diagnostic.Create(Rule, root.GetLocation()));
            }
        }
    }
}
