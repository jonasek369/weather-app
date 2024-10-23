using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace weatherApp
{
    class ApiController
    {
        private static string apiUrl = "https://api.open-meteo.com/v1/";
        private List<Mesto> cities = new List<Mesto>();
        public JsonDocument weatherDescriptor;
        private string dayParameter = "";

        private string GetEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException("Resource not found: " + resourceName);
                }

                using (var reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        

        public ApiController()
        {
            var citiesDbLines = GetEmbeddedResource("weatherApp.cities-database.csv").Split("\n");
            foreach (var line in citiesDbLines)
            {
                if (line == null || line == "")
                    continue;
                if (line == "ASCII Name;Country Code;Coordinates\r")
                    continue;
                string[] a = line.Split(';');
                string cityName = a[0];
                string countryCode = a[1];
                string coordinates = a[2];
                string[] values = coordinates.Remove(coordinates.Length).Replace(".", ",").Split(", ");
                float latitude = float.Parse(values[0]);
                float longitude = float.Parse(values[1]);
                cities.Add(new Mesto(cityName, countryCode, new Tuple<float, float>(latitude, longitude)));
            }
            var descriptorString = GetEmbeddedResource("weatherApp.WmoCodes.json");
            weatherDescriptor = JsonDocument.Parse(descriptorString);

            DateTime today = DateTime.Today;
            DayOfWeek dayOfWeek = today.DayOfWeek;
            int past_days = (int)dayOfWeek - 1;
            if (past_days < 0)
            {
                past_days = 6;
            }
            int forecast_days = 7 - past_days;
            dayParameter = $"&past_days={past_days}&forecast_days={forecast_days}";
        }

        public Mesto[] looseSearch(string name)
        {
            return cities.Where(city => city.name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
        }

        public async Task<string> getWeatherReport(Mesto mesto)
        {
            var requestUrl = $"forecast?latitude={mesto.coordinates.Item1.ToString().Replace(",", ".")}&longitude={mesto.coordinates.Item2.ToString().Replace(",", ".")}&hourly=temperature_2m&daily=weather_code,temperature_2m_max,temperature_2m_min,uv_index_max&timezone=GMT"+dayParameter;
            var response = await new HttpClient().GetAsync(apiUrl + requestUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

