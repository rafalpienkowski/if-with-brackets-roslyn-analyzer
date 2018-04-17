using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using System;
using System.IO;
using TestHelper;
using IfWithBrackets;

namespace IfWithBrackets.Test
{
    public class IfWithBracketsUnitTests : CodeFixVerifier
    {
 
        //No diagnostics expected to show up
        [Fact]
        public void AnalyzeIfStatementTest_NotBeAppliedInEmptyFile()
        {
            var test = @"";
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void AnalyzeIfStatementTest_NotBeAppliedInFileWithoutIfStatement()
        {
            var testFileContent = ReadFile("Resources\\CodeFileWithoutIfStatement.txt");
            VerifyCSharpDiagnostic(testFileContent);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public void AnalyzeIfStatementTest_DiagnosticShouldBeShown()
        {
            var testFileContent = ReadFile("Resources\\CodeFile.txt");
            var fixedTestFileContent = ReadFile("Resources\\FixedCodeFile.txt");

            var expected = new DiagnosticResult
            {
                Id = "IfWithBrackets",
                Message = "The statement under if doesn't contain brackets",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 16, 17)
                        }
            };

            VerifyCSharpDiagnostic(testFileContent, expected);
            VerifyCSharpFix(testFileContent, fixedTestFileContent);
        }

        private static string ReadFile(string path)
        {
            string content;
            using (var streamReader = new StreamReader(path))
            {
                content = streamReader.ReadToEnd();
            }
            return content;
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IfWithBracketsCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new IfWithBracketsAnalyzer();
        }
    }
}
