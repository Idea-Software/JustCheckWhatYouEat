using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JustCheckWhatYouEat.Api.DataAccess
{

    public interface IBingFacade
    {
        List<string> FindImages(string query);
    }
    public class BingFacade : IBingFacade
    {
        public List<string> FindImages(string query)
        {
            HttpClient client = new HttpClient();
            var url = ConfigurationManager.AppSettings["Bing:UrlTemplate"];
            url = String.Format(url, query);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{0}", ConfigurationManager.AppSettings["Bing.AccountKey"]))));

            var result = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;

            return JObject.Parse(result)["d"]["results"].Select(r => (string)r["MediaUrl"]).ToList();
        }
    }
}