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
    public class CREO011_EventArgsNotActionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO011";

        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.CREO011_AnalyzerTitle), 
            Resources.ResourceManager, 
            typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.CREO011_AnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.CREO011_AnalyzerDescription), 
            Resources.ResourceManager, 
            typeof(Resources));

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EventFieldDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = context.Node as EventFieldDeclarationSyntax;

            var typeIdentifierToken = eventDeclaration.DescendantTokens().FirstOrDefault(
                n => n.Kind() == SyntaxKind.IdentifierToken);

            if (typeIdentifierToken == default) return;

            if (typeIdentifierToken.Text == "Action")
            {
                var identifier = eventDeclaration
                    .Declaration.Variables
                        .FirstOrDefault().Identifier;

                context.ReportDiagnostic(Diagnostic.Create(_rule, eventDeclaration.GetLocation(), identifier.Text?? ""));
            }
        }
    }
}
