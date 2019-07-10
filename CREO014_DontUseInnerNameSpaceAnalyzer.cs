using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CREO014_DontUseInnerNameSpace
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO014_DontUseInnerNameSpaceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO014";

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

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor
            (DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Namespace);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namespaceSymbol = context.Symbol as INamespaceSymbol;

            if (!namespaceSymbol.IsGlobalNamespace &
                !namespaceSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, namespaceSymbol.Locations.First(), namespaceSymbol.Name));
            }
        }
    }
}