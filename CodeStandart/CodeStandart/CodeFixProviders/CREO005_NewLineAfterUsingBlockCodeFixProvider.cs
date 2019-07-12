using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeStandart
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CREO003_NewLineAfterAttributeCodeFixProvider)), Shared]
    public class NewLineAfterUsingBlockCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Вставить пустую строку после блока using";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(CREO005_NewLineAfterUsingBlockAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) 
                as CompilationUnitSyntax;

            var usings = root.Usings;

            var diagnostic = context.Diagnostics.First();
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => AddNewLineAfterUsingBlock(context.Document, usings.Last(), c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> AddNewLineAfterUsingBlock(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
        {
            var newUsingDirective = usingDirective
                .WithTrailingTrivia(
                    usingDirective.GetTrailingTrivia().Add(SyntaxFactory.EndOfLine(Environment.NewLine)));

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceNode(usingDirective, newUsingDirective);

            return document.WithSyntaxRoot(root);
        }

    }

}