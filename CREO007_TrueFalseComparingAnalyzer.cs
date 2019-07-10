using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Creo007_TrueFalseComparing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Creo007_TrueFalseComparingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Creo007";

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

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression);

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var literal = context.Node;

            var parent = literal.Parent;

            if (parent.Kind() != SyntaxKind.EqualsExpression && parent.Kind() != SyntaxKind.NotEqualsExpression)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                Rule,
                literal.GetLocation(),
                literal.Parent,
                literal));
        }
    }
}
