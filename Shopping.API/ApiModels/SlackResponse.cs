using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Lilybot.Shopping.API.ApiModels
{
    public class SlackResponse
    {
        public SlackResponse(string text = "")
        {
            this.text = text;
        }

        public string response_type { get; set; } = "ephemeral";
        public string text { get; set; }
        public List<SlackResponseAttachment> attachments { get; set; } = new List<SlackResponseAttachment>();
    }

    public class SlackResponseAttachment
    {
        public string text { get; set; } = "";
    }
}