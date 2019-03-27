using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace CodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DisallowedProjectReferenceAnalyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor DisallowedReference { get; } =
            new DiagnosticDescriptor(
                "DisallowedReference",
                "Disallowed Reference",
                "The project '{0}' has a reference to '{1}', this is not allowed.",
                category: "Maintainability",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DisallowedReference);

        public override void Initialize(AnalysisContext analysisContext)
        {
            analysisContext.RegisterCompilationAction(StartAction);
        }

        private void StartAction(CompilationAnalysisContext compilationAnalysisContext)
        {
            ImmutableArray<AdditionalText> additionalFiles = compilationAnalysisContext.Options.AdditionalFiles;
            AdditionalText configFile = additionalFiles.FirstOrDefault(file =>
            {
                if (file?.Path == null)
                {
                    throw new NullReferenceException($"{nameof(file)} is passed as null via AdditionalFiles.");
                }
                return Path.GetFileName(path: file.Path)
                           .Equals("disallowed-references.yml", StringComparison.InvariantCultureIgnoreCase);
            });


            HashSet<string> disallowedReferences = new HashSet<string>();
            if (configFile == null)
            {
                Console.WriteLine("disallowed-references.yml not found, make sure you have it set as an AdditionalFile.");
            }

            string hardCodedListMessage = string.Empty;
            if (configFile != null)
            {
                // Read the file line-by-line to get the terms.
                SourceText fileText = configFile.GetText(compilationAnalysisContext.CancellationToken);
                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    fileText.Write(writer);
                }

                stream.Position = 0;
                var deserializer = new Deserializer();
                using (var reader = new StreamReader(stream))
                {
                    var configFileDeserialize = deserializer.Deserialize<List<string>>(reader);


                    foreach (var assemblyName in configFileDeserialize)
                    {
                        disallowedReferences.Add(assemblyName.Trim());
                    }
                }
            }

            foreach (var disallowedReference in disallowedReferences)
            {
                Console.WriteLine($"Disallowed Assembly Name: {disallowedReference}");
            }


            // Check every named type for the invalid references.
            var compilation = compilationAnalysisContext.Compilation;

            foreach (var compilationReferencedAssemblyName in compilation.ReferencedAssemblyNames)
            {
                if (disallowedReferences.Contains(compilationReferencedAssemblyName.Name))
                {
                    compilationAnalysisContext.ReportDiagnostic(
                        Diagnostic.Create(
                            DisallowedReference,
                            null,
                            compilation.AssemblyName,
                            compilationReferencedAssemblyName.Name));
                }
            }
        }
    }
}