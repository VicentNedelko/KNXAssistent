using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.HS
{
    public class HsService : IHsService
    {
        public string HostIp { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }


        public async Task<string> GetActionsAsync(string type)
        {
            var basePath = String.Concat("https://",HostIp, "/hslist");
            Uri uri = new(basePath);
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient httpClient = new(clientHandler);
            httpClient.BaseAddress = uri;
            Dictionary<string, string> values = new();
            values.Add("lst", type);
            values.Add("user", UserName);
            values.Add("pw", Pass);
            var content = new FormUrlEncodedContent(values);
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
