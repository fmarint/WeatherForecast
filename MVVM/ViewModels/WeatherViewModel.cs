using PropertyChanged;
using System.Text.Json;
using System.Windows.Input;
using WeatherForecast.MVVM.Models;

namespace WeatherForecast.MVVM.ViewModels
{
   [AddINotifyPropertyChangedInterface]
   public class WeatherViewModel
   {
      public WeatherData? WeatherData { get; set; }
      public string PlaceName { get; set; }
      public DateTime Date { get; set; } = DateTime.Now;
      private HttpClient client;
      public bool IsLoading { get; set; } = false;
      public bool IsLoadComplete { get; set; } = false;
      public WeatherViewModel()
      {
         client = new HttpClient();
      }
      public ICommand SearchCommand => new Command(
       async (searchText) =>
       {
          IsLoading = true;
          IsLoadComplete = false;
          PlaceName = searchText.ToString();
          var location = await GetCoordinatesAsync(PlaceName);
          await GetWeather(location);
          IsLoading = false;
          IsLoadComplete = true;
       });

      private async Task GetWeather(Location location)
      {
         var url = $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&daily=weather_code,temperature_2m_max,temperature_2m_min&current=weather_code,temperature_2m,relative_humidity_2m,precipitation,rain,showers,is_day,wind_speed_10m&timezone=America%2FChicago";

         var response = await client.GetAsync(url);
         if (response.IsSuccessStatusCode)
         {
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
               var data = await JsonSerializer.DeserializeAsync<WeatherData>(responseStream);
               WeatherData = data;

               for (int i = 0; i < WeatherData.daily.time.Length; i++)
               {
                  var daily2 = new Daily2
                  {
                     time = WeatherData.daily.time[i],
                     temperature_2m_max = WeatherData.daily.temperature_2m_max[i],
                     temperature_2m_min = WeatherData.daily.temperature_2m_min[i],
                     weather_code = WeatherData.daily.weather_code[i],
                  };
                  WeatherData.daily2.Add(daily2);
               }
            }
         }
      }

      private async Task<Location> GetCoordinatesAsync(string address)
      {
         IsLoading = true;
         IEnumerable<Location> locations = await Geocoding.GetLocationsAsync(address);
         Location location = locations?.FirstOrDefault();
         if (location != null)
         {
            Console.WriteLine($"Latitude: + {location.Latitude} Longitude: {location.Longitude} Altitud : {location.Altitude} ");
         }
         return location;
      }


   }
}
