using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;		//須參考System.Message.dll
using System.Threading;
using AUOMQ.AUOInterface;


namespace AUOMQ.MSMQ
{
    /// <summary>
    /// MSMQReceiver 類別
    /// </summary>
    /// <remarks>實作Interface MQreceiver</remarks>
    public class MSMQReceiver : MQreceiver 
    {
        /*===================================================================================================================*/
        /*  MSMQ queuePath設定                                                                                               */

        /*  String queuePath = @".\private$\myqueue"; //呼叫本機MSMQ                                                         */
        /*  String queuePath = @"FormatName:Direct=TCP:192.168.222.135\private$\myqueue"; //使用IP呼叫遠端MSMQ               */
        /*  String queuePath = @"FormatName:Direct=OS:STKC\private$\myqueue"; //使用Machine Name呼叫遠端MSMQ                 */
        /*===================================================================================================================*/

        private String hostName = "127.0.0.1";			// hostName 
        private String queueName = "myqueue";           // queueName
        private String connectionStatus = "NotReady";   // 連線狀態
        private MessageQueue myQueue;                   // MessageQueue
        private MQEventListener listener;               // MQEventListener

		private Thread startreceive;					// 收 Message 的執行緒
		private Thread checkConnect;					// Check 連線狀態的執行緒

		/// <summary>
		/// MSMQReceiver建構子
		/// </summary>
		/// <param name="eventlistener"></param>
		/// <param name="InputHostName"></param>
		/// <param name="InputQueueName"></param>
		public MSMQReceiver(MQEventListener eventlistener, String InputHostName, String InputQueueName)
        {
            this.hostName = InputHostName;                              //設定hostName 
            this.queueName = InputQueueName;                            //設定queueName
            listener = eventlistener;                                   //MQEventListener
            connect();                                                  //連線

            startreceive = new Thread(receive);							//宣告Thread來執行無窮迴圈的receive
            startreceive.Start();                                       //執行Thread

            checkConnect = new Thread(checkconnect);					//宣告Thread來執行無窮迴圈的checkConnect
            checkConnect.Start();                                       //執行Thread
        }

        /// <summary>
        /// MSMQReceiver建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="InputHostName"></param>
        public MSMQReceiver(MQEventListener eventlistener, String InputHostName)
        {
            this.hostName = InputHostName;                              //設定hostName 
            listener = eventlistener;                                   //MQEventListener
            connect();                                                  //連線

			startreceive = new Thread(receive);                         //宣告Thread來執行無窮迴圈的receive
			startreceive.Start();                                       //執行Thread

			checkConnect = new Thread(checkconnect);                    //宣告Thread來執行無窮迴圈的checkConnect
			checkConnect.Start();                                       //執行Thread
		}

        /// <summary>
        /// MSMQReceiver建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        public MSMQReceiver(MQEventListener eventlistener)
        {
            listener = eventlistener;                                   //MQEventListener
            connect();                                                  //連線

			startreceive = new Thread(receive);                         //宣告Thread來執行無窮迴圈的receive
			startreceive.Start();                                       //執行Thread

			checkConnect = new Thread(checkconnect);                    //宣告Thread來執行無窮迴圈的checkConnect
			checkConnect.Start();                                       //執行Thread
		}

        /// <summary>
        /// connect方法
        /// </summary>
        /// <remarks>連線</remarks>
        public void connect()
        {
            try
            {
                //設定 queuePath
                String queuePath = @"FormatName:Direct=TCP:" + hostName + @"\private$\" + queueName;

				//設定 MessageQueue
				myQueue = new MessageQueue(queuePath);

				connectionStatus = "Ready";

				listener.systemMessage("receiver connect ok");
				listener.connectMessage("receiver connect");

				// 清空 Queue 裡的資料
				myQueue.Purge();
			}
            catch (Exception e)
            {
                e.ToString();
                listener.systemMessage("receiver connect fail");
                disconnect();//斷線
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
                myQueue.Close();
                myQueue = null;

                connectionStatus = "NotReady";

                listener.systemMessage("receiver disconnect ok");
				listener.connectMessage("receiver disconnect");
			}
            catch (Exception e)
            {
                e.ToString();
                listener.systemMessage("receiver disconnect fail");
            }
        }

        /// <summary>
        /// receive方法
        /// </summary>
        /// <remarks>收message</remarks>
        public void receive()
        {
			// 重新連線次數
			int connectCount = 0;

            while (true) {

				try {
					if (connectionStatus == "Ready") {

						// 檢查 Queue是否沒有 Message
						if (isQueryEmpty() == true) {

							connectCount = connectCount + 1;

							if (connectCount > 3) {

								listener.systemMessage("Connect Count : " + connectCount);
								listener.connectMessage("receiver disconnect");
								connectionStatus = "NotReady";
								continue;
							}

							listener.systemMessage("Connect Count : " + connectCount);
							continue;
						}

						// 檢察 Queue 有 Message
						connectCount = 0;

						myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });     //設定message的格式為string
						Message message = myQueue.Receive();
						String txt = (String)message.Body;

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
						Thread.Sleep(3000);
					}
				}
				catch (Exception e) {

					e.ToString();
					disconnect();
				}

			}
        }

		/// <summary>
		/// isQueryEmpty 方法
		/// </summary>
		/// <remarks>檢查 Queue 是否有 Message</remarks>
		/// <returns></returns>
		private bool isQueryEmpty() {

			try
			{
				TimeSpan timeout = new TimeSpan(0, 0, 10);	// 等待10秒的時間
				myQueue.Peek(timeout);	//再10秒內查看是否有 Message
				return false;
			}
			catch (Exception e) {
				listener.systemMessage(e.ToString());
				return true;
			}
		}
        
        /// <summary>
        /// checkconnect方法
        /// </summary>
        /// <remarks>檢查連線狀態</remarks>
        public void checkconnect() {
            while (true) {
                if (connectionStatus == "NotReady") {
					
                    connect();

				}
            }
        }
    }
}
