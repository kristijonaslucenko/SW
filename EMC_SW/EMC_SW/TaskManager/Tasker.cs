using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EMC_SW.Models;
using EmcProtocol;
using EMC_SW.GenConstants;
using System.Diagnostics;
using EMC_SW.Controllers;
using EMC_SW.DataHandlers;

namespace EMC_SW.TaskManager
{
    public class Tasker
    {
        private Thread TaskManagerThread;
        public TaskQueue LupTaskQueue { get; set; }
        internal Processor Processor { get; set; }

        public Tasker(BaseDataHandler ProcessorDataHandler)
        {
            LupTaskQueue = new TaskQueue();
            Processor = new Processor(ProcessorDataHandler);
        }     

        public void CreateTask(TaskLUP taskToBeCreated)
        {
            LupTaskQueue.Enqueue(taskToBeCreated);
        }

        public void InitiateTaskProcessing()
        {
            TaskManagerThread = new Thread(ProcessTasks);
            TaskManagerThread.Start();
        }

        private void ProcessTasks()
        {

            int queueCount = LupTaskQueue.CountEntries();

            while (LupTaskQueue.Peek(out TaskLUP FirstQueueTask))
            {
                int taskId = FirstQueueTask.id;

                switch (taskId)
                {
                    // CallTaskId
                    case 1:
                        Processor.ProcessCallTask(FirstQueueTask);
                        LupTaskQueue.Dequeue();
                        if (FirstQueueTask.IsContinuous)
                        {
                            LupTaskQueue.Enqueue(FirstQueueTask);
                        }
                        break;
                    //ControlDisplayTaskId
                    case 2:
                        Processor.ProcessControlUsbHostTask(FirstQueueTask);
                        LupTaskQueue.Dequeue();
                        if (FirstQueueTask.IsContinuous)
                        {
                            LupTaskQueue.Enqueue(FirstQueueTask);
                        }
                        break;
                    case 3:
                        //ControlUsbHostTaskId
                        Processor.ProcessControlUsbHostTask(FirstQueueTask);
                        LupTaskQueue.Dequeue();
                        if (FirstQueueTask.IsContinuous)
                        {
                            LupTaskQueue.Enqueue(FirstQueueTask);
                        }
                        break;
                    //RequestLastSeenKeyTaskId
                    case 4:
                        Processor.ProcessRequestLastSeenKeyTask(FirstQueueTask);
                        LupTaskQueue.Dequeue();
                        if (FirstQueueTask.IsContinuous)
                        {
                            LupTaskQueue.Enqueue(FirstQueueTask);
                        }
                        break;
                    //RequestDisplayStateTaskId
                    case 5:
                        Processor.ProcessRequestDisplayStateTask(FirstQueueTask);
                        LupTaskQueue.Dequeue();
                        if (FirstQueueTask.IsContinuous)
                        {
                            LupTaskQueue.Enqueue(FirstQueueTask);
                        }
                        break;
                    //RequestUsbHostStatusTaskId
                    case 6:
                        Processor.ProcessRequestUsbHostStatusTask(FirstQueueTask);
                        LupTaskQueue.Dequeue();
                        if (FirstQueueTask.IsContinuous)
                        {
                            LupTaskQueue.Enqueue(FirstQueueTask);
                        }
                        break;
                        //clean task queue
                    case 7:
                        LupTaskQueue.CleanQueue();
                        break;
                }
            }
        }
        /*Thread;
        Conqurent queue,
        Schedule, 
        Task class su properciais bufferis ir kartojimas, done prop
        ConqurrentBag ? dequeue enqueu
        bytes to read*/

    }
}
