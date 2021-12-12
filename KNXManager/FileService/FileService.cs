﻿using DAL.Models;
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
        readonly List<GA> gaList = new();

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<GA>> ReadSbcFromFileAsync()
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "files", Secret.GASbcList);
            using StreamReader sr = new(path);
            var jsonData = await sr.ReadToEndAsync();
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

    }
}
