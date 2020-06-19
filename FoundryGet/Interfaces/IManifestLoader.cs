using FoundryGet.Models;
using System;
using System.Threading.Tasks;

namespace FoundryGet.Interfaces
{
    public interface IManifestLoader
    {
        Task<Manifest> FromUri(Uri uri);

        Task<Manifest> FromFile(string filePath);
    }
}
