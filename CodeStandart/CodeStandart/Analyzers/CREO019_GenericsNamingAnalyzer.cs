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
    public class CREO019_GenericsNamingAnalyzer
        : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO019";

        private const string Category = "Naming";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Warning);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.TypeParameterList);

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var genericTypesContainer = context.Node as TypeParameterListSyntax;

            var genericTypes = genericTypesContainer.Parameters;

            if (genericTypes.Count == 0) return;

            switch (genericTypes.Count)
            {
                case 0:
                    return;

                case 1:
                    {
                        if (genericTypes.First().Identifier.ValueText != "T")
                        {
                            context.ReportDiagnostic(Diagnostic.Create(_rule, genericTypesContainer.GetLocation(), genericTypesContainer));
                        }

                        break;
                    }

                default:
                    {
                        foreach (var type in genericTypes)
                        {
                            if (type.Identifier.ValueText != string.Empty && type.Identifier.ValueText[0] != 'T')
                            {
                                context.ReportDiagnostic(Diagnostic.Create(_rule, genericTypesContainer.GetLocation(), genericTypesContainer));

                                break;
                            }    
                        }

                        break;
                    }
            }
        }
    }
}
