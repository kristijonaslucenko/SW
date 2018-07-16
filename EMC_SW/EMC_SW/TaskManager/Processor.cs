using EMC_SW.Models;
using EMC_SW.DataHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EMC_SW.Controllers
{
    class Processor
    {
        public Processor(BaseDataHandler processorBaseDataHandler)
        {
            ProcessorBaseDataHandler = processorBaseDataHandler;
        }

        BaseDataHandler ProcessorBaseDataHandler { get; set; }

        public void ProcessCallTask(TaskLUP task)
        {
            ProcessorBaseDataHandler.SendingData(task.AddedTask);

            bool SerialTimedOut;
            byte[] read = ProcessorBaseDataHandler.ReadingData(EmcProtocol.Call.ByteSize,out SerialTimedOut);


            //Debug.Print("PRocessor Call task");
            //ProcessorBaseDataHandler.SendingData(new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 });
        }
    }
}
