using ClientLibrary.Models;
using System.Net.Http.Json;

namespace ClientLibrary.Services
{
    public class ProductService(HttpClient httpClient)
    {
        private const string baseUrl = "http://localhost:5088";
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<ProductDTO>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}/v1/api/products");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProductDTO>>();
        }
    }
}
