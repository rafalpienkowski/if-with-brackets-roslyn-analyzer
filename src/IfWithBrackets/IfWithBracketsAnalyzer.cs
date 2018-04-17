using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IfWithBrackets
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfWithBracketsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IfWithBrackets";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Style";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, ImmutableArray.Create(SyntaxKind.IfStatement));
        }
        
        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var ifStatement = node as IfStatementSyntax;
            var statement = ifStatement?.Statement;
            if (!(statement is ExpressionStatementSyntax))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, statement.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
