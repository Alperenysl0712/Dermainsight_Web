using Dermainsight.Models;
using Dermainsight.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dermainsight.Controllers
{
    public class DiseaseInfoController : Controller
    {
        private readonly FastApiService _fastApiService;

        public DiseaseInfoController(FastApiService fastApiService)
        {

            _fastApiService = fastApiService;
        }

        public async Task<IActionResult> Patient(int patient_id)
        {
            List<DiseaseInfo> diseaseInfos = await _fastApiService.getDiseaseInfo(patient_id);
            return View(diseaseInfos);
        }
        
        public async Task<IActionResult> Doctor(int? patientId)
        {
            List<Users> patientList = await _fastApiService.getPatientsByDoctor(CurrentUser.activeUser!.Id);
            DiseaseInfoModel diseaseInfoModel = new DiseaseInfoModel()
            {
                patientList = patientList ?? [],
                diseaseInfos = patientId == null ? [] : await _fastApiService.getDiseaseInfo(patientId ?? 0)
            };
            return View(diseaseInfoModel);
        }

        [HttpPost, ActionName("Doctor")]
        public IActionResult DoctorPost(int? patientId)
        {
            return RedirectToAction("Doctor", "DiseaseInfo", new { patientId = patientId });
        }
    }

}
