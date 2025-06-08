using Dermainsight.Models;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Dermainsight.Services
{
    public class FastApiService
    {
        private readonly HttpClient _httpClient;
        private readonly String _baseUrl = String.Empty;

        public FastApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _baseUrl = configuration["FastApiUrl"] ?? throw new ArgumentNullException("FastApiUrl not found in config");
        }

        public async Task<ServiceHealthResult> checkService()
        {
            string url = $"{_baseUrl.TrimEnd('/')}/health/";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                return new ServiceHealthResult
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    Message = response.IsSuccessStatusCode ? "FastAPI servisi aktif." : $"Hata: {content}",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ServiceHealthResult
                {
                    IsSuccess = false,
                    Message = $"FastAPI bağlantı hatası: {ex.InnerException.Message ?? ex.Message}",
                    StatusCode = 500
                };
            }
        }
        public async Task<String> CreateUser(Users user)
        {
            var url = $"{_baseUrl}/register/";
            var jsonContent = JsonConvert.SerializeObject(user);
            var content = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return await HandleResponse(response);
        }

        public async Task<Users> getUserByUsername(String userName, String userPassword)
        {
            var url = $"{_baseUrl}/getUserByUsername/";
            var jsonContent = JsonConvert.SerializeObject(new {username = userName, password = userPassword});
            var content = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var respText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Users>(respText);
        }

        public async Task<List<Diseases>> getDiseases()
        {
            var url = $"{_baseUrl}/getDiseases/";
            var response = await _httpClient.GetAsync(url);
            var respText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Diseases>>(respText);
        }

        public async Task<List<DiseaseInfo>> getDiseaseInfo(int patient_id)
        {
            var url = $"{_baseUrl}/getDiseaseInfo/";
            var jsonContent = JsonConvert.SerializeObject(new { patient_id = patient_id });
            var content = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var test = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<DiseaseInfo>>(test);

        }

        public async Task<List<Users>> getPatientsByDoctor(int doctor_id)
        {
            var url = $"{_baseUrl}/getPatientsByDoctor/";
            var jsonContent = JsonConvert.SerializeObject(new { doctor_id = doctor_id });
            var content = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var test = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Users>>(test);
        }

        public async Task<List<Users>> getPatients()
        {
            var url = $"{_baseUrl}/getPatients/";
            var response = await _httpClient.GetAsync(url);
            var test = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Users>>(await response.Content.ReadAsStringAsync());
        }

        public async Task CreateDiseaseInfo(DiseaseInfo diseaseInfo)
        {
            var url = $"{_baseUrl}/createDiseaseInfo/";
            var jsonContent = JsonConvert.SerializeObject(diseaseInfo);
            var content = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
        }

        public async Task<List<PredictionResult>> UploadImageAsync(IFormFile imageFile)
        {
            var url = $"{_baseUrl}/getAiDisease";

            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(imageFile.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);

            content.Add(streamContent, "image", imageFile.FileName);

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Resim yüklenemedi. StatusCode: {response.StatusCode}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PredictionResponse>(responseString);

            return result?.Predictions ?? new List<PredictionResult>();
        }

        public async Task<bool> CreateCsv(CsvDetailDto csvDet)
        {
            try
            {
                var url = $"{_baseUrl}/createNewCsv/";

                var json = JsonConvert.SerializeObject(csvDet);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                // _logger.LogError(ex, "CSV oluşturulurken hata oluştu.");

                return false;
            }
        }

        private async Task<string> HandleResponse(HttpResponseMessage response)
        {
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                var errMsg = await response.Content.ReadAsStringAsync();
                return $"Error: {response.StatusCode} - {errMsg}";
            }
        }
    }

    public class CsvDetailDto
    {
        public string disease_name { get; set; }
        public string image_base64 { get; set; }
    }
}
