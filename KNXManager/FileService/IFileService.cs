using DAL.Models;
using System.Collections.Generic;


namespace KNXManager.FileService
{
    public interface IFileService
    {
        public void WriteSbcToFile(List<GA> gAs);
        public void WriteThToFile(List<GAWithTh> gAWithThs);
        public List<GAWithTh> ReadThFromFile();
        public List<GA> ReadSbcFromFile();
    }
}
