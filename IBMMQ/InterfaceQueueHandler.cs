using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AUOMQ.AUOInterface;

namespace AUOMQ.IBMMQ
{
    public class InterfaceQueueHandler : MQEventListener
    {
        public int receiveCount = 0;        //收MQ的數量
        public int sendCount = 0;           //送MQ的數量
        public String hostName;             //hostname
        public String channelName;          //channel Name
        public int port;                    //port
        public String queueName;            //queue Name

        private MQreceiver receiver;

        //建構子
        public InterfaceQueueHandler(String InputHostName, String InputChannelName,int InputPort,String InputQueueName) {
            this.hostName = InputHostName;
            this.channelName = InputChannelName;
            this.port = InputPort;
            this.queueName = InputQueueName;

            receiver = new IBMMQReceiver(this, hostName,channelName,port,queueName);

            Thread startrun = new Thread(run);
            startrun.Start();
        }
        //建構子
        public InterfaceQueueHandler()
        {
            receiver = new IBMMQReceiver(this);

            Thread startrun = new Thread(run);
            startrun.Start();
        }

        public void onMessage(String txtMessage)
        {
            System.Console.WriteLine(txtMessage + "已收到");
        }

        public void onConnect(String status)
        {
            System.Console.WriteLine(status);
        }

        public void run()
        {
            while (true)
            {
                if (receiver.getConnectionStatus() == "NotReady")
                {
                    receiver.connect();

                }
            }
        }
    }
}
