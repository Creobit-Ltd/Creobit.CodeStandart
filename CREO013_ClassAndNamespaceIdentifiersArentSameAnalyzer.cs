using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CREO013_ClassAndNamespaceIdentifiersArentSame
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO013_ClassAndNamespaceIdentifiersArentSameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO013";

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

        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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
                        Rule, type.Locations[0], new Location[1]{ parentNamespace.Locations[0]}, type.Name));
            }
        }
    }
}
