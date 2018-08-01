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
                            //Dictionary<int, byte[]> someList = ProcessInputBuffer(buffer);
                            /*ProcessInputBuffer(buffer);
                            switch ((int)buffer[1])
                            {
                                case 1:
                                    Ack(s1);
                                    break;
                                case 9:
                                    CtrlUsbHostResponse(s1);
                                    break;
                            }*/
                            CtrlDisplay(s1);
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

        

        private static void Ack(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.Ack.ByteSize];
            buf1 = EmcProtocol.Ack.Create();
            s1.Write(buf1, 0, buf1.Length);
        }
        //0 = Nothing, 1 = Read/Write cycle running, -1 if could not access memory stick
        private static void CtrlUsbHostResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.ControlUsbHostResponse.ByteSize];
            buf1 = EmcProtocol.ControlUsbHostResponse.CreateTestResponseMessage(1);
            s1.Write(buf1, 0, buf1.Length);
        }
        //0 = Currently in Normal UI, 1 = Currently in Test UI
        private static void CtrlDisplay(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.ControlDisplayResponse.ByteSize];
            buf1 = EmcProtocol.ControlDisplayResponse.CreateTestResponseMessage(1);
            s1.Write(buf1, 0, buf1.Length);
        }









        public static void ProcessInputBuffer(byte[] inputBuffer)
        {
            List<byte[]> patternList = new List<byte[]>();
            patternList.Add(EmcProtocol.Call.Create());
            patternList.Add(EmcProtocol.ControlUsbHost.StartUsbReadWrite());
            patternList.Add(EmcProtocol.ControlUsbHost.StopUsbReadWrite());

            foreach (var entry in patternList)
            {
                int resultPosition = ByteSearch(inputBuffer, entry, 0);
            }
            //return null;
        }
        private static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;
            bool matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0])
                    {
                        if (searchIn.Length > 1)
                        {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (int y = 1; y <= searchBytes.Length - 1; y++)
                            {
                                if (searchIn[i + y] != searchBytes[y])
                                {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched)
                            {
                                found = i;
                                break;
                            }

                        }
                        else
                        {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }

                    }
                }

            }
            return found;
        }
    }
}
