using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AUOMQ.MSMQ;
using AUOMQ.IBMMQ;
using AUOMQ.AUOInterface;
using System.Threading;

namespace Project1
{
    class MSMQTest : MQEventListener    //實作MQEventListener
    {
        //實作MQEventListener的onMessage Function
        public void onMessage(String txtMessage)
        {
            System.Console.WriteLine(txtMessage);
        }

        //實作MQEventListener的systemMessage Function
        public void systemMessage(String status)
        {
            System.Console.WriteLine(status);
        }

        //Receiver
        public void receiverAndCheckconnect() {
            AUOMQ.MSMQ.MSMQReceiver a = new MSMQReceiver(this,"192.168.222.135");
        }

        //Sender
        public void sender(String textmessage) {
            AUOMQ.MSMQ.MSMQSender b = new MSMQSender(this, "192.168.222.135");
            b.connect();
            b.send(textmessage);
        }
    }
}
