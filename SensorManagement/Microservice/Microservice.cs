using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Daishi.AMQP;

namespace SensorManagement.Microservice
{
    public class Microservice : IMicroservice
    {
        private RabbitMQAdapter _adapter;
        private RabbitMQConsumerCatchAll _rabbitMqConsumerCatchAll;

        public void Init()
        {
            _adapter = RabbitMQAdapter.Instance;
            _adapter.Init("localhost", 5672, "guest", "guest", 50);

            _rabbitMqConsumerCatchAll = new RabbitMQConsumerCatchAll("SensorManagement", 5000);
            _rabbitMqConsumerCatchAll.MessageReceived += OnMessageReceived;

            _adapter.Connect();
            _adapter.ConsumeAsync(_rabbitMqConsumerCatchAll);
        }

        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string[] values = e.Message.Split('/');
            Console.WriteLine("Received: " + e.Message);

            try
            {
                string requestResult;
                string sensorId = values[0];

                if (values.Length == 1)
                {
                    //get value command
                    var random = new Random();

                    if (sensorId.ToLower().Contains("temp"))
                    {
                        //is a temperature sensor
                        requestResult = random.Next(15, 35).ToString();
                    }
                    else
                    {
                        //is a switch sensor
                        requestResult = random.Next(0, 1).ToString();
                    }
                    requestResult = requestResult + "@ " + e.Message;
                }
                else
                {
                    //set value command
                    string newVal = values[1];

                    requestResult = newVal + "@ " + e.Message;
                }
            
                _adapter.Publish(requestResult, "SensorManagementResult");
                Console.WriteLine("Published: " + requestResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
            }
        }

        public void Shutdown()
        {
            if (_adapter == null) return;
            if (_rabbitMqConsumerCatchAll != null)
            {
                _adapter.StopConsumingAsync(_rabbitMqConsumerCatchAll);
            }
            _adapter.Disconnect();
        }
    }
}
