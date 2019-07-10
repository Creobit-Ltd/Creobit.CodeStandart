using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CREO016_EachEnumFromNewStringAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO016";

        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.CREO016_AnalyzerTitle),
            Resources.ResourceManager, 
            typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.CREO016_AnalyzerMessageFormat), 
            Resources.ResourceManager,
            typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.CREO016_AnalyzerDescription), 
            Resources.ResourceManager,
            typeof(Resources));

        private const string Category = "Using";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.EnumMemberDeclaration);

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var enumMember = context.Node;

            var parent = enumMember.Parent as EnumDeclarationSyntax;

            var commaTokens = parent
                .ChildTokens()
                    .Where(t => t.Kind() == SyntaxKind.CommaToken);

            if (commaTokens.Any(
                comma => !comma.TrailingTrivia.Any(
                    trivia => trivia.Kind() == SyntaxKind.EndOfLineTrivia)))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, enumMember.GetLocation(), parent.Identifier));
            }
        }
    }
}
