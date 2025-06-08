using System.Threading.Tasks;
using Dermainsight.Models;
using Dermainsight.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dermainsight.Controllers
{
    public class DiseaseController : Controller
    {
        private readonly FastApiService _fastApiService;

        public DiseaseController(FastApiService fastApiService)
        {
            _fastApiService = fastApiService;
        }

        public IActionResult Index()
        {
            List<Users> userList = _fastApiService.getPatients().Result;
            List<Diseases> diseaseList = _fastApiService.getDiseases().Result;
            DiseaseModel diseaseModel = new DiseaseModel
            {
                userList = userList,
                diseaseList = diseaseList
            };
            return View(diseaseModel);
        }

        [HttpPost, ActionName("Index")]
        public async Task<IActionResult> IndexPOST(String PatientId, String DiseaseId)
        {
            int DoctorId = CurrentUser.activeUser.Id;

            DiseaseInfo diseaseInfo = new DiseaseInfo
            {
                PatientId = Int32.Parse(PatientId),
                DiseaseId = Int32.Parse(DiseaseId),
                DoctorId = DoctorId
            };

            await _fastApiService.CreateDiseaseInfo(diseaseInfo);
            return RedirectToAction("Index", "Disease");
        }
    }
}
