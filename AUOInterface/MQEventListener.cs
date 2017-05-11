using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging; 

namespace AUOMQ.AUOInterface
{
    public interface MQEventListener
    {
        void onMessage(String txtMessage);
        void systemMessage(String status);
    }
}
