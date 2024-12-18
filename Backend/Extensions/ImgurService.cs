using System.Net.Http.Headers;
using System.Text.Json;
using DotEnv.Core;

namespace Backend.Extensions
{
    public class ImgurService
    {
        private readonly HttpClient _httpClient;
        private string base_url = "https://api.imgur.com/3/image";
        private string access_token = EnvReader.Instance["IMGUR_ACCESS_TOKEN"];
        private string album_id = EnvReader.Instance["IMGUR_ALBUM_ID"];

        public ImgurService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }
        public async Task<(string imgurLink, string type)?> UploadImageAsync(IFormFile imageFile)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = imageFile.OpenReadStream();
            content.Add(new StreamContent(fileStream), "image", imageFile.FileName);
            content.Add(new StringContent(album_id), "album");

            var request = new HttpRequestMessage(HttpMethod.Post, base_url)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            string imgurLink = jsonDocument!.RootElement.GetProperty("data").GetProperty("id").GetString() ?? string.Empty;
            string type = jsonDocument!.RootElement.GetProperty("data").GetProperty("type").GetString() ?? string.Empty;
            return (imgurLink, type);
        }

        public async Task<JsonDocument> DeleteImageAsync(string imageId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, base_url + "/" + imageId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);
            return jsonDocument;
        }
    }
}