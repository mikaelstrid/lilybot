using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Lilybot.Commute.CommuteHomeNotifierJob
{
    public interface ISlackMessageSender
    {
        IRestResponse SendToSlack(string message);
    }

    public class SlackMessageSender : ISlackMessageSender
    {
        public IRestResponse SendToSlack(string message)
        {
            var client = new RestClient("https://hooks.slack.com/services/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");
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(new { text = message });
            return client.Execute(request);
        }
    }
}
