using Dermainsight.Models;
using Dermainsight.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dermainsight.Controllers
{
    public class AiController : Controller
    {
        private readonly FastApiService _fastApiService;

        public AiController(FastApiService fastApiService)
        {

            _fastApiService = fastApiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("upload-endpoint")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Dosya alınamadı.");

            // ➕ wwwroot/uploads klasörüne kaydet (gerekirse log/önizleme için)
            var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();

                // 🔁 diski tekrar yazmak için başa al ve dosyaya yaz
                memoryStream.Position = 0;
                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }
            }

            // 🔄 byte[] → base64 string
            string base64String = Convert.ToBase64String(imageBytes);

            // 📌 Base64 string'i sakla
            CurrentDiseaseImage.disImage64 = base64String;

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            // 🔁 FastAPI servisine yönlendir (senin servisi çağırarak)
            var predictions = await _fastApiService.UploadImageAsync(image); // <-- Buraya yönlendiriyor

            return Ok(predictions); // veya View(predictions);
        }

        [HttpPost("saveDisease")]
        public IActionResult saveDisease([FromBody] DiseaseModelPost disease)
        {
            CsvDetailDto csvDetailDto = new CsvDetailDto();

            if(disease.DiseaseDetail != "")
            {
                csvDetailDto.disease_name = disease.DiseaseDetail;
            }
            else
            {
                csvDetailDto.disease_name = disease.DiseaseType;
            }

            csvDetailDto.image_base64 = CurrentDiseaseImage.disImage64!;

            bool result = _fastApiService.CreateCsv(csvDetailDto).Result;

            return Ok(result);
        }

        
    }

    public class DiseaseModelPost
    {
        public string DiseaseType { get; set; }
        public string? DiseaseDetail { get; set; }
    }
}
