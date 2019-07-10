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

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO003_NewLineAfterAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO003";

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = AnalyzerUtility.CreateDiagnosticDescriptor(
            DiagnosticId, Category, DiagnosticSeverity.Warning);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);
        
        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AttributeList);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeListSyntax)context.Node;

            var closingBracket = attribute.GetLastToken();

            //8539 == SyntaxKind.EndOfLineTrivia
            if (closingBracket.TrailingTrivia.Any() &&
                closingBracket.TrailingTrivia.First().RawKind == 8539)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, context.Node.GetLocation()));
        }        
    }
}
