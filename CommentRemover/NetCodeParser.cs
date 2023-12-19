using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommentRemover
{
    public class NetCodeParser
    {
        public static async Task<string> RemoveCommentsAsync(string? code)
        {
            return await Task.Run(() => RemoveComments(code));
        }

        public static string RemoveComments(string? code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;

            code = code.Replace("; ", $";{Environment.NewLine}");
            code = code.Replace("/// </summary> ", $"/// </summary>{Environment.NewLine}");
            code = code.Replace("</param> ", $"</param>{Environment.NewLine}");
            var root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();
            var tree = root.SyntaxTree;
            if (root.Members.Count == 0)
            {
                return string.Empty;
            }

            MemberDeclarationSyntax firstMember = root.Members[0];

            var commentTrivia = from t in tree.GetRoot().DescendantTrivia()
                                where t.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                                      t.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                                      t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                                select t;

            var newRoot = tree.GetRoot().ReplaceTrivia(commentTrivia, (_, _) => new SyntaxTrivia());

            newRoot = newRoot.NormalizeWhitespace(elasticTrivia: true);

            return newRoot.ToFullString().Replace("\r\n", "");
        }

    }
}