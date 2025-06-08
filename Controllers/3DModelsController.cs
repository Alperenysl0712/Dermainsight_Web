using Dermainsight.Models;
using Dermainsight.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dermainsight.Controllers
{
    public class _3DModelsController : Controller
    {
        private readonly FastApiService _fastApiService;

        public _3DModelsController(FastApiService fastApiService)
        {
            _fastApiService = fastApiService;
        }
        public async Task<IActionResult> Index()
        {
            List<Diseases> diseaseList = await _fastApiService.getDiseases();
            return View(diseaseList);
        }

        public async Task<IActionResult> ShowAsync(String? region, String? body, int diseaseId)
        {
            var diseases = await _fastApiService.getDiseases();
            Diseases? disease = diseases.FirstOrDefault(d => d.Id == diseaseId);

            var base64Image = "data:image/png;base64," + disease!.ImageAr;

            _3DModel model = new _3DModel
            {
                imageBase64 = base64Image,
                diseases = disease,
            };

            return View(model);

        }
    }
}
