using System.IO;
using System.Net;
using Lilybot.Commute.Application;
using RestSharp;

namespace Lilybot.Commute.Infrastructure
{
    public class SlackMessageSender : ISlackMessageSender
    {
        public HttpStatusCode SendToSlack(string message, TextWriter log)
        {
            log.WriteLine($"Sending message '{message}' to Slack...");

            var client = new RestClient("https://hooks.slack.com/services/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");
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new { text = message });
            var result = client.Execute(request).StatusCode;
            log.WriteLine(result == HttpStatusCode.OK ? "Message sent successfully." : $"Sending message failed, status code: {result}.");
            return result;
        }
    }
}
