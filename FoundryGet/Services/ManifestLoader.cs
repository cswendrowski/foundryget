using FoundryGet.Interfaces;
using FoundryGet.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FoundryGet.Services
{
    public class ManifestLoader : IManifestLoader
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<Manifest> FromUri(Uri uri)
        {
            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Could not read the Manifest: " + response.ReasonPhrase);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Manifest>(json);
        }

        public async Task<Manifest> FromFile(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<Manifest>(json);
        }
    }
}
