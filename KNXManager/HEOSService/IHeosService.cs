using DreamNucleus.Heos.Commands.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.HEOSService
{
    public interface IHeosService
    {
        public Task FindPlayersAsync();
        public event Action OnDenonCheck;
        public GetPlayerResponse[] PlayersList { get; set; }
        public MacIpPair[] Denons { get; set; }

        public class MacIpPair
        {
            public string MacAddress;
            public string IpAddress;
            public string Status;
        }
    }
}
