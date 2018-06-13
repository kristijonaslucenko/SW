using EMC_SW.Models;
using System;
using System.Collections.Generic;
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

        public int SampleRate { get; private set; }

        private int sampleCounter;

        //private System.Threading.Timer sampleRateMeasureTimer;

        public event SampleRateUpdatedEventHandler SampleRateUpdated;

        public BaseDataHandler()
        {
            CurrentSample = new Sample();
        }

        protected abstract double ReadData();
        protected abstract void WriteData(byte[] buffer);

        public virtual void Open(String port)
        {
            stopped = false;

            //StartSampleRateMeasure();

            readThread = new Thread(DoRead);
            readThread.Start();
        }

        public virtual void Close()
        {
            stopped = true;

            //StopSampleRateMeasure();
        }

        private void DoRead()
        {
            while (!stopped)
            {
                double value = ReadData();

                //sampleCounter++;

                //CurrentSample.Enqueue(value);
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
