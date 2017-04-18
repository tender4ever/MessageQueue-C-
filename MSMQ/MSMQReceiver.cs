using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;//須參考System.Message.dll
using System.Threading;
using AUOMQ.AUOInterface;


namespace AUOMQ.MSMQ
{
    public class MSMQReceiver : MQreceiver  //implement MQreceiver
    {
        //String queuePath = @".\private$\myqueue"; //呼叫本機MSMQ
        //String queuePath = @"FormatName:Direct=TCP:192.168.222.135\private$\myqueue"; //使用IP呼叫遠端MSMQ
        //String queuePath = @"FormatName:Direct=OS:STKC\private$\myqueue"; //使用Machine Name呼叫遠端MSMQ

        private String hostName = "192.168.222.135";        //hostName 
        private String queueName = "myqueue";               //queueName
        private String connectionStatus = "NotReady";       //連線狀態
        private MessageQueue myQueue;                       //MessageQueue
        private MQEventListener listener;                   //MQEventListener


        //建構子，帶入參數hostName,queueName
        public MSMQReceiver(MQEventListener eventlistener, String InputHostName, String InputQueueName){

            listener = eventlistener;
            this.hostName = InputHostName;
            this.queueName = InputQueueName;
            
            connect();  //連線
            Thread startreceive = new Thread(receive);  //宣告Thread來執行無窮迴圈的receive
            startreceive.Start();   //執行Thread

        }
        //建構子
        public MSMQReceiver(MQEventListener eventlistener)
        {
            listener = eventlistener;

            connect();  //連線
            Thread startreceive = new Thread(receive);  //宣告Thread來執行無窮迴圈的receive
            startreceive.Start();   //執行Thread

        }
        //連線
        public void connect()
        {
            try
            {
                //設定queuePath
                String queuePath = @"FormatName:Direct=TCP:" + hostName + @"\private$\" + queueName;
                /*
                //僅適用於本機端，本機端檢查queue是否存在
                if (!MessageQueue.Exists(@".\private$\" + queueName))
                {
                    myQueue = MessageQueue.Create(@".\private$\" + queueName);
                }
                else 
                {
                    //設定MessageQueue
                    myQueue = new MessageQueue(queuePath);
                }*/

                //設定MessageQueue
                myQueue = new MessageQueue(queuePath);
                connectionStatus = "Ready";

                listener.onConnect("receiver connect ok");

            }
            catch (Exception e)
            {
                e.ToString();
                System.Console.WriteLine("connect fail");
                disconnect();//斷線
            }
        }

        //斷線
        public void disconnect()
        {
            try
            {
                myQueue.Close();
                myQueue = null;

                connectionStatus = "NotReady";
            }
            catch (Exception e)
            {
                e.ToString();
                System.Console.WriteLine("disconnect fail");
            }
        }

        public void receive()
        {
            while (true)
            {
                try
                {
                    if (connectionStatus == "Ready")
                    {
                        myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(String) });

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
                    else
                    {
                        System.Console.WriteLine("Connection Status not Ready");
                        disconnect();
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                    disconnect();
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
