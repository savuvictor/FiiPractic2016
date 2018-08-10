using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeatherService.Models;
using WeatherService.DataManagement;
using System.Net.Http.Headers;
using System.Web;
using System.IO;

namespace WeatherService.Controllers
{
    [RoutePrefix("api/weather")]
    public class WeatherController : ApiController
    {
        // http://localhost:59880/api/weather/GetTemperature/Iasi
        // if you introduce invalid city names sometimes it may show random temperature
        // TODO: Fix random temp when introducing invalid city name

        [HttpGet]
        [Route("GetTemperature/{cityname}")]
        public IHttpActionResult GetTemperature(string cityname)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var response = client.GetAsync("data/2.5/weather?q=" + cityname + ",uk&appid=24f455d3ac142f4690e1048203c03a5e").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;

                        string _path = HttpContext.Current.Server.MapPath("~/App_Data/Data.json");
                        File.WriteAllText(_path, result);

                        var all_data = DataStore.Read();

                        // transform kelvin to celsuis 
                        var temp_celsuis = all_data.main.temp - 273.15;
                        var currenttemp = "In " + cityname + " are " + temp_celsuis + " Celsius degrees.";

                        return Ok(currenttemp);
                    }
                    return Ok();
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }
    }
}