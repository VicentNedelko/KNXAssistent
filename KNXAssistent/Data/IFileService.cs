using KNXManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNXAssistent.Data
{
    public interface IFileService
    {
        public void WriteSbcToFile(List<GA> gAs);
        public void WriteThToFile(List<GAWithTh> gAWithThs);
        public List<GAWithTh> ReadThFromFile();
        public List<GA> ReadSbcFromFile();
    }
}
