using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Dispatchers
{
    public interface IFilesDispatcher
    {
        Task<string> Save(IFormFile form);

        FileResult Get(string id);

        Task Delete(string id);
    }
}