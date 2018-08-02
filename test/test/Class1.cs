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
                SerialPort s1 = new SerialPort("COM15", 9600);
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
                            //CtrlDisplayResponse(s1);
                            //RequestLastKeyResponse(s1);
                            //DisplayStateResponse(s1);
                            //UsbHostStatusResponse(s1);
                            //CtrlUsbHostResponse(s1);
                            //UsbHostModemStatusResponse(s1);
                            Ack(s1);
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

        //0x?? <-- 0 = Nothing, 1 = Read/Write cycle running, -1 (2) if could not access memory stick
        //0x???????? <-- Error count in read/write cycle

        private static void UsbHostModemStatusResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.UsbHostModemStatus.ByteSize];
            buf1 = EmcProtocol.UsbHostModemStatus.CreateTestResponseMessage(1, 25);
            s1.Write(buf1, 0, buf1.Length);
        }

        //0 = Nothing, 1 = Read/Write cycle running, -1 if could not access memory stick
        private static void CtrlUsbHostModemResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.ControlUsbHostModemResponse.ByteSize];
            buf1 = EmcProtocol.ControlUsbHostModemResponse.CreateTestResponseMessage(2);
            s1.Write(buf1, 0, buf1.Length);
        }

        //0x?? <-- 0 = Nothing, 1 = Read/Write cycle running, -1 (2) if could not access memory stick
        //0x???????? <-- Error count in read/write cycle

        private static void UsbHostStatusResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.UsbHostStatus.ByteSize];
            buf1 = EmcProtocol.UsbHostStatus.CreateTestResponseMessage(2, 255);
            s1.Write(buf1, 0, buf1.Length);
        }

        private static void DisplayStateResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.DisplayState.ByteSize];
            buf1 = EmcProtocol.DisplayState.CreateTestResponseMessage(1);
            s1.Write(buf1, 0, buf1.Length);
        }

        private static void Ack(SerialPort s1)
        {
            //Console.Write(" incomming:");
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
            buf1 = EmcProtocol.ControlUsbHostResponse.CreateTestResponseMessage(2);
            s1.Write(buf1, 0, buf1.Length);
        }
        //0 = Currently in Normal UI, 1 = Currently in Test UI
        private static void CtrlDisplayResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.ControlDisplayResponse.ByteSize];
            buf1 = EmcProtocol.ControlDisplayResponse.CreateTestResponseMessage(0);
            s1.Write(buf1, 0, buf1.Length);
        }
        //0x?? <-- This will be the last key value as a char or 0 if no key have been seen yet.
        //0x?? <-- This will be either 0 (no key press seen yet), 1 (key pressed but not yet released), 2 (key pressed and released again)        
        private static void RequestLastKeyResponse(SerialPort s1)
        {
            Console.Write(" incomming:");
            //Console.WriteLine(s);

            byte[] buf1 = new byte[EmcProtocol.LastKeySeen.ByteSize];
            buf1 = EmcProtocol.LastKeySeen.CreateTestResponseMessage(0x58, 1);
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
