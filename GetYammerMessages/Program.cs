using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Json.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetYammerMessages
{
    class Program
    {
        static int totalMessagecount = 0;
        static int maxMessagecount = 100; //change this number to get max messages. if you want get all then remove th if logic in the function.
        static int iteractionCount = 0;
        static string groupId = "<Yammer Group Id>";
        static string authToken = "<Yammer Authorization token>";
        static void Main(string[] args)
        {
            
            FindNext20Messages(string.Empty, groupId);
            Console.WriteLine("Total number of messages"+ totalMessagecount);
            Console.ReadLine();
        }

        private static void FindNext20Messages(string olderThan, string groupId)
        {
            iteractionCount++;
            Console.WriteLine("Starting Iteration - " + iteractionCount);
            string url = "https://www.yammer.com/api/v1/messages/in_group/"+ groupId + ".json";
            if (olderThan != string.Empty)
            {
                url += "?older_than"+ olderThan;
            }

            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "Bearer "+ authToken);
            IRestResponse response = client.Execute(request);

            dynamic results = JsonConvert.DeserializeObject(response.Content);
            JArray messageArray = (JArray)results.messages;
            if (messageArray.Count > 0)
                totalMessagecount+= messageArray.Count;
            File.AppendAllText("results.txt", messageArray.ToString());
            if (Convert.ToBoolean(results.meta.older_available) && (totalMessagecount < maxMessagecount))
            {
                FindNext20Messages((string)results.messages[messageArray.Count - 1].id,groupId);
            }
        }
    }
}
