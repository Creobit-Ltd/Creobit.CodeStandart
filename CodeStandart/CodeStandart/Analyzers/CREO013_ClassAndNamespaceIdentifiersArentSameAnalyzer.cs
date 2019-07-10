using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO013_ClassAndNamespaceIdentifiersArentSameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO013";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var type = context.Symbol as INamedTypeSymbol;

            var parentNamespace = type.ContainingNamespace;

            if (parentNamespace is null)
                return;

            if (type.Name == parentNamespace.Name)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        _rule, type.Locations[0], new Location[1]{ parentNamespace.Locations[0]}, type.Name));
            }
        }
    }
}
