
using Microsoft.AspNetCore.Http;

namespace ASP_421.Services.Storage
{
    public class DiskStorageService : IStorageService
    {
        private const String StoragePath = "C:/Users/Lenovo/source/repos/ASP.NET/storage/ASP421/";
        public byte[] Load(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                {
                throw new ArgumentException("Empty file name", nameof(filename));
            }
            String savedPath = Path.Combine(StoragePath, filename);
            if (File.Exists(savedPath))
            {
                return File.ReadAllBytes(savedPath);
            }
            else throw new FileNotFoundException();
        }
        
        public string Save(IFormFile formFile)
        {
            ArgumentNullException.ThrowIfNull(formFile, nameof(formFile));
            if(formFile.Length==0)
            {
                throw new ArgumentException("Empty file rejected", nameof(formFile));
            }
            String ext = Path.GetExtension(formFile.FileName);
            if(String.IsNullOrEmpty(ext) )
            {
                throw new ArgumentException("Empty file extension", nameof(formFile));
            }

            // TODO: додати перелік дозволених розширень та перевіряти на нього
            String savedName = Guid.NewGuid().ToString() + ext;
            String savedPath = Path.Combine(StoragePath, savedName);
            using var writer = File.OpenWrite(savedPath); // видаляє writer після його використання
            formFile.CopyTo(writer);
            return savedName;
        }
    }
}
