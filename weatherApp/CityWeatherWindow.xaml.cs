using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace weatherApp
{
    /// <summary>
    /// Interakční logika pro Window1.xaml
    /// </summary>
    public partial class CityWeatherWindow : Window
    {
        private Mesto drawnMesto;
        private JsonDocument weatherData;
        private JsonDocument descriptor;

        static int nejblizsiDatum(string[] dateStrings)
        {
            DateTime now = DateTime.Now;
            int closestDateIndex = -1;
            TimeSpan shortestTimeSpan = TimeSpan.MaxValue;

            for (int i = 0; i < dateStrings.Length; i++)
            {
                if (DateTime.TryParse(dateStrings[i], out DateTime parsedDate))
                {
                    TimeSpan timeSpan = (parsedDate - now).Duration();

                    if (timeSpan < shortestTimeSpan)
                    {
                        shortestTimeSpan = timeSpan;
                        closestDateIndex = i;
                    }
                }
                else
                {
                    Console.WriteLine($"Unable to parse date: {dateStrings[i]}");
                }
            }

            return closestDateIndex;
        }

        public CityWeatherWindow(Mesto pMesto, JsonDocument pWeatherData, JsonDocument pDescriptor)
        {
            InitializeComponent();
            drawnMesto = pMesto;
            weatherData = pWeatherData;
            descriptor = pDescriptor;

            mesto.Content = drawnMesto.name;

            string[] dates = JsonSerializer.Deserialize<string[]>(weatherData.RootElement.GetProperty("daily").GetProperty("time"));
            float[] maxTemps = JsonSerializer.Deserialize<float[]>(weatherData.RootElement.GetProperty("daily").GetProperty("temperature_2m_max"));
            float[] minTemps = JsonSerializer.Deserialize<float[]>(weatherData.RootElement.GetProperty("daily").GetProperty("temperature_2m_min"));
            int[] weatherCodes = JsonSerializer.Deserialize<int[]>(weatherData.RootElement.GetProperty("daily").GetProperty("weather_code"));

            string[] hourlyDates = JsonSerializer.Deserialize<string[]>(weatherData.RootElement.GetProperty("hourly").GetProperty("time"));
            float[] hourlyTemps = JsonSerializer.Deserialize<float[]>(weatherData.RootElement.GetProperty("hourly").GetProperty("temperature_2m"));

            int datumIndex = nejblizsiDatum(dates);
            int hourlyDatumIndex = nejblizsiDatum(hourlyDates);
            var datumDescription = descriptor.RootElement.GetProperty(weatherCodes[datumIndex].ToString()).GetProperty("day").GetProperty("description").ToString();

            pocasiMinMaxTeplota.Content = $"{datumDescription} {((int)minTemps[datumIndex]).ToString()}°C / {((int)maxTemps[datumIndex]).ToString()}°C";
            teplota.Content = ((int)hourlyTemps[hourlyDatumIndex]).ToString() +"°C";


            BitmapImage countryImage = new BitmapImage();
            countryImage.BeginInit();
            countryImage.UriSource = new Uri($"https://flagpedia.net/data/flags/h80/{drawnMesto.countryCode.ToLower()}.png?v=un", UriKind.Absolute);
            countryImage.EndInit();

            countryFlag.Source = countryImage;

            for (int i = 0; i < 7; i++) {
                int code = -1;
                string imageUrl = "";
                string currentDescription = "";
                string minMaxTemp = "";
                try {
                    code = weatherCodes[i];
                    imageUrl = descriptor.RootElement.GetProperty(code.ToString()).GetProperty("day").GetProperty("image").ToString();
                    currentDescription = descriptor.RootElement.GetProperty(code.ToString()).GetProperty("day").GetProperty("description").ToString();
                    minMaxTemp = $"{((int)minTemps[i]).ToString()}°C/{((int)maxTemps[i]).ToString()}°C";
                }
                catch(Exception e)
                {
                    Console.WriteLine("Could not find " + code.ToString() + " " + e.Message.ToString());
                    continue;
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imageUrl, UriKind.Absolute);
                image.EndInit();


                switch (DateTime.Parse(dates[i]).DayOfWeek) { 
                    case DayOfWeek.Monday:
                        pondeliImage.Source = image;
                        pocasiPondeli.Content = currentDescription;
                        minMaxTempPondeli.Content = minMaxTemp;
                        break;
                    case DayOfWeek.Tuesday:
                        uteryImage.Source = image;
                        pocasiUtery.Content = currentDescription;
                        minMaxTempUtery.Content = minMaxTemp;
                        break;
                    case DayOfWeek.Wednesday:
                        stredaImage.Source = image;
                        pocasiStreda.Content = currentDescription;
                        minMaxTempStreda.Content = minMaxTemp;
                        break;
                    case DayOfWeek.Thursday:
                        ctvrtekImage.Source = image;
                        pocasiCtvrtek.Content = currentDescription;
                        minMaxTempCtvrtek.Content = minMaxTemp;
                        break;
                    case DayOfWeek.Friday:
                        patekImage.Source = image;
                        pocasiPatek.Content = currentDescription;
                        minMaxTempPatek.Content = minMaxTemp;
                        break;
                    case DayOfWeek.Saturday:
                        sobotaImage.Source = image;
                        pocasiSobota.Content = currentDescription;
                        minMaxTempSobota.Content = minMaxTemp;
                        break;
                    case DayOfWeek.Sunday:
                        nedeleImage.Source = image;
                        pocasiNedele.Content = currentDescription;
                        minMaxTempNedele.Content = minMaxTemp;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
