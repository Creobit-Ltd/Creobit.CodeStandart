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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndividedLocalDeclarationCodeFixProvider)), Shared]
    public class UndividedLocalDeclarationCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Разделить все объявления переменных";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(UndividedLocalDeclarationAnalyzer.DiagnosticId); }
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => DivideDeclarationsAsync(context.Document, declaration, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> DivideDeclarationsAsync(Document document,
            VariableDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            var newDeclarations = new SyntaxList<VariableDeclarationSyntax>();

            foreach (var variable in declaration.Variables)
            {
                var newDeclaration = SyntaxFactory.VariableDeclaration(declaration.Type);

                var firstToken = variable.GetFirstToken();

                var newVariable = variable.ReplaceToken(
                    firstToken,
                    firstToken.WithoutTrivia());

                newDeclaration = newDeclaration.AddVariables(newVariable);

                newDeclarations = newDeclarations.Add(newDeclaration);
            }

            var originalLocalDeclaration = declaration.Parent as LocalDeclarationStatementSyntax;

            var modifiers = originalLocalDeclaration.Modifiers;

            var leadingTrivia = originalLocalDeclaration.GetFirstToken().LeadingTrivia;

            var newLocalDeclarations = new SyntaxList<LocalDeclarationStatementSyntax>();

            foreach (var variableDeclaration in newDeclarations)
            {
                var newField = SyntaxFactory.LocalDeclarationStatement(
                    modifiers,
                    variableDeclaration);

                newLocalDeclarations = newLocalDeclarations.Add(
                    newField
                        .WithLeadingTrivia(
                            newDeclarations.IndexOf(variableDeclaration) > 0 ? 
                                new SyntaxTriviaList() {leadingTrivia.Last()} : leadingTrivia)
                        .WithTrailingTrivia(SyntaxFactory.EndOfLine((Environment.NewLine))));
            }

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(originalLocalDeclaration, newLocalDeclarations);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
