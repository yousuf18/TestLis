using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace Maglumi2000N
{
    public static class ESRROLLER20
    {
      
         private static StringBuilder sb = new StringBuilder();
        public const string MachineName = "ESRROLLER 20";
        private const int wait = 3000;
        private static SerialPort sp;
        private static EventWaitHandle wh;
        public static void StartReceiver(string comPort, int baudRate = 9600)
        {
            ESRROLLER20.sp = new SerialPort(comPort);
            ESRROLLER20.sp.BaudRate = baudRate;
           ESRROLLER20.sp.Parity = Parity.None;
          ESRROLLER20.sp.Handshake = Handshake.None;
            ESRROLLER20.sp.DataBits = 8;
            ESRROLLER20.sp.StopBits = StopBits.One;
            ESRROLLER20.sp.DataReceived += new SerialDataReceivedEventHandler(ESRROLLER20.sp_DataReceived);
            ESRROLLER20.sp.Open();
        }
        private static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ESRROLLER20.Eval(ESRROLLER20.sp.ReadExisting());
        }
        public static void Eval(string data)
        {
            Console.WriteLine("Data From Esr Roller 20 :"+data);
            //if(data==Constants.enq||data.StartsWith(Constants.enq))
            //{
            //    ESRROLLER20.sb.Clear();
            //    ESRROLLER20.sp.Write(Constants.ack);
            //}
             if(data.StartsWith(Constants.stx)||data==Constants.stx)
            {
                ESRROLLER20.sb.Clear();
                ESRROLLER20.sb.Append(data);
               
                if (data.EndsWith(Constants.etx))
                {

                    try
                    {
                        // ESRROLLER20.sb.Append(data);
                        using (StreamWriter streamWriter = new StreamWriter(Constants.DumpPath + "ESRROLLER20_" + DateTime.Now.Ticks.ToString() + ".txt"))
                            streamWriter.Write(ESRROLLER20.sb.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR (Write Error): " + ex.Message);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }

            }
            
            else if(data.EndsWith(Constants.etx))
            {
                //if (sb.ToString().Length > 100)
                //{
                    try
                    {
                    ESRROLLER20.sb.Append(data);
                        using (StreamWriter streamWriter = new StreamWriter(Constants.DumpPath + "ESRROLLER20_" + DateTime.Now.Ticks.ToString() + ".txt"))
                            streamWriter.Write(ESRROLLER20.sb.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR (Write Error): " + ex.Message);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                //}
            }
            // else if(data==Constants.enq)
            //{
            //    ESRROLLER20.sp.Write(Constants.ack);
           // }
            // else if(data==Constants.cr||data.EndsWith(Constants.cr))
            //{
            //    ESRROLLER20.sp.Write(Constants.ack);
            //}
            else
            {
                ESRROLLER20.sb.Append(data);
            }
        }
    }
    
}
