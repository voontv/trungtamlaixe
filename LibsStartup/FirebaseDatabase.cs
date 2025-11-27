using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ttlaixe.LibsStartup
{
    public class FirebaseDatabase
    {
        static string firebaseDatabaseUrl = "https://it-dawaco.firebaseio.com/";
        static string firebaseDatabaseDashboard = "dashboard";
        static readonly HttpClient client = new HttpClient();
        public static async Task<string> Add(string nameAPi, string valueApi, string table = "dashboard")
        {

            var payload = new StringContent(valueApi, Encoding.UTF8, "application/json");

            string url = $"{firebaseDatabaseUrl}" +
                        $"{table}/" +
                        $"{nameAPi}.json";


            var httpResponseMessage = await client.PutAsync(url, payload);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                return contentStream;
            }

            return null;

        }

        public static async Task<string> Get(string nameAPi, string valueApi, string table = "dashboard")
        {

            var payload = new StringContent(valueApi, Encoding.UTF8, "application/json");

            string url = $"{firebaseDatabaseUrl}" +
                        $"{table}/" +
                        $"{nameAPi}.json";


            var httpResponseMessage = await client.PutAsync(url, payload);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                return contentStream;
            }

            return null;

        }

        public static async Task<string> Remove(string nameAPi, string table = "dashboard")
        {

            //var payload = new StringContent(valueApi, Encoding.UTF8, "application/json");

            string url = $"{firebaseDatabaseUrl}" +
                        $"{table}/" +
                        $"{nameAPi}.json";


            var httpResponseMessage = await client.DeleteAsync(url);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                return contentStream;
            }

            return null;

        }
    }
}
