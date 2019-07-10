using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CREO010_AttributeSuffix
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO010_AttributeSuffixAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO010";

        #region localizableStrings

        private static readonly LocalizableString Title_Attribute = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitleAttribute),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString MessageFormat_Attribute = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormatAttribute),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description_Attribute = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescriptionAttribute),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Title_NonAttribute = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitleNonAttribute),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString MessageFormat_NonAttribute = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormatNonAttribute),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description_NonAttribute = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescriptionNonAttribute),
            Resources.ResourceManager,
            typeof(Resources));

        #endregion localizableStrings

        private const string Category = "Naming";

        private static DiagnosticDescriptor AttributeTypeRule = new DiagnosticDescriptor(
            DiagnosticId, Title_Attribute, MessageFormat_Attribute, Category, DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, description: Description_Attribute);

        private static DiagnosticDescriptor NonAttributeTypeRule = new DiagnosticDescriptor(
            DiagnosticId, Title_NonAttribute, MessageFormat_NonAttribute, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true, description: Description_NonAttribute);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(AttributeTypeRule, NonAttributeTypeRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var type = (ITypeSymbol)context.Symbol;
            var typeName = type.MetadataName;

            var baseTypeName = type.BaseType?.MetadataName;

            if (baseTypeName == null)
                return;

            if (baseTypeName.EndsWith("Attribute") && !typeName.EndsWith("Attribute"))
                context.ReportDiagnostic(Diagnostic.Create(AttributeTypeRule, type.Locations.First(), typeName));

            if (!baseTypeName.EndsWith("Attribute") && typeName.EndsWith("Attribute"))
                context.ReportDiagnostic(Diagnostic.Create(NonAttributeTypeRule, type.Locations.First(), typeName));
        }
    }
}
