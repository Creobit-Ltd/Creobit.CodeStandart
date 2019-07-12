using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeStandart.Extensions
{
    public static class EnumDeclarationSyntaxExtensions
    {
        public static EnumMemberDeclarationSyntax TryGetNonPascalMember(this EnumDeclarationSyntax _enum)
        {
            return _enum.Members.FirstOrDefault(
                member =>
                    char.IsLower(member.GetFirstToken().ToString()[0]) ||
                    member.GetFirstToken().ToString()[0] == '_');
        }

        public static bool IsPascal(this EnumMemberDeclarationSyntax member)
        {
            return !char.IsLower(member.GetFirstToken().ToString()[0]) &&
                !(member.GetFirstToken().ToString()[0] == '_');
        }
    }
}
