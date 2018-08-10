using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WeatherService.Models;

namespace WeatherService.DataManagement
{
    public static class DataStore
    {
        private static string _path = HttpContext.Current.Server.MapPath("~/App_Data/Data.json");

        public static WeatherModel.RootObject Read()
        {

            return JsonConvert.DeserializeObject<WeatherModel.RootObject>(File.ReadAllText(_path));
            
        }
    }
}
