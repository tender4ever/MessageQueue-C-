using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Project1
{
    class startprogram
    {

        public static void Main()
        {
            MSMQTest a = new MSMQTest();        //MSMQTest物件
            //IBMMQTest a = new IBMMQTest();        //IBMMQTest物件
            
            //Receivee Message
            a.receiverAndCheckconnect();

            //Send Message
            for (int i = 1; i <= 2; i++)
            {
                a.sender("test~~~Test");
                Thread.Sleep(500);
                i = 0;
            }

        }
    }
}
