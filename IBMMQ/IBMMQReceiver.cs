using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBM.WMQ;
using AUOMQ.AUOInterface;
using System.Threading;

namespace AUOMQ.IBMMQ
{
    public class IBMMQReceiver : MQreceiver
    {
        private String hostName = "192.168.222.135";    //hostname
        private String channelName = "mychannel";       //channel Name
        private int port = 1414;                        //port
        private String queueManagerName = "mq_app";     //queueManager Name
        private String queueName = "myqueue";           //queue Name

        private String connectionStatus = "NotReady";   //連線狀態

        private MQQueueManager queueManager;            //佇列管理程式
        private MQQueue receivequeue;                      //建立queue
        private MQGetMessageOptions queueGetMessage;    //put Message
        private MQMessage message;                      //建立message

        private MQEventListener listener;

        //建構子
        public IBMMQReceiver(MQEventListener eventlistener,String hostName,String channelName,int port ,String queueName)
        {
            this.hostName = hostName;
            this.channelName = channelName;
            this.port = port;
            this.queueName = queueName;
            MQEnvironment.Hostname = hostName;
            MQEnvironment.Channel = channelName;
            MQEnvironment.Port = port;

            listener = eventlistener;

            connect();//連線
            Thread startreceive = new Thread(receive);
            startreceive.Start();
        }
        //建構子
        public IBMMQReceiver(MQEventListener eventlistener)
        {
            MQEnvironment.Hostname = hostName;
            MQEnvironment.Channel = channelName;
            MQEnvironment.Port = port;

            listener = eventlistener;

            connect();//連線
            Thread startreceive = new Thread(receive);
            startreceive.Start();
        }
        //連線
        public void connect()
        {
            try
            {
                queueManager = new MQQueueManager(queueManagerName);
                receivequeue = queueManager.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);
                connectionStatus = "Ready";

                listener.onConnect("receiver connect ok");
            }
            catch (Exception e)
            {
                e.ToString();
                System.Console.WriteLine("connect fail");
                disconnect();
            }

        }
        //斷線
        public void disconnect()
        {
            try
            {
                receivequeue.Close();
                receivequeue = null;
                connectionStatus = "NotReady";
            }
            catch (Exception e)
            {
                e.ToString();
                System.Console.WriteLine("disconnect fail");
            }
        }

        public void receive() {
            while (true) {
                try
                {
                    if (connectionStatus == "Ready")
                    {
                        queueGetMessage = new MQGetMessageOptions();

                        message = new MQMessage();
                        message.Format = MQC.MQFMT_STRING;
                        receivequeue.Get(message, queueGetMessage);

                        String txt = message.ReadString(message.DataLength);

                        if (listener != null)
                        {
                            try
                            {
                                listener.onMessage(txt);
                            }
                            catch (Exception e)
                            {
                                e.Message.ToString();
                            }
                        }
                    }
                    else {
                        System.Console.WriteLine("Connection Status not Ready");
                        disconnect();
                    }
                }
                catch (Exception e) {
                    e.ToString();
                }
            }
        }

        public String getConnectionStatus()
        {
            return connectionStatus;
        }

        public void setConnectionStatus(String connectionStatus)
        {
            this.connectionStatus = connectionStatus;
        }
    }
}
