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
        public List<GA> ReadSbcFromFile();

        // ACU services
        public Task<List<ACError>> ReadErrorFromFileAsync();
        public Task<List<ACUnit>> ReadACUsFromFileAsync();
        public List<ACUnit> ReadACUsFromFile();
        public List<ACError> ReadErrorsFromFile();
        public Task WriteACUsToFileAsync(List<ACUnit> Acus);
        public Task WriteAcuErrorsToFileAsync(List<ACError> errors);
        public Task WriteErrorValuesToFileAsync(List<ErrorValue> errorValues);

        // ACU services
    }
}
