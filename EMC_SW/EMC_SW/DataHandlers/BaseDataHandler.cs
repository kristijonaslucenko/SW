using EMC_SW.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private bool stopped;

        private Thread readThread;

        public Sample CurrentSample { get; set; }
        //public Sample SentSample { get; set; }

        public int SampleRate { get; private set; }


        public BaseDataHandler()
        {
            CurrentSample = new Sample();
            //SentSample = new Sample();
        }

        protected abstract byte[] ReadData();
        protected abstract void WriteData(byte[] buffer);

        public virtual void Open(String port)
        {
            stopped = false;
            readThread = new Thread(DoRead);
            readThread.Start();
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
                byte[] value = ReadData();

                //sampleCounter++;
                if (value != null)
                {
                    CurrentSample.Enqueue(value);
                }
                //Debug.Print(value.ToString());
            }
        }

        private void DoWrite(byte[] buffer)
        {
            WriteData(buffer);
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
