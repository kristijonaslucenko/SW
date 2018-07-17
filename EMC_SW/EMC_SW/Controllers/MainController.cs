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

        public TransmissionRecord TransmissionResults
        {
            get
            {
                return DataHandler.TransmissionResults;
            }
        }

        public int TransmissionResultsSize
        {
            get
            {
                return DataHandler.TransmissionResults.Limit;
            }
            set
            {
                DataHandler.TransmissionResults.Limit = value;
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
