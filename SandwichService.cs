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
    }
}