using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EMC_SW.Controllers;
using System.Threading;
using EMC_SW.Models;

namespace EMC_SW
{


    public partial class MainForm : Form
    {
        MainController RS232controller = null;
        MainController RS485controller = null;
        MainController USBdev1Controller = null;
        MainController USBdev2Controller = null;
        MainController USBmodemController = null;

        LocalResults localResultsRS232 = null;
        LocalResults localResultsRS485 = null;
        LocalResults localResultsDev1 = null;
        LocalResults localResultsDev2 = null;
        LocalResults localResultsModem = null;

        TaskLUP InitiateLCDinTestMode = null;
        TaskLUP InitiateUsbWriting = null;
        TaskLUP CallingTask = null;
        TaskLUP RequestLastSeenKey = null;
        TaskLUP RequestDisplayState = null;
        TaskLUP RequestUsbHostStatus = null;
        TaskLUP ReturnLcdToNormalMode = null;
        TaskLUP UsbControlTask = null;
        TaskLUP StopAllTask = null;

        public MainForm()
        {
            InitializeComponent();
            RS232controller = new MainController();
            RS485controller = new MainController();
            USBdev1Controller = new MainController();
            USBdev2Controller = new MainController();
            USBmodemController = new MainController();

            localResultsRS232 = new LocalResults()
            {
                transmittedPackets = 0,
                receivedPackets = 0,
                missedPackets = 0,
                errorsInTransmission = 0
            };

            localResultsRS485 = new LocalResults()
            {
                transmittedPackets = 0,
                receivedPackets = 0,
                missedPackets = 0,
                errorsInTransmission = 0
            };

            StopAllTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(), //dummy
                id = GenConstants.GenConstants.StopTaskId,
                Repetition = 0,
                IsContinuous = false
            };

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                RS232comBox.Items.Add(s);
                RS485comBox.Items.Add(s);
                USBmodemComBox.Items.Add(s);
                USBhostComBox.Items.Add(s);
                USBdeviceComBox.Items.Add(s);
            }

        }

        private void RS232startBtn_Click(object sender, EventArgs e)
        {
            RS232controller.TaskManager.InitiateTaskProcessing();
        }

        private void RS485startBtn_Click(object sender, EventArgs e)
        {
            RS485controller.TaskManager.InitiateTaskProcessing();
        }

        private void COMconnectBtn_Click(object sender, EventArgs e)
        {
            String RS232port = RS232comBox.GetItemText(RS232comBox.SelectedItem);
            String RS485port = RS485comBox.GetItemText(RS485comBox.SelectedItem);
            String USBhostPort = USBhostComBox.GetItemText(USBhostComBox.SelectedItem);
            String USBdevicePort = USBdeviceComBox.GetItemText(USBdeviceComBox.SelectedItem);
            String USBmodemPort = USBmodemComBox.GetItemText(USBmodemComBox.SelectedItem);

            GUIrefreshTimer.Enabled = true;

            if (RS232port != "")
            {
                RS232controller.Start(RS232port);


            }

            if (RS485port != "")
            {
                RS485controller.Start(RS485port);
            }
            /*
            if (USBhostPort != "")
            {
                USBhostController.Start(USBhostPort);
            }

            if (USBdevicePort != "")
            {
                USBdeviceController.Start(USBdevicePort);
            }

            if (USBmodemPort != "")
            {
                USBmodemController.Start(USBmodemPort);
            }*/
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            while (RS232controller.TransmissionResults.Peek(out Record LocalPeekedRecord))
            {
                localResultsRS232.transmittedPackets += LocalPeekedRecord.Transmitted;
                localResultsRS232.receivedPackets += LocalPeekedRecord.Received;
                localResultsRS232.missedPackets += LocalPeekedRecord.Missing;
                localResultsRS232.errorsInTransmission += LocalPeekedRecord.Errors;
                RS232controller.TransmissionResults.Dequeue();
            }
            RS232textboxT.Text = localResultsRS232.transmittedPackets.ToString();
            RS232textboxR.Text = localResultsRS232.receivedPackets.ToString();
            RS232textboxM.Text = localResultsRS232.missedPackets.ToString();
            RS232textboxE.Text = localResultsRS232.errorsInTransmission.ToString();

            while (RS485controller.TransmissionResults.Peek(out Record LocalPeekedRecord))
            {
                localResultsRS485.transmittedPackets += LocalPeekedRecord.Transmitted;
                localResultsRS485.receivedPackets += LocalPeekedRecord.Received;
                localResultsRS485.missedPackets += LocalPeekedRecord.Missing;
                localResultsRS485.errorsInTransmission += LocalPeekedRecord.Errors;
                if (LocalPeekedRecord.Status != 0 || LocalPeekedRecord.OpErrors != 0) //Diffrentiate from simple calls
                {
                    switch (LocalPeekedRecord.Id)
                    {
                        //ControlUsbHostResponse 
                        case GenConstants.GenConstants.ControlUsbHostTaskId:

                            localResultsRS485.controlUsbHostResponse = LocalPeekedRecord.Status;
                            if (localResultsRS485.controlUsbHostResponse == GenConstants.GenConstants.controlUsbHostResponseRWrunning)
                            {
                                UsbHostStatusText.Text = GenConstants.GenConstants.controlUsbHostTextBoxValueRWRunning;
                            }
                            else if (localResultsRS485.controlUsbHostResponse == GenConstants.GenConstants.controlUsbHostResponseNotAccessible)
                            {
                                UsbHostStatusText.Text = GenConstants.GenConstants.controlUsbHostTextBoxValueNotAcc;
                            }
                            break;
                        // ControlDisplayResponse 
                        case GenConstants.GenConstants.ControlDisplayTaskId:

                            localResultsRS485.controlDisplayResponse = LocalPeekedRecord.Status;

                            if (localResultsRS485.controlDisplayResponse == GenConstants.GenConstants.controlDisplayResponseNui)
                            {
                                displayStatusTextBox.Text = GenConstants.GenConstants.controlDisplayResponseNuiText;
                            }else if(localResultsRS485.controlDisplayResponse == GenConstants.GenConstants.controlDisplayResponseTui)
                            {
                                displayStatusTextBox.Text = GenConstants.GenConstants.controlDisplayResponseTuiText;
                            }
                            break;
                        // RequestLastKey
                        case GenConstants.GenConstants.RequestLastSeenKeyTaskId:

                            localResultsRS485.lastKeySeen = LocalPeekedRecord.Status;
                            localResultsRS485.keyPressStatus = LocalPeekedRecord.OpErrors;

                            if (localResultsRS485.lastKeySeen == GenConstants.GenConstants.requestedLastKeyNA)
                            {
                                lastKeyPressedTextBox.Text = GenConstants.GenConstants.requestedLastKeyNAtext;
                            }
                            else
                            {
                                Char key = (char)localResultsRS485.lastKeySeen;
                                lastKeyPressedTextBox.Text = key.ToString();
                                localResultsRS485.keyCount++;
                                if (localResultsRS485.keyPressStatus == GenConstants.GenConstants.keyPressStatusNoEv)
                                {

                                }
                            }
                            
                            break;
                    }
                }
                RS485controller.TransmissionResults.Dequeue();
            }
            RS485textboxT.Text = localResultsRS485.transmittedPackets.ToString();
            RS485textboxR.Text = localResultsRS485.receivedPackets.ToString();
            RS485textboxM.Text = localResultsRS485.missedPackets.ToString();
            RS485textboxE.Text = localResultsRS485.errorsInTransmission.ToString();


        }

        private void RS232resetBtn_Click(object sender, EventArgs e)
        {
            localResultsRS232.ZeroCounters();
        }

        private void RS485resetBtn_Click(object sender, EventArgs e)
        {
            localResultsRS485.ZeroCounters();
        }

        private void RS232stopBtn_Click(object sender, EventArgs e)
        {
            RS232controller.TaskManager.CreateTask(StopAllTask);
        }

        private void RS485stopBtn_Click(object sender, EventArgs e)
        {

            RS485controller.TaskManager.CreateTask(StopAllTask);
        }

        private void USBdev1StopBtn_Click(object sender, EventArgs e)
        {
            USBdev1Controller.TaskManager.CreateTask(StopAllTask);
        }

        private void USBdev2StopBtn_Click(object sender, EventArgs e)
        {
            USBdev2Controller.TaskManager.CreateTask(StopAllTask);
        }

        private void USBmodemStopBtn_Click(object sender, EventArgs e)
        {
            USBmodemController.TaskManager.CreateTask(StopAllTask);
        }

        private void USBdev1ResetBtn_Click(object sender, EventArgs e)
        {
            localResultsDev1.ZeroCounters();
        }

        private void USBdev2ResetBtn_Click(object sender, EventArgs e)
        {
            localResultsDev2.ZeroCounters();
        }

        private void USBmodemResetBtn_Click(object sender, EventArgs e)
        {
            localResultsModem.ZeroCounters();
        }

        private void RS232TaskInit()
        {
            //min repetition is 3
            CallingTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(),
                id = GenConstants.GenConstants.CallTaskId,
                Repetition = 3,
                IsContinuous = true
            };

            UsbControlTask = new TaskLUP
            {
                AddedTask = null,
                id = GenConstants.GenConstants.ControlUsbHostTaskId,
                Repetition = 0,
                IsContinuous = false
            };

            RS232controller.TaskQueueSize = 200; //deal with this one
            RS232controller.TransmissionResultsSize = 200; //deal with this one
            RS232controller.TaskManager.CreateTask(CallingTask);
            //RS232controller.MyTasker.CreateTask(UsbControlTask);
        }

        private void RS485TaskInit()
        {
            CallingTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(),
                id = GenConstants.GenConstants.CallTaskId,
                Repetition = 3,
                IsContinuous = true
            };

            UsbControlTask = new TaskLUP
            {
                AddedTask = null,
                id = GenConstants.GenConstants.ControlUsbHostTaskId,
                Repetition = 0,
                IsContinuous = false
            };
            RS485controller.TaskQueueSize = 200; //deal with this one
            RS485controller.TransmissionResultsSize = 200; //deal with this one
            RS485controller.TaskManager.CreateTask(CallingTask);
        }

        private void SerialOverDev1TaskInit()
        {

        }

        private void SerialOverDev2TaskInit()
        {

        }

        private void ModemRS232TtlTaskInit()
        {

        }

        private void USBdev1StartBtn_Click(object sender, EventArgs e)
        {
            SerialOverDev1TaskInit();
        }

        private void USBdev2StartBtn_Click(object sender, EventArgs e)
        {
            SerialOverDev2TaskInit();
        }

        private void USBmodemStartBtn_Click(object sender, EventArgs e)
        {
            ModemRS232TtlTaskInit();
        }
    }
    public class LocalResults
    {
        public int transmittedPackets { get; set; }
        public int receivedPackets { get; set; }
        public int missedPackets { get; set; }
        public int errorsInTransmission { get; set; }
        public int usbHostStatus { get; set; }
        public int usbHostErrorCount { get; set; }
        public int controlUsbHostResponse { get; set; }
        public int controlDisplayResponse { get; set; }
        public int displayState { get; set; }
        public int lastKeySeen { get; set; }
        public int keyPressStatus { get; set; }
        public int keyCount { get; set; }

        public void ZeroCounters()
        {
            transmittedPackets = 0;
            receivedPackets = 0;
            missedPackets = 0;
            errorsInTransmission = 0;
        }

        public void ZeroAdditionalProperties()
        {
            usbHostStatus = 0;
            usbHostErrorCount = 0;
            controlUsbHostResponse = 0;
            controlDisplayResponse = 0;
            displayState = 0;
            lastKeySeen = 0;
            keyPressStatus = 0;
            keyCount = 0;
        }

        public void ZeroAll()
        {
            transmittedPackets = 0;
            receivedPackets = 0;
            missedPackets = 0;
            errorsInTransmission = 0;
            usbHostStatus = 0;
            usbHostErrorCount = 0;
            controlUsbHostResponse = 0;
            controlDisplayResponse = 0;
            displayState = 0;
            lastKeySeen = 0;
            keyPressStatus = 0;
            keyCount = 0;
        }
    }

}
