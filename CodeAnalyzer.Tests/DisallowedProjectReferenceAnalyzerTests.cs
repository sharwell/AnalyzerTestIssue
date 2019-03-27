using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using VerifyCS = CodeAnalyzer.Tests.CSharpCodeFixVerifier<CodeAnalyzer.DisallowedProjectReferenceAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
using VerifyVB = CodeAnalyzer.Tests.VisualBasicCodeFixVerifier<CodeAnalyzer.DisallowedProjectReferenceAnalyzer, Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace CodeAnalyzer.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class DisallowedProjectReferenceAnalyzerTests
    {
        [Test]
        public async Task NoDisallowedReferencesConfig_ShouldThrowDiagnosticErrorAboutMissingConfigFile()
        {
            var source = @"
            using System;
            using Newtonsoft.Json;

            namespace ConsoleApplication1
            {
                class CategoryDefinedTest
                {   
                    var x = JsonConvert.False;
                }
            }";

            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { source },
                    AdditionalReferences = { MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location) },
                    AdditionalFiles = { ("disallowed-references.yml", "- Newtonsoft.Json") },
                    ExpectedDiagnostics =
                    {
                        VerifyCS.Diagnostic().WithArguments("TestProject", "Newtonsoft.Json"),
                        DiagnosticResult.CompilerError("CS0825").WithSpan(9, 21, 9, 24).WithMessage("The contextual keyword 'var' may only appear within a local variable declaration or in script code"),
                    },
                }
            };

            await test.RunAsync();
        }
    }
}
