using DAL.Models;
using KNXManager.Globals;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KNXManager.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        readonly List<GA> gaList = new();

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
            }
            catch(Exception)
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
    }
}
