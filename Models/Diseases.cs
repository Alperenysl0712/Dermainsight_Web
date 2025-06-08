namespace Dermainsight.Models
{
    public class Diseases
    {
        public int Id { get; set; }
        public string DiseaseName { get; set; }
        public string ImageName { get; set; }
        public string ImageAr { get; set; }
        public string DiseaseDetail { get; set; }
    }

    public class DiseaseTr
    {
        public static Dictionary<string, string> disTrList = new()
        {
            { "nevus", "Nevus - {Ben}" },
            { "melanoma", "Melanoma - {Melanom}" },
            { "seborrheic keratosis", "Seborrheic Keratosis - {Seboreik Keratoz}" },
            { "dermatofibroma", "Dermatofibroma - {Dermatofibrom}" },
            { "basal cell carcinoma", "Basal Cell Carcinoma - {Bazal Hücreli Karsinom}" },
            { "squamous cell carcinoma", "Squamous Cell Carcinoma - {Skuamöz Hücreli Karsinom}" },
            { "verruca", "Verruca - {Siğil}" },
            { "pigmented benign keratosis", "Pigmented Benign Keratosis - {Pigmente İyi Huylu Keratoz}" }
        };
    }

    public class DiseaseModel
    {
        public List<Users>? userList { get; set; }
        public List<Diseases>? diseaseList { get; set; }
    }

    public class CurrentDiseaseImage
    {
        public static String? disImage64 { get; set; }
    }
}
