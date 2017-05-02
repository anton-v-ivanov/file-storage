using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FileStorage.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        private readonly ILogger<FilesRepository> _logger;

        public FilesRepository(ILogger<FilesRepository> logger)
        {
            _logger = logger;
        }

        public async Task Save(string filePath, IFormFile file)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                throw;
            }
        }

        public FileResult Get(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            try
            {
                var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fileStream, "image/jpeg");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                throw;
            }
        }

        public Task Delete(string fileName)
        {
            if (!File.Exists(fileName))
                return Task.CompletedTask;

            return Task.Factory.StartNew(() => File.Delete(fileName));
        }
    }
}