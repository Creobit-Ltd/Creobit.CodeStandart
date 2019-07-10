using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeStyle;

namespace CREO003_NewLineAfterAttribute
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO003_NewLineAfterAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO003";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.CREO003_AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.CREO003_AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.CREO003_AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";
        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        
        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AttributeList);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeListSyntax)context.Node;

            var closingBracket = attribute.GetLastToken();

            //8539 == SyntaxKind.EndOfLineTrivia
            if (closingBracket.TrailingTrivia.Any() && closingBracket.TrailingTrivia.First().RawKind == 8539)
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }        
    }
}
