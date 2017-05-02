using System;
using System.IO;
using System.Threading.Tasks;
using FileStorage.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FileStorage.Dispatchers
{
    public class FilesDispatcher : IFilesDispatcher
    {
        private readonly IFilesRepository _repository;
        private readonly string _uploadsPath;

        public FilesDispatcher(IOptions<UploadConfiguration> configuration, IFilesRepository repository)
        {
            _repository = repository;
            _uploadsPath = configuration.Value.UploadPath;
        }

        public async Task<string> Save(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString("N");
            var filePath = GetFullFileName(fileName);
            await _repository.Save(filePath, file);
            return fileName;
        }

        public FileResult Get(string id)
        {
            return _repository.Get(GetFullFileName(id));
        }

        public Task Delete(string id)
        {
            return _repository.Delete(GetFullFileName(id));
        }

        private string GetFullFileName(string id)
        {
            return Path.Combine(_uploadsPath, id);
        }
    }
}