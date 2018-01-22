using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;     //須參考System.Message.dll
using System.Threading;
using AUOMQ.AUOInterface;


namespace AUOMQ.MSMQ
{
    /// <summary>
    /// MSMQSender類別
    /// </summary>
    /// <remarks>實作Interface MQsender</remarks>
    public class MSMQSender : MQsender 
    {
        /*===================================================================================================================*/
        /*  MSMQ queuePath設定                                                                                               */
        /*  String queuePath = @".\private$\myqueue"; //呼叫本機MSMQ                                                         */
        /*  String queuePath = @"FormatName:Direct=TCP:192.168.222.135\private$\myqueue"; //使用IP呼叫遠端MSMQ               */
        /*  String queuePath = @"FormatName:Direct=OS:STKC\private$\myqueue"; //使用Machine Name呼叫遠端MSMQ                 */
        /*===================================================================================================================*/

        private String hostName = "127.0.0.1";                          //hostName 
        private String queueName = "myqueue";                           //queueName
        private String queuePath = null;                                //queuePath
        private String connectionStatus = "NotReady";                   //連線狀態
        private MessageQueue myQueue;                                   //宣告Queue
        private MQEventListener listener;                               //MQEventListener

        /// <summary>
        /// MSMQSender建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="InputHostName"></param>
        /// <param name="InputQueueName"></param>
        public MSMQSender(MQEventListener eventlistener,String InputHostName, String InputQueueName) 
        {
            this.hostName = InputHostName;                              //設定hostName
            this.queueName = InputQueueName;                            //設定queueName
            listener = eventlistener;                                   //MQEventListener
            connect();                                                  //連線
        }

        /// <summary>
        /// MSMQSender建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        /// <param name="InputHostName"></param>
        public MSMQSender(MQEventListener eventlistener, String InputHostName)
        {
            this.hostName = InputHostName;                              //設定hostName
            listener = eventlistener;                                   //MQEventListener
            connect();                                                  //連線
        }

        /// <summary>
        /// MSMQSender建構子
        /// </summary>
        /// <param name="eventlistener"></param>
        public MSMQSender(MQEventListener eventlistener)
        {
            listener = eventlistener;                                   //MQEventListener
            connect();                                                  //連線
        }
        
        /// <summary>
        /// connect方法
        /// </summary>
        /// <remarks>連線</remarks>
        public void connect(){
            try
            {
                queuePath = @"FormatName:Direct=TCP:" + hostName + @"\private$\" + queueName;       //設定queuePath
                myQueue = new MessageQueue(queuePath);                                              //設定MessageQueue
                connectionStatus = "Ready";                                                         //設連線狀態為Ready
				listener.systemMessage("sender connect ok");
				listener.connectMessage("sender connect");
			}
			catch (Exception e) 
            {
                e.ToString();
                listener.systemMessage("sender connect fail");                                     //輸出文字 連線失敗
                disconnect();                                                                      //斷線
            }
        }
        
        /// <summary>
        /// disconnect方法
        /// </summary>
        /// <remarks>斷線</remarks>
        public  void disconnect() {
            try
            {
                myQueue.Close();
                myQueue = null;
                connectionStatus = "NotReady";                                                     //設連線狀態為NotReady
                listener.systemMessage("sender disconnect ok");
				listener.connectMessage("sender disconnect");
            }
            catch (Exception e)
            {
                e.ToString();
                listener.systemMessage("sender disconnect fail");                                   //輸出文字 斷線失敗
            }
        }
        
        /// <summary>
        /// send方法
        /// </summary>
        /// <remarks>寄送message</remarks>
        /// <param name="InputText"></param>
        public void send(String InputText){
            try
            {
                if (connectionStatus == "Ready")
                {
                    myQueue.Send(InputText);                                                        //送出message
                    disconnect();                                                                   //斷線
                }
                else 
                {
                    listener.systemMessage("sender Connection Status not Ready");
                    disconnect();                                                                   //斷線
                }
            }
            catch (Exception e)
            {
                e.ToString();
                disconnect();                                                                       //斷線
            }
           
        }
    }
}
