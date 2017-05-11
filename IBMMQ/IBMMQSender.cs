using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBM.WMQ;
using AUOMQ.AUOInterface;

namespace AUOMQ.IBMMQ 
{
    public class IBMMQSender : MQsender     //實作MQsender
    {
        private String hostName = "127.0.0.1";                  //hostname
        private String channelName = "mychannel";               //channel Name
        private int port = 1414;                                //port
        private String queueManagerName = "mq_app";             //queueManager Name
        private String queueName = "myqueue";                   //queue Name
        private String connectionStatus = "NotReady";           //連線狀態

        private MQQueueManager queueManager;                    //佇列管理程式
        private MQQueue sendqueue;                              //建立send queue
        private MQPutMessageOptions queuePutMessage;            //put Message
        private MQMessage message;                              //建立message
        private MQEventListener listener;                       //Listener

        //建構子
        public IBMMQSender(MQEventListener eventlistener, String hostName, String channelName, int port, String queueManagerName, String queueName)
        {
            this.hostName = hostName;                           //設定host Name
            this.channelName = channelName;                     //設定channelName
            this.port = port;                                   //設定port
            this.queueManagerName = queueManagerName;           //設定queue Maneger Name
            this.queueName = queueName;                         //設定queue Name
            MQEnvironment.Hostname = hostName;                  //設定MQ的hostName
            MQEnvironment.Channel = channelName;                //設定MQ的channelName
            MQEnvironment.Port = port;                          //設定MQ的port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
        }
        //建構子
        public IBMMQSender(MQEventListener eventlistener, String hostName, String channelName, int port)
        {
            this.hostName = hostName;                           //設定host Name
            this.channelName = channelName;                     //設定channelName
            this.port = port;                                   //設定port
            MQEnvironment.Hostname = hostName;                  //設定MQ的hostName
            MQEnvironment.Channel = channelName;                //設定MQ的channelName
            MQEnvironment.Port = port;                          //設定MQ的port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
        }
        //建構子
        public IBMMQSender(MQEventListener eventlistener,String hostName) 
        {
            this.hostName = hostName;                           //設定hostname
            MQEnvironment.Hostname = hostName;                  //設定MQ的hostName
            MQEnvironment.Channel = channelName;                //設定MQ的channelName
            MQEnvironment.Port = port;                          //設定MQ的port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
        }
        //建構子
        public IBMMQSender(MQEventListener eventlistener)
        {
            MQEnvironment.Hostname = hostName;                  //設定MQ的hostName
            MQEnvironment.Channel = channelName;                //設定MQ的channelName
            MQEnvironment.Port = port;                          //設定MQ的port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
        }

        //連線
        public void connect() {
            try
            {
                queueManager = new MQQueueManager(queueManagerName);                                                //MQ Queue Manager物件
                sendqueue = queueManager.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);      //send Queue
                connectionStatus = "Ready";                                                                         //設定連線狀態為Ready
            }
            catch (Exception e) {
                e.ToString();
                listener.systemMessage("sender connect fail");                                                      //sender 連線失敗
                disconnect();                                                                                       //斷線
            }
        }

        //斷線
        public void disconnect() {
            try
            {
                sendqueue.Close();
                sendqueue = null;
                connectionStatus = "NotReady";                                                                      //設定連線狀態為NotReady
            }
            catch (Exception e) {
                e.ToString();
                listener.systemMessage("sender disconnect fail");                                                   //sender 斷線失敗
            }
        }

        public void send(String InputText) {
            try
            {
                if (connectionStatus == "Ready")
                {
                    queuePutMessage = new MQPutMessageOptions();                                                    //put Message物件
                    message = new MQMessage();                                                                      //message物件
                    message.WriteString(InputText);                                                                 //把資料寫入message
                    message.Format = MQC.MQFMT_STRING;                                                              //設定message的格式為string
                    sendqueue.Put(message, queuePutMessage);                                                        //send message
                    disconnect();                                                                                   //斷線
                }
                else {
                    listener.systemMessage("sender Connection Status not Ready");                                   
                    disconnect();
                }
            }
            catch (Exception e) {
                e.ToString();
            }
        }
    }
}
