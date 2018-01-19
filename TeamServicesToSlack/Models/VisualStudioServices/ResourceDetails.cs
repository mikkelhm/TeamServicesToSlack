namespace TeamServicesToSlack.Models.VisualStudioServices
{
    public class ResourceDetails
    {
        public Links _Links { get; set; }
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string Url { get; set; }
        public Project Project { get; set; }
        public string SourceBranch { get; set; }
        public string SourceVersion { get; set; }
        public Repository Repository { get; set; }
    }
}
