using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace CodeStandart
{
    static internal class AnalyzerUtility
    {
        public static DiagnosticDescriptor CreateDiagnosticDescriptor(string id, string category, DiagnosticSeverity severity)
        {
            var title = GetLocalizedString(id, LocalizedStringKind.Title);
            var message = GetLocalizedString(id, LocalizedStringKind.FormattableMessage);
            var description = GetLocalizedString(id, LocalizedStringKind.Description);

            return new DiagnosticDescriptor(id, title, message, category, severity, true, description);
        }

        public static bool CheckIfNodeFirstInBlock(SyntaxNode node)
        {
            var parent = node.Parent;

            return parent.ChildNodes().ElementAt(0) == node;
        }

        public static bool CheckIfHaveEmptyStringBefore(SyntaxNode node)
        {
            var EndOfLineTrivias = node.GetLeadingTrivia()
                .Where(trivia => trivia.Kind() == SyntaxKind.EndOfLineTrivia);

            return EndOfLineTrivias.Count() > 0;
        }

        public static LocalizableResourceString GetLocalizedString(string id, LocalizedStringKind kind)
        {
            var name = "";

            switch (kind)
            {
                case LocalizedStringKind.Title:
                    name = id + "_AnalyzerTitle";
                    break;
                case LocalizedStringKind.FormattableMessage:
                    name = id + "_AnalyzerMessageFormat";
                    break;
                case LocalizedStringKind.Description:
                    name = id + "_AnalyzerDescription";
                    break;
            }

            return new LocalizableResourceString(name, Resources.ResourceManager, typeof(Resources));
        }

    }

    internal enum LocalizedStringKind
    {
        Title,
        FormattableMessage,
        Description
    }
}
