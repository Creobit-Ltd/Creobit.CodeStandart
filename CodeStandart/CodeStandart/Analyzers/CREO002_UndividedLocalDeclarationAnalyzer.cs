using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStandart
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UndividedLocalDeclarationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CREO002";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.CREO002_AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.CREO002_AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.CREO002_AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";
        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        { 
            var declaration = (VariableDeclarationSyntax)context.Node;

            if (declaration.Parent.GetType() != typeof(LocalDeclarationStatementSyntax))
            {
                return;
            }

            var declarationChild = declaration.ChildNodes();

            var declarators = new List<VariableDeclaratorSyntax>();

            foreach (var node in declarationChild)
            {
                if (node is VariableDeclaratorSyntax)
                {
                    declarators.Add(node as VariableDeclaratorSyntax);
                }
            }

            if (declarators.Count <= 1)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(_rule, context.Node.GetLocation()));
        }
    }
}
