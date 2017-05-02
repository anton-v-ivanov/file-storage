using System.Threading.Tasks;
using FileStorage.Dispatchers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatsdClient;

namespace FileStorage.Controllers
{
    [Route("api/files")]
    public class FilesController : Controller
    {
        private readonly IFilesDispatcher _filesDispatcher;

        public FilesController(IFilesDispatcher filesDispatcher)
        {
            _filesDispatcher = filesDispatcher;
        }
        
        [HttpGet("{id}", Name = "GetFile")]
        public IActionResult Get(string id)
        {
            using (Metrics.StartTimer("file-storage.get-file.time"))
            {
                var result = _filesDispatcher.Get(id);
                if (result == null)
                {
                    Metrics.Counter("file-storage.get-file.file-not-found");
                    return NotFound();
                }

                Metrics.Counter("file-storage.get-file.file-served");
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            using (Metrics.StartTimer("file-storage.upload-file.time"))
            {
                if (file == null || file.Length == 0)
                {
                    Metrics.Counter("file-storage.upload-file.empty-file");
                    return BadRequest("File is empty");
                }

                var fileName = await _filesDispatcher.Save(file);
                Metrics.Counter("file-storage.upload-file.file-uploaded");
                return CreatedAtRoute("GetFile", new {id = fileName}, null);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            using (Metrics.StartTimer("file-storage.delete-file.time"))
            {
                await _filesDispatcher.Delete(id);
                Metrics.Counter("file-storage.delete-file.file-deleted");
                return NoContent();
            }
        }
    }
}
