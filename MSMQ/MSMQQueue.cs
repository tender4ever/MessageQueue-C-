using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;//須參考System.Message.dll
using AUOMQ.AUOInterface;

namespace AUOMQ.MSMQ
{
	/// <summary>
	/// MSMQQueue類別
	/// </summary>
	/// <remarks>實作Interface MQqueue</remarks>
	public class MSMQQueue : MQqueue
	{
		private MQEventListener listener;               // MQEventListener

		/// <summary>
		/// MSMQQueue 建構子
		/// </summary>
		/// <param name="eventlistener"></param>
		public MSMQQueue(MQEventListener eventlistener) {
			listener = eventlistener;
		}

		/// <summary>
		/// createQueue 方法
		/// </summary>
		/// <remarks>建立Queue</remarks>
		/// <param name="queueName"></param>
		public bool createQueue(String queueName)
		{

			// 本機端queuePath設定
			string queuePath = @".\private$\" + queueName;

			//判斷queuePath是否存在
			if (MessageQueue.Exists(queuePath))
			{
				listener.systemMessage("Create Queue Fail");
				return false;
			}
			else {

				//建立新的佇列
				MessageQueue.Create(queuePath);
				listener.systemMessage("Create Queue Sucess");
				return true;
			}

		}

		/// <summary>
		/// deleteQueue 方法
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public bool deleteQueue(String queueName) {

			// 本機端queuePath設定
			string queuePath = @".\private$\" + queueName;

			//判斷queuePath是否存在
			if (MessageQueue.Exists(queuePath))
			{
				//建立新的佇列
				MessageQueue.Delete(queuePath);
				listener.systemMessage("Delete Queue Sucess");
				return true;
			}
			else
			{
				listener.systemMessage("Delete Queue Fail");
				return false;
			}
		}
	}
}
