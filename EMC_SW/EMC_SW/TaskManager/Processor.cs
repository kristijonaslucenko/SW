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
            int transmit = 0, missing = 0, error = 0, receive = 0, status = 0, operrors = 0, missingTrck1 = 0, missingTrck2 = 0, missingTrck3 = 0;

            for (int i = 0; i < task.Repetition; i++)
            {
                ProcessorBaseDataHandler.SendingData(task.AddedTask);
                transmit++;
                bool SerialTimedOut;
                byte[] read = ProcessorBaseDataHandler.ReadingData(EmcProtocol.Ack.ByteSize, out SerialTimedOut);

                if (!SerialTimedOut && EmcProtocol.Ack.IsValid(ref read))
                {
                    receive++;
                    missingTrck1 = 0;
                }
                else
                {
                    missing++;
                    missingTrck1 = 1;
                }
                if (missingTrck1 == 1 && missingTrck2 == 1 && missingTrck3 == 1)
                {
                    error++;
                    missingTrck1 = 0;
                    missingTrck2 = 0;
                    missingTrck3 = 0;
                }
                missingTrck3 = missingTrck2;
                missingTrck2 = missingTrck1;
            }
            CreateTransmissionRecord(task, transmit, receive, missing, error, status, operrors);
        }

        //Type 0x09 package: ControlUsbHost

        public void ProcessControlUsbHostTask(TaskLUP task)
        {
            int transmit = 0, missing = 0, error = 0, receive = 0, status = 0, operrors = 0, missingTrck1 = 0, missingTrck2 = 0, missingTrck3 = 0;

            for (int i = 0; i < task.Repetition; i++)
            {
                ProcessorBaseDataHandler.SendingData(task.AddedTask);
                transmit++;
                bool SerialTimedOut;
                byte[] read = ProcessorBaseDataHandler.ReadingData(EmcProtocol.ControlUsbHostResponse.ByteSize, out SerialTimedOut);

                if (!SerialTimedOut && EmcProtocol.ControlUsbHostResponse.IsValid(ref read))
                {
                    receive++;
                    missingTrck1 = 0;
                    status = (int)read[EmcProtocol.ControlUsbHostResponse.UsbStatePosition];
                }
                else
                {
                    missing++;
                    missingTrck1 = 1;
                }
                if (missingTrck1 == 1 && missingTrck2 == 1 && missingTrck3 == 1)
                {
                    error++;
                    missingTrck1 = 0;
                    missingTrck2 = 0;
                    missingTrck3 = 0;
                }
                missingTrck3 = missingTrck2;
                missingTrck2 = missingTrck1;
            }
            CreateTransmissionRecord(task, transmit, receive, missing, error, status, operrors);
        }

        //Type 0x07 package: ControlDisplay

        public void ProcessControlDisplayTask(TaskLUP task)
        {
            int transmit = 0, missing = 0, error = 0, receive = 0, status = 0, operrors = 0, missingTrck1 = 0, missingTrck2 = 0, missingTrck3 = 0;

            for (int i = 0; i < task.Repetition; i++)
            {
                ProcessorBaseDataHandler.SendingData(task.AddedTask);
                transmit++;
                bool SerialTimedOut;
                byte[] read = ProcessorBaseDataHandler.ReadingData(EmcProtocol.ControlDisplayResponse.ByteSize, out SerialTimedOut);

                if (!SerialTimedOut && EmcProtocol.ControlDisplayResponse.IsValid(ref read))
                {
                    receive++;
                    missingTrck1 = 0;
                    status = (int)read[EmcProtocol.ControlDisplayResponse.UiPosition];
                }
                else
                {
                    missing++;
                    missingTrck1 = 1;
                }
                if (missingTrck1 == 1 && missingTrck2 == 1 && missingTrck3 == 1)
                {
                    error++;
                    missingTrck1 = 0;
                    missingTrck2 = 0;
                    missingTrck3 = 0;
                }
                missingTrck3 = missingTrck2;
                missingTrck2 = missingTrck1;
            }
            CreateTransmissionRecord(task, transmit, receive, missing, error, status, operrors);
        }

        //Type 0x03 package: RequestLastKey

        public void ProcessRequestLastSeenKeyTask(TaskLUP task)
        {
            int transmit = 0, missing = 0, error = 0, receive = 0, status = 0, operrors = 0, missingTrck1 = 0, missingTrck2 = 0, missingTrck3 = 0;

            for (int i = 0; i < task.Repetition; i++)
            {
                ProcessorBaseDataHandler.SendingData(task.AddedTask);
                transmit++;
                bool SerialTimedOut;
                byte[] read = ProcessorBaseDataHandler.ReadingData(EmcProtocol.LastKeySeen.ByteSize, out SerialTimedOut);

                if (!SerialTimedOut && EmcProtocol.LastKeySeen.IsValid(ref read))
                {
                    receive++;
                    missingTrck1 = 0;
                    status = (int)read[EmcProtocol.LastKeySeen.KeyPosition];
                    operrors = (int)read[EmcProtocol.LastKeySeen.EventPosition];
                }
                else
                {
                    missing++;
                    missingTrck1 = 1;
                }
                if (missingTrck1 == 1 && missingTrck2 == 1 && missingTrck3 == 1)
                {
                    error++;
                    missingTrck1 = 0;
                    missingTrck2 = 0;
                    missingTrck3 = 0;
                }
                missingTrck3 = missingTrck2;
                missingTrck2 = missingTrck1;
            }
            CreateTransmissionRecord(task, transmit, receive, missing, error, status, operrors);
        }

        //Type 0x05 package: RequestDisplayState

        public void ProcessRequestDisplayStateTask(TaskLUP task)
        {
            int transmit = 0, missing = 0, error = 0, receive = 0, status = 0, operrors = 0, missingTrck1 = 0, missingTrck2 = 0, missingTrck3 = 0;

            for (int i = 0; i < task.Repetition; i++)
            {
                ProcessorBaseDataHandler.SendingData(task.AddedTask);
                transmit++;
                bool SerialTimedOut;
                byte[] read = ProcessorBaseDataHandler.ReadingData(EmcProtocol.DisplayState.ByteSize, out SerialTimedOut);

                if (!SerialTimedOut && EmcProtocol.DisplayState.IsValid(ref read))
                {
                    receive++;
                    missingTrck1 = 0;
                    status = (int)read[EmcProtocol.DisplayState.ConnectedPosition];
                }
                else
                {
                    missing++;
                    missingTrck1 = 1;
                }
                if (missingTrck1 == 1 && missingTrck2 == 1 && missingTrck3 == 1)
                {
                    error++;
                    missingTrck1 = 0;
                    missingTrck2 = 0;
                    missingTrck3 = 0;
                }
                missingTrck3 = missingTrck2;
                missingTrck2 = missingTrck1;
            }
            CreateTransmissionRecord(task, transmit, receive, missing, error, status, operrors);
        }

        //Type 0x0B package: RequestUsbHostStatus

        public void ProcessRequestUsbHostStatusTask(TaskLUP task)
        {
            int transmit = 0, missing = 0, error = 0, receive = 0, status = 0, operrors = 0, missingTrck1 = 0, missingTrck2 = 0, missingTrck3 = 0;

            for (int i = 0; i < task.Repetition; i++)
            {
                ProcessorBaseDataHandler.SendingData(task.AddedTask);
                transmit++;
                bool SerialTimedOut;
                byte[] read = ProcessorBaseDataHandler.ReadingData(EmcProtocol.UsbHostStatus.ByteSize, out SerialTimedOut);

                if (!SerialTimedOut && EmcProtocol.UsbHostStatus.IsValid(ref read))
                {
                    receive++;
                    missingTrck1 = 0;
                    status = (int)read[EmcProtocol.UsbHostStatus.StatusPosition];
                    operrors = (int)read[EmcProtocol.UsbHostStatus.ErrorPosition];
                }
                else
                {
                    missing++;
                    missingTrck1 = 1;
                }
                if (missingTrck1 == 1 && missingTrck2 == 1 && missingTrck3 == 1)
                {
                    error++;
                    missingTrck1 = 0;
                    missingTrck2 = 0;
                    missingTrck3 = 0;
                }
                missingTrck3 = missingTrck2;
                missingTrck2 = missingTrck1;
            }
            CreateTransmissionRecord(task, transmit, receive, missing, error, status, operrors);
        }

        private void CreateTransmissionRecord(TaskLUP task, int transmit, int receive, int missing, int error, int status, int operrors)
        {
            Record NewRecord = null;
            NewRecord = new Record
            {
                AddedTask = task.AddedTask,
                Id = task.id,
                Repetition = task.Repetition,
                IsContinuous = task.IsContinuous,
                Status = status,
                OpErrors = operrors,
                Transmitted = transmit,
                Received = receive,
                Missing = missing,
                Errors = error
            };
            ProcessorBaseDataHandler.TransmissionResults.Enqueue(NewRecord);
        }
    }
}
