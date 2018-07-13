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

namespace EMC_SW.TaskManager
{
    public class Tasker
    {
        private Thread TaskManagerThread;
        public TaskQueue LupTaskQueue { get; set; }
        internal Processor Processor { get; set; }

        //public TaskLUP taskLUP { get; set; }
        private bool stopTaskProcessing;
        

        public Tasker()
        {
            LupTaskQueue = new TaskQueue();
            Processor = new Processor();
        }

        public void CreateTask(TaskLUP taskToBeCreated)
        {
            LupTaskQueue.Enqueue(taskToBeCreated);
        }

        public void InitiateTaskProcessing()
        {
            TaskManagerThread = new Thread(ProcessTasks);
        }

        public void ProcessTasks()
        {

            while (!stopTaskProcessing)
            {
                int queueCount = LupTaskQueue.CountEntries();
                for (int i = 0; i < queueCount; ++i)
                {
                    int taskId= LupTaskQueue.Get(i).id;
                    TaskLUP CurrentTask = LupTaskQueue.Get(i);

                    switch (taskId)
                    {
                        // GenConstants.GenConstants.CallingTaskId
                        case  1:
                            Processor.ProcessCallTask(CurrentTask);

                            break;
                        //InitiateUsbWritingId
                        case 2:
                            

                            break;
                        case 3:
                            //RequestLastSeenKeyTaskId

                            break;
                        //RequestDisplayStateTaskId
                        case 4:

                            break;
                        //RequestUsbHostStatusTaskId
                        case 5:

                            break;
                        //ReturnLcdToNormalModeTaskId
                        case 6:

                            break;
                        //StopUsbWritingTaskId
                        case 7:

                            break;
                        default:

                            break;
                    }
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
