using DAL.Models;
using Knx.Bus.Common;
using KNXManager.Globals;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KNXManager.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public List<GA> ReadSbcFromFile()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.GASbcList);
            using StreamReader sr = new(path);
            var jsonData = sr.ReadToEnd();
            sr.Close();
            List<GA> result = new();
            try
            {
                result = JsonSerializer.Deserialize<List<GA>>(jsonData);
                if (result.Count != 0)
                {
                    foreach (var ga in result)
                    {
                        if (GroupAddress.TryParse(ga.GAddress, out GroupAddress groupAddress))
                        {
                            ga.Address = groupAddress;
                        }
                    }
                }

            }
            catch (Exception)
            {
                return new List<GA>();
            }
            return result;
        }

        public List<GAWithTh> ReadThFromFile()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.GAThList);
            using StreamReader sr = new(path);
            var jsonData = sr.ReadToEnd();
            sr.Close();
            return JsonSerializer.Deserialize<List<GAWithTh>>(jsonData);
        }

        public void WriteSbcToFile(List<GA> gAs)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.GASbcList);
            var jsonData = JsonSerializer.Serialize(gAs);
            using StreamWriter sw = new(path, false);
            sw.Write(jsonData);
            sw.Close();
        }

        public void WriteThToFile(List<GAWithTh> gAWithThs)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.GAThList);
            var jsonData = JsonSerializer.Serialize(gAWithThs);
            using StreamWriter sw = new(path, false);
            sw.Write(jsonData);
            sw.Close();
        }

        public void WriteSbcValueToFile(List<GaValue> gaValues)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", "Data", DateTime.Now.ToShortDateString(), Secret.GASbcValue);
            var dir = Path.Combine(_webHostEnvironment.WebRootPath, "files", "Data", DateTime.Now.ToShortDateString());
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var json = JsonSerializer.Serialize(gaValues);
            using StreamWriter sw = new(path, true);
            sw.Write(json);
            sw.Flush();
            sw.Close();
        }

        public void ClearSbcValueFile()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.GASbcValue);
            using StreamWriter sw = new(path, false);
            sw.Write(string.Empty);
            sw.Close();
        }

        // ACU Service

        public async Task<List<ACUnit>> ReadACUsFromFileAsync()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.AcuList);
            using StreamReader sr = new(path);
            var jsonDataStream = sr.BaseStream;
            return await JsonSerializer.DeserializeAsync<List<ACUnit>>(jsonDataStream);
        }

        public async Task<List<ACError>> ReadErrorFromFileAsync()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.AcuErrors);
            using StreamReader sr = new(path);
            var jsonDataStream = sr.BaseStream;
            return await JsonSerializer.DeserializeAsync<List<ACError>>(jsonDataStream);
        }

        public async Task WriteACUsToFileAsync(List<ACUnit> Acus)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.AcuList);
            using FileStream fs = new(path, FileMode.Create);
            await JsonSerializer.SerializeAsync(fs, Acus);
            await fs.DisposeAsync();
        }

        public async Task WriteAcuErrorsToFileAsync(List<ACError> errors)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.AcuErrors);
            using FileStream fs = new(path, FileMode.Create);
            await JsonSerializer.SerializeAsync(fs, errors);
            await fs.DisposeAsync();
        }

        public List<ACUnit> ReadACUsFromFile()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.AcuList);
            using StreamReader sr = new(path);
            var jsonData = sr.ReadToEnd();
            sr.Close();
            return JsonSerializer.Deserialize<List<ACUnit>>(jsonData);
        }

        public List<ACError> ReadErrorsFromFile()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.AcuErrors);
            using StreamReader sr = new(path);
            var json = sr.ReadToEnd();
            return JsonSerializer.Deserialize<List<ACError>>(json);
        }
        
        public async Task WriteErrorValuesToFileAsync(List<ErrorValue> errorValues)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ACUErrors", DateTime.Now.ToShortDateString(), Secret.AcuErrorValues);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using FileStream fs = new(path, FileMode.Append);
            await JsonSerializer.SerializeAsync(fs, errorValues);
            await fs.DisposeAsync();
        }
    }
}
