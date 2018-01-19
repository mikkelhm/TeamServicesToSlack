# TeamServicesToSlack
A Azure function, responsible to recieve informations from TeamServices, format them, and report nicely into Slack


The idea is that you are able to setup a webhook url to post to, when a build definition fails in VSTS. This webhook should be this code, hosted 
in an Azure function. update the values from the local.settings.json, and add them as application settings on your Azure function.

Will format and post to a Slack channel, with various links
