namespace ElevPortalen.Services
{

    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.DotNet.Scaffolding.Shared.Messaging;
    using Newtonsoft.Json;

    public class PostCodeService
    {
        private readonly HttpClient _httpClient;

        public PostCodeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PostalCodeModel>?> GetPostalCodes(string searchTerm)
        {
            // Construct the URL with the search term as a query parameter
            var apiUrl = $"https://api.dataforsyningen.dk/postnumre?navn={searchTerm}";

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PostalCodeModel>>(content);
            }
            else
            {
                throw new ApplicationException("The request crashed successfully!");
            }

        }
    }

    public class PostalCodeModel
    {
        public string? Nr { get; set; }
        public string? Navn { get; set; }
    }

}
