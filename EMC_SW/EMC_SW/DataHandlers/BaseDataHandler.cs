using EMC_SW.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EMC_SW.GenConstants;
using EmcProtocol;

namespace EMC_SW.DataHandlers
{
    public delegate void SampleRateUpdatedEventHandler(object sender, SampleRateUpdatedEventArgs e);

    public class SampleRateUpdatedEventArgs : EventArgs
    {
        public SampleRateUpdatedEventArgs(int sampleRate)
        {
            DataRate = sampleRate;
        }
        public int DataRate { get; set; }
    }

    public abstract class BaseDataHandler
    {
       public TransmissionRecord TransmissionResults { get; set; }


        public BaseDataHandler()
        {
            TransmissionResults = new TransmissionRecord();
        }

        protected abstract byte[] ReadData(int BufferSize, out bool timeout);
        protected abstract void WriteData(byte[] buffer);

        public virtual void Open(String port, int baudrate)
        {
            //stopped = false;
            //readThread = new Thread(DoRead);
            //readThread.Start();
            //writeThread = new Thread(DoWrite);
            //writeThread.Start();

        }

        public virtual void Close()
        {
            //stopped = true;
        }

        public virtual void SendingData(byte[] buffer)
        {
            WriteData(buffer);
        }

        public virtual byte[] ReadingData(int BufferSize, out bool timeout)
        {
           return ReadData(BufferSize, out timeout);
        }
        //                string s = Encoding.GetEncoding("Windows-1252").GetString(ReadData());


        private bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        private byte[] Decode(byte[] packet)
        {
            var i = packet.Length - 1;
            while (packet[i] == 0)
            {
                --i;
            }
            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
            return temp;
        }
    }
}
