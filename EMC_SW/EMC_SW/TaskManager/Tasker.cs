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

namespace EMC_SW.TaskManager
{
    public class Tasker
    {
        private Thread TaskManagerThread;
        public TaskQueue LupTaskQueue { get; set; }
        //public TaskLUP taskLUP { get; set; }
        private bool stopTaskProcessing;

        public Tasker()
        {
            LupTaskQueue = new TaskQueue();
            //taskLUP = new TaskLUP();
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
                    switch (taskId)
                    {
                        // GenConstants.GenConstants.CallingTaskId
                        case  1:
                            Debug.Print(LupTaskQueue.Get(i).id.ToString());

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
