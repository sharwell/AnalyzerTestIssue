using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Tests.Helpers
{
    /// <summary>
    /// Options to use for the project.
    /// </summary>
    public class ProjectOptions
    {
        /// <summary>
        /// A list of <see cref="MetadataReference"/>s to add to a project.
        /// </summary>
        public MetadataReference[] MetadataReferences { get; set; }
        /// <summary>
        /// A list of <see cref="AdditionalDocuments"/> to add to the project.
        /// </summary>
        public AdditionalDocuments[] AdditionalDocuments { get; set; }
    }
}