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
        private String hostName = "127.0.0.1";              //hostname
        private String channelName = "mychannel";           //channel Name
        private int port = 1414;                            //port
        private String queueManagerName = "mq_app";         //queue Manager Name
        private String queueName = "myqueue";               //queue Name
        private String connectionStatus = "NotReady";       //連線狀態

        private MQQueueManager queueManager;                //佇列管理程式
        private MQQueue receivequeue;                       //建立queue
        private MQGetMessageOptions queueGetMessage;        //get Message
        private MQMessage message;                          //建立message
        private MQEventListener listener;                   //Listener

        //建構子
        public IBMMQReceiver(MQEventListener eventlistener,String hostName,String channelName,int port ,String queueManegerName, String queueName)
        {
            this.hostName = hostName;                           //設定host Name
            this.channelName = channelName;                     //設定channel Name
            this.port = port;                                   //設定port
            this.queueManagerName = queueManegerName;           //設定queue Manager Name
            this.queueName = queueName;                         //設定queue Name
            MQEnvironment.Hostname = hostName;                  //設定MQ host Name
            MQEnvironment.Channel = channelName;                //設定MQ channel Name
            MQEnvironment.Port = port;                          //設定MQ port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }
        //建構子
        public IBMMQReceiver(MQEventListener eventlistener, String hostName, String channelName, int port)
        {
            this.hostName = hostName;                           //設定host Name
            this.channelName = channelName;                     //設定channel Name
            this.port = port;                                   //設定port
            MQEnvironment.Hostname = hostName;                  //設定MQ host Name
            MQEnvironment.Channel = channelName;                //設定MQ channel Name
            MQEnvironment.Port = port;                          //設定MQ port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }
        //建構子
        public IBMMQReceiver(MQEventListener eventlistener, String hostName)
        {
            this.hostName = hostName;                           //設定host Name
            MQEnvironment.Hostname = hostName;                  //設定MQ host Name
            MQEnvironment.Channel = channelName;                //設定MQ channel Name
            MQEnvironment.Port = port;                          //設定MQ port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }
        //建構子
        public IBMMQReceiver(MQEventListener eventlistener)
        {
            MQEnvironment.Hostname = hostName;                  //設定MQ host Name
            MQEnvironment.Channel = channelName;                //設定MQ channel Name
            MQEnvironment.Port = port;                          //設定MQ port
            listener = eventlistener;                           //Listener
            connect();                                          //連線
            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }
        //連線
        public void connect()
        {
            try
            {
                queueManager = new MQQueueManager(queueManagerName);                                                            //MQ Queue Manager物件
                receivequeue = queueManager.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);       //reveice Queue
                connectionStatus = "Ready";                                                                                     //設定連線狀態為Ready
                listener.systemMessage("receiver connect ok");
            }
            catch (Exception e)
            {
                e.ToString();
                listener.systemMessage("receiver connect fail");
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
                listener.systemMessage("receiver disconnect fail");
            }
        }

        public void receive() {
            while (true) {
                try
                {
                    if (connectionStatus == "Ready")
                    {
                        queueGetMessage = new MQGetMessageOptions();                    //get Message物件
                        message = new MQMessage();                                      //message物件
                        message.Format = MQC.MQFMT_STRING;                              //設定message的格式為string
                        receivequeue.Get(message, queueGetMessage);                     //receive message
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
                        listener.systemMessage("receiver Connection Status not Ready");
                        disconnect();
                    }
                }
                catch (Exception e) {
                    e.ToString();
                }
            }
        }
        public void checkconnect()
        {
            while (true)
            {
                if (connectionStatus == "NotReady")
                {
                    connect();
                }
            }
        }
    }
}
