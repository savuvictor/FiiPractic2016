using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using FiiPracticProject.Models;
using Newtonsoft.Json;

namespace FiiPracticProject.DataStore
{
    public static class DataStoreUtil
    {
        private static string _path = HttpContext.Current.Server.MapPath("~/App_Data/DataStore.json");

        public static List<AccountModel> ReadModels()
        {
            return JsonConvert.DeserializeObject<List<AccountModel>>(File.ReadAllText(_path));
        }

        public static void SaveModels(List<AccountModel> accountModels)
        {
            string json = JsonConvert.SerializeObject(accountModels);
            File.WriteAllText(_path, json);
        }
    }
}