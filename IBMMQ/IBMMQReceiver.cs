using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBM.WMQ;
using AUOMQ.AUOInterface;
using System.Threading;

namespace AUOMQ.IBMMQ
{
    /// <summary>
    /// IBMMQReceiver類別
    /// </summary>
    /// <remarks>實作Interface MQreceiver</remarks>
    public class IBMMQReceiver : MQreceiver
    {
        private String hostName = "127.0.0.1";				// hostname
        private String channelName = "mychannel";           // channel Name
        private int port = 1414;                            // port
        private String connName;							// connection Name
        private String queueManagerName = "mq_app";         // queue Manager Name
        private String queueName = "myqueue";               // queue Name
        private String connectionStatus = "NotReady";       // 連線狀態

        private MQQueueManager queueManager;                // 佇列管理程式
        private MQQueue receivequeue;                       // 建立queue
        private MQGetMessageOptions queueGetMessage;        // get Message
        private MQMessage message;                          // 建立message
        private MQEventListener listener;                   // Listener

        /// <summary>
        /// IBMMQReceiver建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="hostName"></param>
        /// <param name="channelName"></param>
        /// <param name="port"></param>
        /// <param name="queueManegerName"></param>
        /// <param name="queueName"></param>
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


            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }

        /// <summary>
        /// IBMMQReceiver建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="hostName"></param>
        /// <param name="channelName"></param>
        /// <param name="port"></param>
        public IBMMQReceiver(MQEventListener eventlistener, String hostName, String channelName, int port)
        {
            this.hostName = hostName;                           //設定host Name
            this.channelName = channelName;                     //設定channel Name
            this.port = port;                                   //設定port
            MQEnvironment.Hostname = hostName;                  //設定MQ host Name
            MQEnvironment.Channel = channelName;                //設定MQ channel Name
            MQEnvironment.Port = port;                          //設定MQ port
            listener = eventlistener;                           //Listener


            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }

        /// <summary>
        /// IBMMQReceiver建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="hostName"></param>
        public IBMMQReceiver(MQEventListener eventlistener, String hostName)
        {
            this.hostName = hostName;                           //設定host Name
            MQEnvironment.Hostname = hostName;                  //設定MQ host Name
            MQEnvironment.Channel = channelName;                //設定MQ channel Name
            MQEnvironment.Port = port;                          //設定MQ port
            listener = eventlistener;                           //Listener


            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }

        /// <summary>
        /// IBMMQReceiver建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        public IBMMQReceiver(MQEventListener eventlistener)
        {
            MQEnvironment.Hostname = hostName;                  //設定MQ host Name
            MQEnvironment.Channel = channelName;                //設定MQ channel Name
            MQEnvironment.Port = port;                          //設定MQ port
            listener = eventlistener;                           //Listener


            Thread startreceive = new Thread(receive);          //宣告Thread來執行receive
            startreceive.Start();                               //開始執行Thread
            Thread checkConnect = new Thread(checkconnect);     //宣告Thread來執行checkconnect
            checkConnect.Start();                               //開始執行Thread
        }

        /// <summary>
        /// connect方法
        /// </summary>
        /// <remarks>連線</remarks>
        public void connect()
        {
            try
            {
				connName = hostName + "(" + port + ")";
				queueManager = new MQQueueManager(queueManagerName,channelName,connName);                                       // MQ Queue Manager物件 遠端連線
                receivequeue = queueManager.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);       // reveice Queue
				queueGetMessage = new MQGetMessageOptions();
				queueGetMessage.Options = MQC.MQGMO_WAIT;
				queueGetMessage.WaitInterval = 10000;
				connectionStatus = "Ready";                                                                                     // 設定連線狀態為Ready
				listener.systemMessage("receiver connect ok");
				listener.connectMessage("receiver connect");
			}
            catch (Exception e)
            {
                listener.systemMessage("receiver connect fail  " + e.ToString() );
                disconnect();
            }

        }

        /// <summary>
        /// disconnect方法
        /// </summary>
        /// <remarks>斷線</remarks>
        public void disconnect()
        {
            try
            {
                if (receivequeue != null)
                {
                    receivequeue.Close();
					queueManager = null;
					receivequeue = null;
					queueGetMessage = null;
                    connectionStatus = "NotReady";
					listener.systemMessage("receiver disconnect ok");
					listener.connectMessage("receiver disconnect");
				}
            }
            catch (Exception e)
            {
                listener.systemMessage("receiver disconnect fail" + e.ToString());
            }
        }

        /// <summary>
        /// receive方法
        /// </summary>
        /// <remarks>收message</remarks>
        public void receive() {

			int connectCount = 0;

			while (true) {

				try {

					if (connectionStatus == "Ready")
					{
						message = new MQMessage();                                      //message物件
						message.Format = MQC.MQFMT_STRING;                              //設定message的格式為string

						receivequeue.Get(message, queueGetMessage);                     //receive message
						String txt = message.ReadString(message.DataLength);

						connectCount = 0;

						if (listener != null)
						{
							try{
								listener.onMessage(txt);
							}
							catch (Exception e){

								e.Message.ToString();
							}
						}
					}
					else
					{
						listener.systemMessage("receiver Connection Status not Ready");
						disconnect();
						Thread.Sleep(5000);
					}
				}
				catch (Exception) {

					connectCount = connectCount + 1;

					if (connectCount > 2)
					{
						disconnect();
					}
					else {
						Thread.Sleep(5000);
					}
				}
			}
        }
        
        /// <summary>
        /// checkconnect方法
        /// </summary>
        /// <remarks>檢查連線狀態</remarks>
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
