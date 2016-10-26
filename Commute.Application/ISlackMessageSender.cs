using System.IO;
using System.Net;

namespace Lilybot.Commute.Application
{
    public interface ISlackMessageSender
    {
        HttpStatusCode SendToSlack(string message, TextWriter log);
    }
}