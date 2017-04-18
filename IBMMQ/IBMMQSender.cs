using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBM.WMQ;
using AUOMQ.AUOInterface;

namespace AUOMQ.IBMMQ 
{
    public class IBMMQSender : MQsender //實作
    {
        private String hostName = "192.168.222.135";    //hostname
        private String channelName = "mychannel";       //channel Name
        private int port = 1414;                        //port
        private String queueManagerName = "mq_app";     //queueManager Name
        private String queueName = "myqueue";           //queue Name

        private String connectionStatus = "NotReady";   //連線狀態

        private MQQueueManager queueManager;            //佇列管理程式
        private MQQueue sendqueue;                      //建立queue
        private MQPutMessageOptions queuePutMessage;    //put Message
        private MQMessage message;                      //建立message
        
        //建構子
        public IBMMQSender(String hostName) {
            this.hostName = hostName;
            MQEnvironment.Hostname = hostName;
            MQEnvironment.Channel = channelName;
            MQEnvironment.Port = port;

            connect();//連線
        }
        //建構子
        public IBMMQSender()
        {
            MQEnvironment.Hostname = hostName;
            MQEnvironment.Channel = channelName;
            MQEnvironment.Port = port;

            connect();//連線
        }

        //連線
        public void connect() {
            try
            {
                queueManager = new MQQueueManager(queueManagerName);
                sendqueue = queueManager.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);
                connectionStatus = "Ready";

            }
            catch (Exception e) {
                e.ToString();
                System.Console.WriteLine("connec fail");
                disconnect();
            }

        }

        //斷線
        public void disconnect() {
            try
            {
                sendqueue.Close();
                sendqueue = null;
                connectionStatus = "NotReady";
            }
            catch (Exception e) {
                e.ToString();
                System.Console.WriteLine("disconnec fail");
            }
        }

        public void send(String InputText) {
            try
            {
                if (connectionStatus == "Ready")
                {
                    queuePutMessage = new MQPutMessageOptions();

                    message = new MQMessage();
                    message.WriteString(InputText);
                    message.Format = MQC.MQFMT_STRING;
                    sendqueue.Put(message, queuePutMessage);

                    disconnect();
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

        public String getConnectionStatus()
        {
            return connectionStatus;
        }

        public void setConnectionStatus(String connectionStatus) {
            this.connectionStatus = connectionStatus;
        }
    }
}
