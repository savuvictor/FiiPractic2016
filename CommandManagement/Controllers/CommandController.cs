using Daishi.AMQP;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CommandManagement.Controllers
{
    [RoutePrefix("api/command")]
    public class CommandController : ApiController
    {
        [HttpGet]
        [Route("{sensorId}")]

        // localhost:5171/api/command/Temperature_Sensor

        public IHttpActionResult GetCommand(string sensorId)
        {   
            try
            {
                if (sensorId == null) return BadRequest();

                var account = GetAccountSettings(sensorId);
                var sensorValue = GetSensorMeasurement(sensorId);

                string result = "";

                if (sensorId.ToLower().Contains("temp"))
                {
                    //temperature sensor
                    result = string.Format("Account Temp: {0} vs Sensor Temp: {1}", account.Temperature, sensorValue);
                }
                else
                {
                    //switch sensor

                    SetSensorMeasurement(sensorId, sensorValue == 0 ? 1 : 0);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private dynamic GetAccountSettings(string sensorId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:2524/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var response = client.GetAsync("api/account/GetSettings?sensorId=" + sensorId).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                    }
                    return new
                    {
                        Temperature = 22,
                        SleepHour = 23,
                        SleepMinute = 30
                    };

                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        private int GetSensorMeasurement(string sensorId)
        {
            var rabbitMqAdapterInstance = RabbitMQAdapter.Instance;
            rabbitMqAdapterInstance.Init("localhost", 5672, "guest", "guest", 50);
            rabbitMqAdapterInstance.Connect();

            var random = new Random();
            int value = random.Next(5, 30);

            rabbitMqAdapterInstance.Publish(sensorId + ": " + value, "SensorManagement");

            string response;
            BasicDeliverEventArgs args;

            var responded = rabbitMqAdapterInstance.TryGetNextMessage("SensorManagementResult", out response, out args, 5000);

            return value;
        }

        private void SetSensorMeasurement(string sensorId, int value)
        {
            var rabbitMqAdapterInstance = RabbitMQAdapter.Instance;
            rabbitMqAdapterInstance.Init("localhost", 5672, "guest", "guest", 50);
            rabbitMqAdapterInstance.Connect();

            value = 11;

            rabbitMqAdapterInstance.Publish(sensorId + value, "SensorManagement");
        }
    }
}
