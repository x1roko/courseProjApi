using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using courseProjAPI.Models;

namespace courseProjAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly BrosShopDbContext _context;
        private readonly string _fileDirectory = "/var/media/";

        public ImagesController(BrosShopDbContext context)
        {
            _context = context;
        }

        // GET: api/Images/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BrosShopImage>> GetBrosShopImage(int id)
        {
            var brosShopImage = await _context.BrosShopImages.FindAsync(id);

            if (brosShopImage == null)
            {
                return NotFound($"Image with ID {id} not found.");
            }

            var imagePath = Path.Combine(_fileDirectory, brosShopImage.BrosShopImageTitle);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound($"File not found at path: {imagePath}");
            }

            var contentType = "application/octet-stream"; 

            using (var file = new FileStream( imagePath,FileMode.Open, FileAccess.Read, FileShare.Read, 0, true))
            {
                return File(file, contentType, brosShopImage.BrosShopImageTitle);
            }
        }


        [HttpPost]
        public async Task<ActionResult<BrosShopImage>> UploadBrosShopImage(int productId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не загружен.");
            }

            // Генерация уникального имени файла или использование оригинального имени файла
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_fileDirectory, fileName);

            // Сохранение файла в указанной директории
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Создание новой записи BrosShopImage
            var brosShopImage = new BrosShopImage
            {
                BrosShopImageTitle = fileName,
                BrosShopProductId = productId
            };

            // Добавление новой записи в базу данных
            _context.BrosShopImages.Add(brosShopImage);
            await _context.SaveChangesAsync();

            // Возврат созданной записи изображения
            return CreatedAtAction(nameof(GetBrosShopImage), new { id = brosShopImage.BrosShopImagesId}, brosShopImage);
        }
    }
}
