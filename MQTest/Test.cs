using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AUOMQ.MSMQ;
using AUOMQ.IBMMQ;
using AUOMQ.AUOInterface;
using System.Messaging;
using System.Threading;

namespace Project1
{
    class Test 
    {
        //MSMQ 
        public static List<AUOMQ.MSMQ.InterfaceQueueHandler> interfaceQueueList;
        public static void Main()
        {
            interfaceQueueList = new List<AUOMQ.MSMQ.InterfaceQueueHandler>();

            /* 開始receivee Message
             * new 帶入ip,queueName的IntefaceQueueHandler
            */
            AUOMQ.MSMQ.InterfaceQueueHandler a = new AUOMQ.MSMQ.InterfaceQueueHandler("192.168.222.135", "myqueue");

            /* new 不帶入ip,queueName的InterfaceQueueHandler
             * MQ.MSMQ.InterfaceQueueHandler a = new MQ.MSMQ.InterfaceQueueHandler();
            */

            interfaceQueueList.Add(a);

            //send Message
            //new帶入ip,queueName的MSMQSender
            //MQsender testsend = new MSMQSender("192.168.222.135","myqueue");

            //new不帶入ip,queueName的MSMQSender
            //MQsender testsend = new MSMQSender();
            for (int i = 1; i <= 2; i++) {
                MQsender testsend = new MSMQSender("192.168.222.135", "myqueue");
                testsend.connect();
                testsend.send("test~test");
                i = 0;
            }
                
            

        }
  
        /*
        public static List<MQ.IBMMQ.InterfaceQueueHandler> interfaceQueueList;
        static void Main() {
            interfaceQueueList = new List<MQ.IBMMQ.InterfaceQueueHandler>();
            for (int i = 1; i <= 2; i++) {
                MQ.IBMMQ.InterfaceQueueHandler a = new MQ.IBMMQ.InterfaceQueueHandler("192.168.222.135", "myqueue");

                interfaceQueueList.Add(a);

                a.send("testets");

                i = 0;
            }
        }*/
    }
}
