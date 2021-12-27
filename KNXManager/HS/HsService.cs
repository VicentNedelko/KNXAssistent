using DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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


        public async Task<string> GetLoginActionsAsync()
        {
            var basePath = String.Concat("https://", HostIp, "/hslist");
            Uri uri = new(basePath);
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient httpClient = new(clientHandler);
            httpClient.BaseAddress = uri;
            Dictionary<string, string> values = new();
            values.Add("lst", "login");
            values.Add("user", Login);
            values.Add("pw", Pass);
            var content = new FormUrlEncodedContent(values);
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public string[] GetParsedLoginActions(string logData)
        {
            string[] parsedLogins = logData.Split(new string[] { "<tr>", "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            return parsedLogins;
        }

        public LogInfo[] GetLogValues(string[] parsedLogins)
        {
            List<LogInfo> logInfoList = new();
            foreach (var l in parsedLogins)
            {
                string[] logFields = l.Split(new string[] { "<td>", "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                if (DateTime.TryParseExact(logFields[0], "dd.MM.yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    LogInfo logInfo = new()
                    {
                        TimeStamp = dt,
                        Action = logFields[1],
                        UserName = logFields[2],
                        IP = logFields[3],
                        Auth = logFields[4],
                        Loc = logFields[5],
                        Client = logFields[6],
                        ListType = logFields[7],
                    };
                    logInfoList.Add(logInfo);
                }
            }
            return logInfoList.ToArray();
        }
    }
}
