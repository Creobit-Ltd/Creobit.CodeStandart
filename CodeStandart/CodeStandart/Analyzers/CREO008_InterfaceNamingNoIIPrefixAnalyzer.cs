using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Creo008_InterfaceNamingNoIIPrefixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO008";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Error);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol as INamedTypeSymbol;

            if (symbol.TypeKind != TypeKind.Interface) return;

            var name = symbol.ToString().Split('.').Last();

            if (name.Length > 0 && (name[1] == 'I' & name[0] == 'I'))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, symbol.Locations[0], name));
            }
        }
    }
}
