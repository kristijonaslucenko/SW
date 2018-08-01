using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace serialConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //open serial port with given arguments COM and baudrate
                SerialPort s1 = new SerialPort("COM5", 115200);
                s1.ReadTimeout = 200;
                s1.Open();
                // Console.WriteLine(args[0].ToString());

                

                while (true)
                {
                    // Byte[] buffer1 = EmcProtocol.RequestLastSeenKey.Create();
                    // s1.Write(buffer1, 0, buffer1.Length);

                    /*int bToRead = s1.BytesToRead;
                    if (bToRead > 0)
                    {
                        //read input to console and format it to readable encoding
                        byte[] buffer = new byte[bToRead];
                        s1.Read(buffer, 0, buffer.Length);
                        string s = Encoding.GetEncoding("Windows-1252").GetString(buffer);
                        //Console.Write(args[0].ToString());
                        Console.Write(" incomming:");
                        Console.WriteLine(s);
                    }*/


                    try
                    {
                        int bToRead = s1.BytesToRead;
                        if (bToRead >= 7)
                        {
                            byte[] buffer = new byte[bToRead];
                            s1.Read(buffer, 0, bToRead);
                            Dictionary<int, byte[]> someList = ProcessInputBuffer(buffer);

                            switch ((int)buffer[1])
                            {
                                case 1:
                                    Ack(s1);
                                    break;
                                case 9:
                                    CtrlUsbHostResponse(s1);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Error: " + ex.ToString(), "ERROR");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Dictionary<int, byte[]> ProcessInputBuffer(byte[] inputBuffer)
        {
            Dictionary<int, byte[]> patternList = new Dictionary<int, byte[]>();
            patternList.Add(0, EmcProtocol.Call.Create());
            patternList.Add(1, EmcProtocol.ControlUsbHost.StartUsbReadWrite());
            patternList.Add(2, EmcProtocol.ControlUsbHost.StopUsbReadWrite());

            foreach (var entry in patternList)
            {
                var pattern = entry.Value;
                IEnumerable<int> gg = PatternAt(inputBuffer, pattern);
            }
            return null;
        }
        public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }

        private static void Ack(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.Ack.ByteSize];
            buf1 = EmcProtocol.Ack.Create();
            s1.Write(buf1, 0, buf1.Length);
        }

        private static void CtrlUsbHostResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.ControlUsbHostResponse.ByteSize];
            buf1 = EmcProtocol.ControlUsbHostResponse.CreateTestResponseMessage(1);
            s1.Write(buf1, 0, buf1.Length);
        }
    }
}
