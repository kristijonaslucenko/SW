using EMC_SW.DataHandlers;
using EMC_SW.Models;
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
                return DataHandler.CurrentSample;
            }
        }

        public int CurrentSampleRate
        {
            get;
            private set;
        }

        public double ScaleCoeff { get; set; }
        public double Offset { get; set; }

        public int SampleSize
        {
            get
            {
                return DataHandler.CurrentSample.Limit;
            }
            set
            {
                DataHandler.CurrentSample.Limit = value;
            }
        }

        public MainController()
        {
            DataHandler = new SerialDataHandler();

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
