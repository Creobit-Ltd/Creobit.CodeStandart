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
using CodeStandart.Analyzers;

namespace CodeStandart.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndividedFieldDeclarationCodeFixProvider)), Shared]
    public class UndividedFieldDeclarationCodeFixProvider : CodeFixProvider
    {
        private const string Title = "��������� ����";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(UndividedFieldDeclarationAnalyzer.DiagnosticId); }
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

            var originalField = declaration.Parent as FieldDeclarationSyntax;

            var modifiers = originalField.Modifiers;

            var leadingTrivia = originalField.GetFirstToken().LeadingTrivia;

            var newFields = new SyntaxList<FieldDeclarationSyntax>();

            foreach (var variableDeclaration in newDeclarations)
            {
                var newField = SyntaxFactory.FieldDeclaration(
                    new SyntaxList<AttributeListSyntax>(),
                    modifiers,
                    variableDeclaration)
                    .WithTrailingTrivia(SyntaxFactory.EndOfLine(Environment.NewLine));

                newFields = newFields.Add(
                    newField
                        .WithLeadingTrivia(
                            newDeclarations.IndexOf(variableDeclaration) > 0 ?
                                new SyntaxTriviaList()
                                {
                                    SyntaxFactory.EndOfLine(Environment.NewLine), leadingTrivia.Last()
                                } 
                                : leadingTrivia)
                        .WithTrailingTrivia(
                            newDeclarations.IndexOf(variableDeclaration) !=  newDeclarations.IndexOf(newDeclarations.Last()) ? 
                                newField.GetTrailingTrivia()
                                    .Add(SyntaxFactory.EndOfLine(Environment.NewLine)) : newField.GetTrailingTrivia()));            
            }

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceNode(originalField, newFields);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
