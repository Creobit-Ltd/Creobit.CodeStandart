using System;
using System.Collections.Generic;
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
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.CodeStyle;
using Microsoft.CodeAnalysis.CodeStyle;
using Microsoft.CodeAnalysis.Formatting;

namespace CodeStandart
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CREO003_NewLineAfterAttributeCodeFixProvider)), Shared]
    public class CREO003_NewLineAfterAttributeCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Вынести использование атрибута на отдельную строку";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CREO003_NewLineAfterAttributeAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var attribute = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AttributeListSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => AddNewLineToAttribute(context.Document, attribute, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> AddNewLineToAttribute(Document document, AttributeListSyntax attribute, CancellationToken cancellationToken)
        {
            var leadingWhitespace = attribute.HasLeadingTrivia ?
                attribute.GetLeadingTrivia().Last() 
                : SyntaxFactory.ElasticWhitespace("");

            var newAttribute = GetNodeWithTrailingEOL(attribute.WithoutTrailingTrivia());
            newAttribute = newAttribute
                .WithTrailingTrivia(newAttribute
                    .GetTrailingTrivia()
                        .Add(leadingWhitespace));

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceNode(attribute, newAttribute);

            return document.WithSyntaxRoot(root);
        }

        private CSharpSyntaxNode GetNodeWithTrailingEOL(CSharpSyntaxNode node)
        {
            return node.WithTrailingTrivia(
                node.GetTrailingTrivia().Insert(
                    0,
                    SyntaxFactory.EndOfLine(Environment.NewLine)));
        }

    }

}
