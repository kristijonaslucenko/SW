﻿using System;
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
                TaskLUP FirstQueueTask;
                
            while (LupTaskQueue.Peek(out FirstQueueTask))
                {
                int taskId = FirstQueueTask.id;

                    switch (taskId)
                    {
                        // GenConstants.GenConstants.CallingTaskId
                        case 1:
                                Processor.ProcessCallTask(FirstQueueTask);
                                LupTaskQueue.Dequeue();
                                //queueCount = 0;
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
                        LupTaskQueue.Dequeue();
                        break;
                        default:

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
