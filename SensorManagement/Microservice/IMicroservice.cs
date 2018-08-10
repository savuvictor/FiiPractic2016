using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daishi.AMQP;

namespace SensorManagement.Microservice
{
    interface IMicroservice
    {
        void Init();
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
        void Shutdown();
    }
}
