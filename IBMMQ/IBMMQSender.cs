using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBM.WMQ;
using AUOMQ.AUOInterface;

namespace AUOMQ.IBMMQ 
{
    /// <summary>
    /// IBMMQSender類別
    /// </summary>
    /// <remarks>
    /// 實作Interface MQsender
    /// </remarks>
    public class IBMMQSender : MQsender    
    {
        private String hostName = "127.0.0.1";                  // hostname
        private String channelName = "mychannel";               // channel Name
        private int port = 1414;                                // port
        private String connName;								// connection Name
        private String queueManagerName = "mq_app";             // queueManager Name
        private String queueName = "myqueue";                   // queue Name
        private String connectionStatus = "NotReady";           // 連線狀態

        private MQQueueManager queueManager;                    // 佇列管理程式
        private MQQueue sendqueue;                              // 建立send queue
        private MQPutMessageOptions queuePutMessage;            // put Message
        private MQMessage message;                              // 建立message
        private MQEventListener listener;                       // Listener


        /// <summary>
        /// IBMMQSender 建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="hostName"></param>
        /// <param name="channelName"></param>
        /// <param name="port"></param>
        /// <param name="queueManagerName"></param>
        /// <param name="queueName"></param>
        public IBMMQSender(MQEventListener eventlistener, String hostName, String channelName, int port, String queueManagerName, String queueName)
        {
            this.hostName = hostName;                           // 設定host Name
            this.channelName = channelName;                     // 設定channelName
            this.port = port;                                   // 設定port
            this.queueManagerName = queueManagerName;           // 設定queue Maneger Name
            this.queueName = queueName;                         // 設定queue Name
            MQEnvironment.Hostname = hostName;                  // 設定MQ的hostName
            MQEnvironment.Channel = channelName;                // 設定MQ的channelName
            MQEnvironment.Port = port;                          // 設定MQ的port
            listener = eventlistener;                           // Listener

        }

        /// <summary>
        /// IBMMQSender 建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="hostName"></param>
        /// <param name="channelName"></param>
        /// <param name="port"></param>
        public IBMMQSender(MQEventListener eventlistener, String hostName, String channelName, int port)
        {
            this.hostName = hostName;                           // 設定host Name
            this.channelName = channelName;                     // 設定channelName
            this.port = port;                                   // 設定port
            MQEnvironment.Hostname = hostName;                  // 設定MQ的hostName
            MQEnvironment.Channel = channelName;                // 設定MQ的channelName
            MQEnvironment.Port = port;                          // 設定MQ的port
            listener = eventlistener;                           // Listener

        }

        /// <summary>
        /// IBMMQSender 建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="hostName"></param>
        public IBMMQSender(MQEventListener eventlistener,String hostName) 
        {
            this.hostName = hostName;                           //設定hostname
            MQEnvironment.Hostname = hostName;                  //設定MQ的hostName
            MQEnvironment.Channel = channelName;                //設定MQ的channelName
            MQEnvironment.Port = port;                          //設定MQ的port
            listener = eventlistener;                           //Listener

        }

        /// <summary>
        /// IBMMQSender 建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        public IBMMQSender(MQEventListener eventlistener)
        {
            MQEnvironment.Hostname = hostName;                  //設定MQ的hostName
            MQEnvironment.Channel = channelName;                //設定MQ的channelName
            MQEnvironment.Port = port;                          //設定MQ的port
            listener = eventlistener;                           //Listener

        }

        /// <summary>
        /// connect method
        /// </summary>
        /// <remarks>連線</remarks>
        public void connect() {
            try
            {
				connName = hostName + "(" + port + ")";
				queueManager = new MQQueueManager(queueManagerName, channelName, connName);                         // MQ Queue Manager物件 遠端連線
                sendqueue = queueManager.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);      // send Queue
				queuePutMessage = new MQPutMessageOptions();														//put Message物件
				connectionStatus = "Ready";                                                                         // 設定連線狀態為Ready
				listener.systemMessage("sender connect ok");
				listener.connectMessage("sender connect");
			}
            catch (Exception e) {
                e.ToString();
                listener.systemMessage("sender connect fail  " + e.ToString ());                                    // sender 連線失敗
                disconnect();                                                                                       // 斷線
            }
        }

        /// <summary>
        /// disconnect method
        /// </summary>
        /// <remarks>斷線</remarks>
        public void disconnect() {
            try
            {
                sendqueue.Close();
				queueManager = null;
                sendqueue = null;
				queuePutMessage = null;
                connectionStatus = "NotReady";                                                                      // 設定連線狀態為NotReady
				listener.systemMessage("sender disconnect ok");
				listener.connectMessage("sender disconnect");
			}
            catch (Exception e) {

                listener.systemMessage("sender disconnect fail" + e.ToString());                                    // sender 斷線失敗
            }
        }

        /// <summary>
        /// send method
        /// </summary>
        /// <remarks>寄送message</remarks>
        /// <param name="InputText"></param>
        public void send(String InputText) {
            try
            {
				connect();

				if (connectionStatus == "Ready")
                {
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
				disconnect();
			}
        }
    }
}
