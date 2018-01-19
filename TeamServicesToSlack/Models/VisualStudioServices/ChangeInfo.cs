using System;

namespace TeamServicesToSlack.Models.VisualStudioServices
{
    public class ChangeInfo
    {
        public string id { get; set; }
        public string message { get; set; }
        public string location { get; set; }
        public string displayUri { get; set; }
        public Author Author { get; set; }
        public DateTimeOffset timestamp { get; set; }
    }
}
