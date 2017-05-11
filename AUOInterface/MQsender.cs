using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace AUOMQ.AUOInterface
{
    public interface MQsender
    {
        void connect();
        void disconnect();
        void send(String InputText);
    }
}
