using EMC_SW.DataHandlers;
using EMC_SW.Models;
using EMC_SW.TaskManager;
//using EMC_SW.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EMC_SW.Controllers
{
    internal class MainController
    {
        public BaseDataHandler DataHandler
        {
            get;
            private set;
        }

        public Sample CurrentSample
        {
            get
            {
                return DataHandler.ReadSample;
            }
        }

        public Sample SentSample
        {
            get
            {
                return DataHandler.SentSample;
            }
        }

        public int CurrentSampleRate
        {
            get;
            private set;
        }

        public int SampleSize
        {
            get
            {
                return DataHandler.ReadSample.Limit;
            }
            set
            {
                DataHandler.ReadSample.Limit = value;
            }
        }

        public int SentSampleSize
        {
            get
            {
                return DataHandler.SentSample.Limit;
            }
            set
            {
                DataHandler.SentSample.Limit = value;
            }
        }

        public int TaskQueueSize
        {
            get
            {
                return MyTasker.LupTaskQueue.Limit;
            }
            set
            {
                MyTasker.LupTaskQueue.Limit = value;
            }
        }

        public Tasker MyTasker { get; private set; }

        public MainController()
        {
            DataHandler = new SerialDataHandler();
            MyTasker = new Tasker(DataHandler);
        }

        public void Start(String port)
        {
            DataHandler.Open(port);
        }

        public void Stop()
        {
            DataHandler.Close();
        }

        public void SendData(byte[] buffer)
        {
            DataHandler.SendingData(buffer);
        }

    }
}
