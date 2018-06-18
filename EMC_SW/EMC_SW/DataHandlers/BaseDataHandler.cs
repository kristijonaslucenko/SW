using EMC_SW.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EMC_SW.GenConstants;

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
        private byte inc = 0;
        private bool stopped;

        private Thread readThread;
        private Thread writeThread;

        public Sample ReadSample { get; set; }
        public Sample SentSample { get; set; }
        public Sample ResultsSample { get; set; }

        public int SampleRate { get; private set; }


        public BaseDataHandler()
        {
            ReadSample = new Sample();
            SentSample = new Sample();
            ResultsSample = new Sample();
        }

        protected abstract byte[] ReadData();
        protected abstract void WriteData(byte[] buffer);

        public virtual void Open(String port)
        {
            stopped = false;
            readThread = new Thread(DoRead);
            readThread.Start();
            writeThread = new Thread(DoWrite);
            writeThread.Start();

        }

        public virtual void Close()
        {
            stopped = true;
        }

        public virtual void SendingData(byte[] buffer)
        {
            WriteData(buffer);
        }

        private void DoRead()
        {
            while (!stopped)
            {
                byte[] value = Decode(ReadData());

                //sampleCounter++;
                if (value != null)
                {
                    //ReadSample.Enqueue(value);
                    ProcessResults(value);
                }
                //Debug.Print(value.ToString());
            }
        }

        private void ProcessResults(byte[] ReceivedData)
        {
            int StartInd = -1;
            byte[] ProcessedPacket = new byte[GenConstants.GenConstants.PacketSize];
            //List<byte[]> PacketList = new List<byte[]>();

            for (int i = 0; i < ReceivedData.Length; i++)
            {
                if (ReceivedData[i] == GenConstants.GenConstants.StartByte && ReceivedData[i + (GenConstants.GenConstants.PacketSize - 1)] == GenConstants.GenConstants.StopByte)
                {
                    //we know it's a packet
                    StartInd = i;
                    //Console.WriteLine("StartByte {0}", i);
                    Array.Copy(ReceivedData, StartInd, ProcessedPacket, 0, 3);
                    //PacketList.Add(ProcessedPacket);
                    ReadSample.Enqueue(ProcessedPacket);
                    QueueProcessing();
                }
            }

        }

        private void QueueProcessing()
        {
            for(int i = 0; i < ReadSample.CountEntries(); i++)
            {
                for(int a = 0; a < SentSample.CountEntries(); a++)
                {
                    if(!ByteArrayCompare(ReadSample.Get(i), SentSample.Get(a)))
                    {
                        ResultsSample.Enqueue(GenConstants.GenConstants.ErrorValue);
                       // ReadSample
                    }
                }
            }
        }

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

        private void DoWrite()
        {
            while (!stopped)
            {
                byte[] packet = ConstructPacket();
                WriteData(packet);
                SentSample.Enqueue(packet);
            }
        }

        private byte[] ConstructPacket()
        {
            byte StartByte = 0x58;
            byte StopByte = 0x59;
            Byte[] PacketToSend = new byte[3];
            PacketToSend[0] = StartByte;
            PacketToSend[1] = inc;
            PacketToSend[2] = StopByte;
            if (inc >= 255)
            {
                inc = 0;
            }
            inc++;
            return PacketToSend;
        }
        /*private void StartSampleRateMeasure()
        {
            sampleRateMeasureTimer = new System.Threading.Timer(SampleRateCounterTimerTickCallback, this, 0, 1000);
        }

        private void StopSampleRateMeasure()
        {
            sampleRateMeasureTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SampleRateCounterTimerTickCallback(object state)
        {
            SampleRate = sampleCounter;

            sampleCounter = 0;

            OnSampleRateUpdated(new SampleRateUpdatedEventArgs(SampleRate));
        }

        private void OnSampleRateUpdated(SampleRateUpdatedEventArgs e)
        {
            SampleRateUpdated(this, e);
        }
        */

    }
}
