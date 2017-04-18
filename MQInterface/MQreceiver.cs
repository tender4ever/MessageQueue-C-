using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace AUOMQ.AUOInterface
{
    public interface MQreceiver
    {
        void connect();
        void disconnect();
        void receive();
        String getConnectionStatus();
        void setConnectionStatus(String connectionStatus);
    }
}
