using Microsoft.Azure.WebJobs;

namespace Lilybot.Shopping.UpdateItemCountJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        static void Main()
        {
            var host = new JobHost();
            host.Call(typeof(Functions).GetMethod("UpdateItemCounts"));
        }
    }
}
