using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;//須參考System.Message.dll
using IBM.WMQ;
using AUOMQ.AUOInterface;

namespace AUOMQ.IBMMQ
{
	/// <summary>
	/// IBMMQueue 類別
	/// </summary>
	/// <remarks>實作Interface MQqueue</remarks>
	public class IBMMQQueue : MQqueue
	{
		private MQEventListener listener;               // MQEventListener

		private MQQueueManager queueManager = null;     // 佇列管理程式

		IBM.WMQ.PCF.PCFMessageAgent agent = null;		// Message Agent

		private String queueManagerName = "mq_app";			// Queue Manager
		private String channelName = "mychannel";			// Channel Name
		private String connectString = "127.0.0.1(1414)";	// IP(Port)

		/// <summary>
		/// IBMMQueue 建構子
		/// </summary>
		/// <param name="eventlistener"></param>
		/// <param name="queueManager"></param>
		/// <param name="channel"></param>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		public IBMMQQueue(MQEventListener eventlistener, String queueManager, String channel, String ip, String port) {

			listener = eventlistener;

			queueManagerName = queueManager;
			channelName = channel;
			connectString = ip + "(" + port + ")";
		}

		/// <summary>
		/// IBMMQueue 建構子
		/// </summary>
		/// <param name="eventlistener"></param>
		/// <param name="channel"></param>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		public IBMMQQueue(MQEventListener eventlistener, String channel, String ip, String port) {

			listener = eventlistener;

			channelName = channel;
			connectString = ip + "(" + port + ")";
		}

		/// <summary>
		/// IBMMQueue 建構子
		/// </summary>
		/// <param name="eventlistener"></param>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		public IBMMQQueue(MQEventListener eventlistener, String ip, String port) {

			listener = eventlistener;

			connectString = ip + "(" + port + ")";
		}

		/// <summary>
		/// IBMMQueue 建構子
		/// </summary>
		/// <param name="eventlistener"></param>
		public IBMMQQueue(MQEventListener eventlistener) {

			listener = eventlistener;
		}

		/// <summary>
		/// createQueue 方法
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public bool createQueue(String queueName) {

			// Queue Manager
			queueManager = new MQQueueManager(queueManagerName, channelName, connectString);

			// Message Agent
			agent = new IBM.WMQ.PCF.PCFMessageAgent(queueManager);

			// PCF Message
			IBM.WMQ.PCF.PCFMessage request = new IBM.WMQ.PCF.PCFMessage(IBM.WMQ.PCF.CMQCFC.MQCMD_CREATE_Q);

			request.AddParameter(IBM.WMQ.MQC.MQCA_Q_NAME, queueName);
			request.AddParameter(IBM.WMQ.MQC.MQIA_Q_TYPE, IBM.WMQ.MQC.MQQT_LOCAL);

			try
			{
				agent.Send(request);

				listener.systemMessage("Create Queue Sucess");
				return true;
			}
			catch (Exception) {

				listener.systemMessage("Create Queue Fail");
				return false;
			}
		}

		/// <summary>
		/// deleteQueue 方法
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public bool deleteQueue(String queueName) {

			// Queue Manager
			queueManager = new MQQueueManager(queueManagerName, channelName, connectString);

			// Message Agent
			agent = new IBM.WMQ.PCF.PCFMessageAgent(queueManager);

			// PCF Message
			IBM.WMQ.PCF.PCFMessage request = new IBM.WMQ.PCF.PCFMessage(IBM.WMQ.PCF.CMQCFC.MQCMD_DELETE_Q);

			request.AddParameter(IBM.WMQ.MQC.MQCA_Q_NAME, queueName);

			try
			{
				agent.Send(request);

				listener.systemMessage("Delete Queue Sucess");
				return true;
			}
			catch (Exception) {

				listener.systemMessage("Delete Queue Fail");
				return false;
			}
		}
	}
}
