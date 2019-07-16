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
using Microsoft.CodeAnalysis.Rename;
using CodeStandart.Analyzers;

namespace CodeStandart.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = "CREO006_EnumElementsInPascalCase"), Shared]
    public class EnumElementsInPascalCaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(Creo006_EnumElementsInPascalCaseAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private const string Title = "Сделать идентификатор PascalCase";
        
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context
                .Document
                    .GetSyntaxRootAsync(context.CancellationToken)
                        .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var member = root.FindToken(diagnosticSpan.Start)
                .Parent
                    .AncestorsAndSelf()
                        .OfType<EnumMemberDeclarationSyntax>()
                        .First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedSolution: c => MakeMemberPascalCase(context.Document, member, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Solution> MakeMemberPascalCase(
            Document document,
            EnumMemberDeclarationSyntax member,
            CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
            ;

            var originalSolution = document.Project.Solution;

            var options = originalSolution.Options;

            var name = member.Identifier.ToString();
            var newNameInChars = name
                .TrimStart('_')
                .ToCharArray();

            newNameInChars[0] = char.ToUpper(newNameInChars[0]);

            var newName = new string(newNameInChars);    

            originalSolution = await Renamer.RenameSymbolAsync(
                originalSolution,
                semanticModel.GetDeclaredSymbol(member),
                newName,
                options,
                cancellationToken)
                .ConfigureAwait(false);
            
            return originalSolution;
        }
    }
}
