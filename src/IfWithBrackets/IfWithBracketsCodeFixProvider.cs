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

namespace IfWithBrackets
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfWithBracketsCodeFixProvider)), Shared]
    public class IfWithBracketsCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add brackets to the if statement";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(IfWithBracketsAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var node = root.FindNode(context.Span);

            var expression = node as ExpressionStatementSyntax;
            if (expression == null)
            {
                return;
            }

            var action = CodeAction.Create(title, token =>
            {
                var block = SyntaxFactory.Block(expression);
                var newRoot = root.ReplaceNode(expression,block);
                var newDocument = context.Document.WithSyntaxRoot(newRoot);
                return Task.FromResult(newDocument);
            });

            context.RegisterCodeFix(action, context.Diagnostics.First());

        }

    }
}
