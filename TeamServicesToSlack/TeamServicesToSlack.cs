using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using TeamServicesToSlack.Models;
using TeamServicesToSlack.Services;
using TeamServicesToSlack.Models.VisualStudioServices;

namespace TeamServicesToSlack
{
    public static partial class TeamServicesToSlack
    {
        [FunctionName("TeamServicesToSlack")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            bool simple = !(req.Headers["format"] == "extended");
            var channel = req.Headers["channel"];
            if (!string.IsNullOrWhiteSpace(channel))
            {
                if (!channel.ToString().StartsWith("#"))
                    channel = "#" + channel;
            }
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var request = JsonConvert.DeserializeObject<WebhookRequest>(requestBody);
            //var message = data?.message?.text;
            var detailedMessage = request.DetailedMessage.text;
            // important - get this data
            var resourceLink = request.Resource.url;
            var changesData = await GetVstsResource($"{resourceLink}/changes");
            var changes = JsonConvert.DeserializeObject<Changes>(changesData);
            var changesCount = changes.count;
            var changesText = "";
            foreach (var change in changes.value)
            {
                changesText +=
                    $"<{change.displayUri}|{change.message}> by *{change.Author.displayName}*\n";
                if (!simple)
                    changesText += $"{change.message}\n\n";
            }
            var resource = await GetVstsResource(resourceLink);
            var resourceData = JsonConvert.DeserializeObject<ResourceDetails>(resource);
            var projectName = resourceData.Project.Name;
            var buildLink = resourceData._Links.Web.Href;
            var sourceBranch = resourceData.SourceBranch;
            var sourceVersion = resourceData.SourceVersion;
            var repository = resourceData.Repository.Id;
            var definitionName = request.Resource.definition.name;
            var gitUrl = $"{repository.Replace(".git", "")}/commit/{sourceVersion}";

            var timeline = await GetVstsResource(resourceData._Links.Timeline.Href);
            var timeLineData = JsonConvert.DeserializeObject<Timeline>(timeline);
            var failingTask = timeLineData.Records.FirstOrDefault(x => x.Result == "failed");

            var slackService = new SlackService(log, KeyManager.GetSecret("SlackWebhookUrl"));
            var model = new SlackMessageModel
            {
                username = "The VSTS detective",
                icon_emoji = ":vsts:",
                text = $"*{projectName}/{definitionName} - {failingTask.Name} failed*",
                channel = channel,
                attachments = new List<SlackMessageModel.SlackAttachment>()
                {
                    new SlackMessageModel.SlackAttachment
                    {
                        color = "#ff0000",
                        pretext = $"Repository: {repository}\nBranch: {sourceBranch}\nCommit: {sourceVersion}",
                        title = $"{changesCount} Change(s) in the build: ",
                        text = changesText,
                        actions = new[]
                        {
                            new SlackMessageModel.SlackAction
                            {
                                type = "button",
                                text = ":octocat: Git Repo Url",
                                url = repository
                            },
                            new SlackMessageModel.SlackAction
                            {
                                type = "button",
                                text = ":octocat: Git Commit Url",
                                url = gitUrl
                            },
                            new SlackMessageModel.SlackAction
                            {
                                type = "button",
                                text = ":vsts: VSTS Url",
                                url = buildLink
                            }
                        }
                    }
                }
            };
            if (!simple)
            {
                model.attachments.Add(new SlackMessageModel.SlackAttachment
                {
                    color = "#ff0000",
                    title = "Build message",
                    text = $"{detailedMessage}"
                });
            }


            slackService.PostToSlack(model);
            return new OkObjectResult("");
        }

        public static async Task<string> GetVstsResource(string url)
        {
            try
            {
                var personalaccesstoken = KeyManager.GetSecret("VSTSPersonalAccessToken");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalaccesstoken))));

                    using (HttpResponseMessage response = client.GetAsync(
                        url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return responseBody;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
