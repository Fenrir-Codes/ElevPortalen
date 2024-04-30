using System.Text.Json;

/// <summary>
///  Lavet af Jozsef
/// </summary>
namespace ElevPortalen.Services
{
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
        public async Task<(List<AddressModel>? Addresses, string? ErrorMessage)> GetAddress(string searchTerm)
        {
            try
            {
                var response = await httpClient.GetAsync($"https://api.dataforsyningen.dk/adresser/autocomplete?q={searchTerm}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var addresses = JsonSerializer.Deserialize<List<AddressModel>>(content);
                    return (addresses, null); // Return addresses and no error message
                }
                else
                {
                    // Handle the case where the API response is not successful.
                    return (null, $"API request failed with status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                // Handle the case where there is no internet connection
                return (null, "No internet connection, or the service unreachable in the moment.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return (null, $"Error occurred while getting the address data: {ex.Message}");
            }
        }
        #endregion

    }
}
