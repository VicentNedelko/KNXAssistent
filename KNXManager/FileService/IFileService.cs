using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNXManager.FileService
{
    public interface IFileService
    {
        public void WriteSbcToFile(List<GA> gAs);
        public void WriteSbcValueToFile(List<GaValue> gaValues);
        public void WriteThToFile(List<GAWithTh> gAWithThs);
        public void ClearSbcValueFile();
        public List<GAWithTh> ReadThFromFile();
        public Task<List<GA>> ReadSbcFromFileAsync();
    }
}
