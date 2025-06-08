using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Dermainsight.Models
{
    public class DiseaseInfo
    {
        public int? Id { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? DiseaseId { get; set; }
        public DateTime? DiagnosisDate { get; set; }

        public Users? Doctor { get; set; }
        public Users? Patient { get; set; }
        public Diseases? Disease { get; set; }
    }

    public class DiseaseInfoModel
    {
        public List<DiseaseInfo>? diseaseInfos { get; set; }
        public List<Users>? patientList { get; set; }
    }
}
