using CoreWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using WeatherForecast.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CoreWebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private string appId; 
        private string link1 = "http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&appid={1}";
        private string link2 = "http://api.openweathermap.org/data/2.5/weather?zip={0},{1}&units=metric&cnt=1&appid={2}";
        private string link3 = "http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&units=metric&cnt=1&appid={2}";

        WeatherViewModel weatherIn = new WeatherViewModel()
        {City = "Montreal" //default
        };
       

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            appId = _configuration["OpenMapAPI:Key"];
                    //"cf002751564a4c78f5f7ed479f1b9ba3";


        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Weather()
        {

            return View(weatherIn);
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Weather(string City, string Zipcode, string Country, string Lon, string Lat)
        {

            //Assign API KEY for Openweathermap.org
                        
            string url = (!String.IsNullOrEmpty(City)) ? string.Format(link1, City, appId) :
                        (!String.IsNullOrEmpty(Zipcode)&& !String.IsNullOrEmpty(Country)) ? string.Format(link2, Zipcode, Country, appId) : 
                         ((!String.IsNullOrEmpty(Lon) && !String.IsNullOrEmpty(Lat)) ? string.Format(link3, Lat, Lon, appId):"");
            
           
            WeatherOutput weatherInfo = new WeatherOutput();
            try
            {
                using (WebClient client = new WebClient())
                {
                    string json = client.DownloadString(url);

                    /*Example API Response: 
                     * {
                           "coord": {
                             "lon": -122.08,
                             "lat": 37.39
                           },
                           "weather": [
                             {
                               "id": 800,
                               "main": "Clear",
                               "description": "clear sky",
                               "icon": "01d"
                             }
                           ],
                           "base": "stations",
                           "main": {
                             "temp": 282.55,
                             "feels_like": 281.86,
                             "temp_min": 280.37,
                             "temp_max": 284.26,
                             "pressure": 1023,
                             "humidity": 100
                           },
                           "visibility": 16093,
                           "wind": {
                             "speed": 1.5,
                             "deg": 350
                           },
                           "clouds": {
                             "all": 1
                           },
                           "dt": 1560350645,
                           "sys": {
                             "type": 1,
                             "id": 5122,
                             "message": 0.0139,
                             "country": "US",
                             "sunrise": 1560343627,
                             "sunset": 1560396563
                           },
                           "timezone": -25200,
                           "id": 420006353,
                           "name": "Mountain View",
                           "cod": 200
                           }   
                     */

                    weatherInfo = JsonSerializer.Deserialize<WeatherOutput>(json);

                    weatherIn.Country = weatherInfo.sys.country;
                    weatherIn.City = weatherInfo.name;
                    weatherIn.Lat = Convert.ToString(weatherInfo.coord.lat);
                    weatherIn.Lon = Convert.ToString(weatherInfo.coord.lon);
                    weatherIn.Description = weatherInfo.weather[0].description;
                    weatherIn.Humidity = Convert.ToString(weatherInfo.main.humidity);
                    weatherIn.Temp = Convert.ToString(weatherInfo.main.temp);
                    weatherIn.TempFeelsLike = Convert.ToString(weatherInfo.main.feels_like);
                    weatherIn.TempMax = Convert.ToString(weatherInfo.main.temp_max);
                    weatherIn.TempMin = Convert.ToString(weatherInfo.main.temp_min);
                    weatherIn.WeatherIcon = "http://openweathermap.org/img/wn/"+weatherInfo.weather[0].icon+".png";
                    return Weather();
                }
            }
            catch(Exception)
            {
                weatherIn.City = "!!!!! Weather could not be found !!!!!";
                return Weather();
            }
        }
    }
}
