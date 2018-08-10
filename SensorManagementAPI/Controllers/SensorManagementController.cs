using Daishi.AMQP;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;

namespace SensorManagementAPI.Controllers
{
    [RoutePrefix("api/sensormanagement")]
    public class SensorManagementController : ApiController
    {
        [HttpGet]
        [Route("AddMesurement")]
        public IHttpActionResult AddMesurment(int value, string sensorId)
        {
            try
            {
                var rabbitMqAdapterInstance = RabbitMQAdapter.Instance;
                rabbitMqAdapterInstance.Init("localhost", 5672, "guest", "guest", 50);
                rabbitMqAdapterInstance.Connect();

                rabbitMqAdapterInstance.Publish(sensorId + " " + value, "SensorManagement");

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
