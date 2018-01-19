using System;

namespace TeamServicesToSlack.Models.VisualStudioServices
{
    public class Resource
    {
        public Uri Uri { get; set; }
        public int id { get; set; }
        public string buildnumber { get; set; }
        public string url { get; set; }
        public string status { get; set; }
        public string sourceGetVersion { get; set; }
        public BuildDefinition definition { get; set; }
    }
}
