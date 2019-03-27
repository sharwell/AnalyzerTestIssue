namespace CodeAnalyzer.Tests.Helpers
{
    /// <summary>
    /// A structure to hold the additional documents.
    /// </summary>
    public class AdditionalDocuments
    {
        public AdditionalDocuments(string name, string content)
        {
            this.Name = name;
            this.Content = content;
        }
        /// <summary>
        /// The name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The content of the additional document.
        /// </summary>
        public string Content { get; set; }
    }
}