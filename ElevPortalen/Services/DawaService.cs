using System.Text.Json;

namespace ElevPortalen.Services
{//Lavet af Jozsef
    using ElevPortalen.Models; // Using Models

    //DAWA stands for Danmarks Adressers Web API ( https://dawadocs.dataforsyningen.dk/ )
    public class DawaService
    {
        private readonly HttpClient httpClient;

        #region constructor
        public DawaService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        #endregion

        #region GetAdress function
        public async Task<List<AddressModel>> GetAddress(string searchTerm)
        {
            var response = await httpClient.GetAsync($"https://api.dataforsyningen.dk/adresser/autocomplete?q={searchTerm}");
            Console.WriteLine($"API Response: {response}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var address = JsonSerializer.Deserialize<List<AddressModel>>(content);
                return address;
            }
            else
            {
                // Handle the case where the API response is not successful.
                return null;
            }
        }
        #endregion
    }
}
