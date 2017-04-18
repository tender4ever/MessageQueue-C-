using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Threading;
using AUOMQ.AUOInterface;


namespace AUOMQ.MSMQ
{
    public class InterfaceQueueHandler : MQEventListener //implement MQEventListener
    {
        public int receiveCount = 0;        //收MQ的數量
        public int sendCount = 0;           //送MQ的數量
        public String hostName;             //hostName
        public String queueName;            //queueu Name

        private MQreceiver receiver;

        //建構子
        public InterfaceQueueHandler(String InputHostName, String InputQueueName) {
            this.hostName = InputHostName;
            this.queueName = InputQueueName;
            
            receiver = new MSMQReceiver(this,hostName, queueName);

            Thread startrun = new Thread(run);
            startrun.Start();
        }
        //建構子
        public InterfaceQueueHandler()
        {
            receiver = new MSMQReceiver(this);

            Thread startrun = new Thread(run);
            startrun.Start();
        }

        public void onMessage(String txtMessage) {
            System.Console.WriteLine(txtMessage + "已收到");

        }

        public void onConnect(String status) {
            System.Console.WriteLine(status);
        }

        public void run() {
            while (true) {
                if (receiver.getConnectionStatus() == "NotReady" ) {
                    receiver.connect();
                }

            }
        }
    }
}
