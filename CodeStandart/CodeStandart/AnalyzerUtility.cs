using Microsoft.CodeAnalysis;

namespace CodeStandart
{
    static internal class AnalyzerUtility
    {
        public static LocalizableResourceString GetLocalizedString(string id, LocalizedStringKind kind)
        {
            string name = "";

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

        public static DiagnosticDescriptor CreateDiagnosticDescriptor(string id, string category, DiagnosticSeverity severity)
        {
            var title = GetLocalizedString(id, LocalizedStringKind.Title);
            var message = GetLocalizedString(id, LocalizedStringKind.FormattableMessage);
            var description = GetLocalizedString(id, LocalizedStringKind.Description);

            return new DiagnosticDescriptor(id, title, message, category, severity, true, description);
        }
    }

    internal enum LocalizedStringKind
    {
        Title,
        FormattableMessage,
        Description
    }
}
