using DreamNucleus.Heos;
using DreamNucleus.Heos.Commands.Player;
using DreamNucleus.Heos.Infrastructure.Heos;
using DreamNucleus.Heos.Infrastructure.Telnet;
using KNXManager.MessageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static KNXManager.HEOSService.IHeosService;

namespace KNXManager.HEOSService
{
    public class HeosService : IHeosService
    {
        public GetPlayerResponse[] PlayersList { get; set; }
        private readonly IMessService _messService;
        public MacIpPair[] Denons { get; set; }
        public bool IsInProcess { get; set; }

        public HeosService(IMessService messService)
        {
            _messService = messService;
            IsInProcess = false;
        }

        internal static Task<PingReply> PingNodesByIPAsync(string ip)
        {
            Ping ping = new();
            return Task.Run(() => ping.Send(IPAddress.Parse(ip)));
        }

        internal static byte GetSubnetworkValue()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            var myIP = localIPs.FirstOrDefault(ip => ip.AddressFamily.ToString() == "InterNetwork");
            var byteCollection = myIP.GetAddressBytes() ?? null;
            return (byteCollection is not null) ? byteCollection[2] : (byte)0;
        }

        public async Task FindPlayersAsync()
        {
            List<Task<PingReply>> pingReplies = new();
            List<MacIpPair> mip = new();
            List<MacIpPair> denons = new();
            List<GetPlayerResponse> getPlayers = new();

            for (int i = 0; i <= 255; i++)
            {
                string address = string.Concat("192.168.", GetSubnetworkValue(), ".", i);
                pingReplies.Add(PingNodesByIPAsync(address));
            }
            Task.WaitAll(pingReplies.ToArray());

            System.Diagnostics.Process pProcess = new();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                mip.Add(new MacIpPair()
                {
                    MacAddress = m.Groups["mac"].Value,
                    IpAddress = m.Groups["ip"].Value,
                });
            }

            foreach (var machine in mip)
            {
                var mac = machine.MacAddress.Split(new char[] { '-' });
                if ((mac[0] == "00" & mac[1] == "05" & mac[2] == "cd")
                    || (mac[0] == "00" & mac[1] == "06" & mac[2] == "78")
                    || (mac[0] == "8c" & mac[1] == "a9" & mac[2] == "6f"))
                {
                    denons.Add(machine);
                }
            }

            for(int i = 0; i < denons.Count; i++)
            {
                Ping ping = new();
                IPAddress ip = IPAddress.Parse(denons[i].IpAddress);
                PingReply reply = ping.Send(ip);
                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        var telnetClient = new SimpleTelnetClient(denons[i].IpAddress);
                        var heosClient = new HeosClient(telnetClient, CancellationToken.None);
                        var commandProcessor = new CommandProcessor(heosClient);
                        var plyrs = await commandProcessor.Execute(new GetPlayersCommand());
                        foreach (var p in plyrs.Payload)
                        {
                            getPlayers.Add(p);
                            _messService.AddInfoMessage($"Denon added successfully - {p.Pid} & {p.Ip}");
                            denons[i].Status = "Added successfully";
                        }
                    }
                    catch
                    {
                        _messService.AddWarningMessage($"Denon can't be added - {denons[i].IpAddress} & Ping - {reply.Status}. TelNet Client failed.");
                        denons[i].Status = "TelNet Client failed";
                    }

                }
                else
                {
                    _messService.AddDangerMessage($"{denons[i].IpAddress} --> Status : {reply.Status}");
                }
            }
            PlayersList = getPlayers.ToArray();
            Denons = denons.ToArray();
            IsInProcess = false;
        }
    }
}
