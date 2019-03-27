using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Newtonsoft.Json;
using NUnit.Framework;
using CodeAnalyzer.Test;
using CodeAnalyzer.Test.Helpers;
using CodeAnalyzer.Tests.Helpers;

namespace CodeAnalyzer.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class DisallowedProjectReferenceAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void NoDisallowedReferencesConfig_ShouldThrowDiagnosticErrorAboutMissingConfigFile()
        {
            var projectOptions = new ProjectOptions()
            {
                MetadataReferences = new MetadataReference[]
                    {MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),},
                AdditionalDocuments = new AdditionalDocuments[]
                    {new AdditionalDocuments("disallowed-references.yml", 
                        content: @"- Newtonsoft.Json"),}
            };

            var test = @"
            using System;
            using Newtonsoft.Json;

            namespace ConsoleApplication1
            {
                class CategoryDefinedTest
                {   
                    var x = JsonConvert.False;
                }
            }";

            var expected = new DiagnosticResult
            {
                Id = "DisallowedReference",
                Message = "The project '{0}' has a reference to 'Newtonsoft.Json', this is not allowed.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 17, 33)
                    }
            };

            VerifyCSharpDiagnostic(test, projectOptions, expected);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DisallowedProjectReferenceAnalyzer();
        }
    }
}