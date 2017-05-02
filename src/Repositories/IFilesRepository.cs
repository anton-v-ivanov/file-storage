using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Repositories
{
    public interface IFilesRepository
    {
        Task Save(string filePath, IFormFile file);

        FileResult Get(string fileName);

        Task Delete(string fileName);
    }
}