using System.Collections.Generic;

namespace TeamServicesToSlack.Models
{
    public class SlackMessageModel
    {
        public string username { get; set; }
        public string icon_emoji { get; set; }
        public List<SlackAttachment> attachments { get; set; }
        public string text { get; set; }
        public string channel { get; set; }

        public class SlackAttachment
        {
            public string color { get; set; }
            public string title { get; set; }
            public string text { get; set; }
            public string pretext { get; set; }
            public List<SlackField> fields { get; set; }
            public long ts { get; set; }
            public string[] mrkdwn_in { get; set; }
            public SlackAction[] actions { get; set; }
        }

        public class SlackAction
        {
            public string type { get; set; }
            public string text { get; set; }
            public string url { get; set; }
        }
        public class SlackField
        {
            public string title { get; set; }
            public string value { get; set; }
        }
    }
}
