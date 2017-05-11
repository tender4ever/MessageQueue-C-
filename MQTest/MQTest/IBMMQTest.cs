using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AUOMQ.IBMMQ;
using AUOMQ.AUOInterface;
using System.Threading;

namespace Project1
{
    class IBMMQTest : MQEventListener    //實作MQEventListener
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
        public void receiverAndCheckconnect()
        {
            AUOMQ.IBMMQ.IBMMQReceiver a = new IBMMQReceiver(this, "192.168.222.135");
        }

        //Sender
        public void sender(String textmessage)
        {
            AUOMQ.IBMMQ.IBMMQSender b = new IBMMQSender(this, "192.168.222.135");
            b.connect();
            b.send(textmessage);
        }
    }
}
