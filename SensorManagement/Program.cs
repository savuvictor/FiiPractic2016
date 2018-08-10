using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            var microservice = new Microservice.Microservice();
            microservice.Init();
        }
    }
}
