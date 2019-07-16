using System.Linq;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeStandart.Analyzers;

namespace CodeStandart.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = "CREO006_EnumElementsInPascalCase"), Shared]
    public class TrueFalseComparingCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Убрать явное сравнение с true или false";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(Creo007_TrueFalseComparingAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context
                .Document
                    .GetSyntaxRootAsync(context.CancellationToken)
                        .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var expression = root.FindToken(diagnosticSpan.Start)
                .Parent
                    .AncestorsAndSelf()
                        .OfType<BinaryExpressionSyntax>()
                        .First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => RemoveExplicitComparing(context.Document, expression, c),
                    equivalenceKey: Title),
                diagnostic);
         }

        private async Task<Document> RemoveExplicitComparing(
            Document document,
            BinaryExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            var leftPart = expression.Left;
            var rigthPart = expression.Right;

            var root = await document.GetSyntaxRootAsync(cancellationToken);

            if (leftPart.Kind() == SyntaxKind.FalseLiteralExpression ||
                leftPart.Kind() == SyntaxKind.TrueLiteralExpression)
            {
                return document.WithSyntaxRoot(root.ReplaceNode(expression, rigthPart));
            }
            else if (rigthPart.Kind() == SyntaxKind.FalseLiteralExpression ||
                rigthPart.Kind() == SyntaxKind.TrueLiteralExpression)
            {
                return document.WithSyntaxRoot(root.ReplaceNode(expression, leftPart.WithoutTrivia()));
            }
            else
            {
                return document;
            }
        }
        
    }
}
