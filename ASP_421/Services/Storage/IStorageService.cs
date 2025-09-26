namespace ASP_421.Services.Storage
{
    public interface IStorageService
    {
        String Save(IFormFile formFile); // return ImageUrl (filename)
        byte[] Load(String filename);


    }
}
