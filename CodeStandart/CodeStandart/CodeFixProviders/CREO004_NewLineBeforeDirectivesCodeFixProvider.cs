using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.CodeStyle;
using Microsoft.CodeAnalysis.CodeStyle;
using Microsoft.CodeAnalysis.Formatting;

namespace CodeStandart.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = "CREO004_NewLineBeforeDirectives"), Shared]
    public class NewLineBeforeDirectivesCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(CREO004_NewLineBeforeDirectivesAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private const string Title = "Вставить пустую строку перед оператором управления";

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var statement = root.FindToken(diagnosticSpan.Start)
                .Parent
                    .AncestorsAndSelf()
                        .OfType<StatementSyntax>()
                        .First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => AddNewLineBefore(context.Document, statement, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> AddNewLineBefore(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            var leadingTriviaWithEOL = statement.GetLeadingTrivia().Insert(0, SyntaxFactory.EndOfLine(Environment.NewLine));

            var newStatement = statement.WithLeadingTrivia(leadingTriviaWithEOL);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceNode(statement, newStatement);

            return document.WithSyntaxRoot(root);
        }
    }
}
