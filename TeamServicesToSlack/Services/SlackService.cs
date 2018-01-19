using System.Collections.Specialized;
using System.Net;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using TeamServicesToSlack.Models;

namespace TeamServicesToSlack.Services
{
    public class SlackService
    {
        private readonly TraceWriter _log;
        private readonly string _slackWebhook;

        public SlackService(TraceWriter log) : this(log, "")
        {
        }

        public SlackService(TraceWriter log, string slackWebhook)
        {
            _log = log;
            _slackWebhook = slackWebhook;
        }

        public void PostToSlack(SlackMessageModel payload)
        {
            PostToSlack(_slackWebhook, payload);
        }

        public void PostToSlack(string slackWebhook, SlackMessageModel payload)
        {
            var payloadJson = JsonConvert.SerializeObject(payload);
            _log.Info($"Payload JSON {payloadJson}");
            _log.Info($"Slack Webhook URL {slackWebhook}");

            using (WebClient client = new WebClient())
            {
                NameValueCollection slackData = new NameValueCollection();
                slackData["payload"] = payloadJson;

                var response = client.UploadValues(slackWebhook, "POST", slackData);
            }
        }
    }
}
