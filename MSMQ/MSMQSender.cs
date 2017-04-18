using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;//須參考System.Message.dll
using System.Threading;
using AUOMQ.AUOInterface;


namespace AUOMQ.MSMQ
{
    public class MSMQSender : MQsender  //實作
    {
        //String queuePath = @".\private$\myqueue"; //呼叫本機MSMQ
        //String queuePath = @"FormatName:Direct=TCP:192.168.222.135\private$\myqueue"; //使用IP呼叫遠端MSMQ
        //String queuePath = @"FormatName:Direct=OS:STKC\private$\myqueue"; //使用Machine Name呼叫遠端MSMQ

        private String hostName = "192.168.222.135";        //hostName 
        private String queueName = "myqueue";               //queueName
        private String queuePath = null;                    //queuePath
        private String connectionStatus = "NotReady";       //連線狀態
        private MessageQueue myQueue;                       //宣告Queue

        //建構子，帶入參數hostName,QueueName
        public MSMQSender(String InputHostName, String InputQueueName) {
            this.hostName = InputHostName;
            this.queueName = InputQueueName;
            
            connect();//連線
        }

        //建構子
        public MSMQSender()
        {
            connect();//連線
        }
        
        //連線
        public void connect(){
            try
            {
                queuePath = @"FormatName:Direct=TCP:" + hostName + @"\private$\" + queueName;   //設定queuePath

                myQueue = new MessageQueue(queuePath);  //設定MessageQueue

                connectionStatus = "Ready"; //設連線狀態為Ready

            }
            catch (Exception e) 
            {
                e.ToString();
                System.Console.WriteLine("connect fail");
                disconnect();//斷線
            }
        }
        
        //斷線
        public  void disconnect() {
            try
            {
                 myQueue.Close();
                 myQueue = null;

                 connectionStatus = "NotReady"; //設連線狀態為NotReady
            }
            catch (Exception e)
            {
                e.ToString();
                System.Console.WriteLine("disconnect fail");
            }
        }

        
        public void send(String InputText){
            try
            {
                if (connectionStatus == "Ready")    //假設連線狀態為Ready
                {
                    myQueue.Send(InputText);        //送出message

                    disconnect();                   //斷線
                }
                else 
                {
                    System.Console.WriteLine("Connection Status not Ready");
                    disconnect();                   //斷線
                }
            }
            catch (Exception e)
            {
                e.ToString();
                disconnect();                       //斷線
            }
           
        }

        public String getConnectionStatus() {
            return connectionStatus;
        }

        public void setConnectionStatus(String connectionStatus) {
            this.connectionStatus = connectionStatus;
        }
    }
}
