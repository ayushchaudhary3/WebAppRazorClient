using System.Net.Http;
using System.Text.Json;

namespace WebAppRazorClient
{
    public class SandwichService
    {
        public SandwichService()
        {

        }

        public async Task<List<SandwichModel>> GetSandwiches()
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                HttpClient client = new HttpClient(handler); // needed for consuming a web api

                await using Stream stream = await client.GetStreamAsync("https://localhost:7084/api/Sandwich");

                //var data = await client.GetAsync("https://localhost:7250/api/Sandwich");
                var sandwiches = await JsonSerializer.DeserializeAsync<List<SandwichModel>>(stream);
                //return sandwiches ?? new();// Initialize a new instance of List<Repository> class
                return sandwiches ?? new List<SandwichModel>();                          // which is empty and has default initial capacity.
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        //Add Sandwich Method
        public async Task<SandwichModel> AddSandwich(SandwichModel sandwich)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.PostAsJsonAsync("https://localhost:7084/api/Sandwich", sandwich);

                response.EnsureSuccessStatusCode();
                var createdSandwich = await response.Content.ReadFromJsonAsync<SandwichModel>();
                return createdSandwich;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteSandwichAsync(int id)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.DeleteAsync($"https://localhost:7084/api/Sandwich/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateSandwich(int id, SandwichModel sandwich)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.PutAsJsonAsync($"https://localhost:7084/api/Sandwich/{id}", sandwich);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SandwichModel> GetSandwichById(int id)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.GetAsync($"https://localhost:7084/api/Sandwich/{id}");
                response.EnsureSuccessStatusCode();
                var sandwich = await response.Content.ReadFromJsonAsync<SandwichModel>();
                return sandwich;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}