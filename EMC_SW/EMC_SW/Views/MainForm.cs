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
        TaskLUP GenCallingTask = null;
        TaskLUP RequestLastSeenKey = null;
        TaskLUP RequestDisplayState = null;
        TaskLUP RequestUsbHostStatus = null;
        TaskLUP ReturnLcdToNormalMode = null;
        TaskLUP UsbControlTask = null;
        TaskLUP StopAllTask = null;

        bool rs232Started = false;
        bool rs485Started = false;
        bool dev1Started = false;
        bool dev2Started = false;
        bool rs232TtlStarted = false;

        bool rs232Connected = false;
        bool rs485Connected = false;
        bool dev1Connected = false;
        bool dev2Connected = false;
        bool rs232TtlConnected = false;

        List<string> portsInUse = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            RS232controller = new MainController();
            RS485controller = new MainController();
            USBdev1Controller = new MainController();
            USBdev2Controller = new MainController();
            USBmodemController = new MainController();

            localResultsRS232 = new LocalResults();
            localResultsRS485 = new LocalResults();
            localResultsDev1 = new LocalResults();
            localResultsDev2 = new LocalResults();
            localResultsModem = new LocalResults();

            localResultsRS232.ZeroAll();
            localResultsRS485.ZeroAll();
            localResultsDev1.ZeroAll();
            localResultsDev2.ZeroAll();
            localResultsModem.ZeroAll();

            baudrateComboBox.SelectedIndex = 4;

            StopAllTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(), //dummy
                id = GenConstants.GenConstants.StopTaskId,
                Repetition = 0,
                IsContinuous = false
            };

            GenCallingTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(),
                id = GenConstants.GenConstants.CallTaskId,
                Repetition = 3,     //min 3
                IsContinuous = true
            };

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                RS232comBox.Items.Add(s);
                RS485comBox.Items.Add(s);
                USBmodemComBox.Items.Add(s);
                USBdev1ComBox.Items.Add(s);
                USBdev2ComBox.Items.Add(s);
                /*RS232comBox.SelectedIndex = 5;
                RS485comBox.SelectedIndex = 1;
                USBdev1ComBox.SelectedIndex = 2;
                USBdev2ComBox.SelectedIndex = 3;
                USBmodemComBox.SelectedIndex = 4;*/
            }

        }

        private void RS232startBtn_Click(object sender, EventArgs e)
        {
            RS232TaskInit();
        }

        private void RS485startBtn_Click(object sender, EventArgs e)
        {
            RS485TaskInit();
        }

        private void COMconnectBtn_Click(object sender, EventArgs e)
        {
            string RS232port = RS232comBox.GetItemText(RS232comBox.SelectedItem);
            string RS485port = RS485comBox.GetItemText(RS485comBox.SelectedItem);
            string USBdev1tPort = USBdev1ComBox.GetItemText(USBdev1ComBox.SelectedItem);
            string USBdev2Port = USBdev2ComBox.GetItemText(USBdev2ComBox.SelectedItem);
            string USBmodemPort = USBmodemComBox.GetItemText(USBmodemComBox.SelectedItem);
            int userBaudrate = Int32.Parse(baudrateComboBox.GetItemText(baudrateComboBox.SelectedItem));

            GUIrefreshTimer.Enabled = true;

            if (RS232port != "" && !rs232Connected && !portsInUse.Contains(RS232port))
            {
                RS232controller.Connect(RS232port, userBaudrate);
                rs232StatusLbl.Text = userBaudrate.ToString() + ", connected";
                portsInUse.Add(RS232port);
                rs232Connected = true;
                RS232comBox.Enabled = false;
            }

            if (RS485port != "" && !rs485Connected && !portsInUse.Contains(RS485port))
            {
                RS485controller.Connect(RS485port, userBaudrate);
                rs485StatusLbl.Text = userBaudrate.ToString() + ", connected";
                portsInUse.Add(RS485port);
                rs485Connected = true;
                RS485comBox.Enabled = false;
            }

            if (USBdev1tPort != "" && !dev1Connected && !portsInUse.Contains(USBdev1tPort))
            {
                USBdev1Controller.Connect(USBdev1tPort, userBaudrate);
                dev1StatusLbl.Text = userBaudrate.ToString() + ", connected";
                portsInUse.Add(USBdev1tPort);
                dev1Connected = true;
                USBdev1ComBox.Enabled = false;
            }

            if (USBdev2Port != "" && !dev2Connected && !portsInUse.Contains(USBdev2Port))
            {
                USBdev2Controller.Connect(USBdev2Port, userBaudrate);
                dev2StatusLbl.Text = userBaudrate.ToString() + ", connected";
                dev2Connected = true;
                portsInUse.Add(USBdev2Port);
                USBdev2ComBox.Enabled = false;
            }

            if (USBmodemPort != "" && !rs232TtlConnected && !portsInUse.Contains(USBmodemPort))
            {
                USBmodemController.Connect(USBmodemPort, userBaudrate);
                rs232ttlStatusLbl.Text = userBaudrate.ToString() + ", connected";
                rs232TtlConnected = true;
                portsInUse.Add(USBmodemPort);
                USBmodemComBox.Enabled = false;
            }
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

            while (USBdev1Controller.TransmissionResults.Peek(out Record LocalPeekedRecord))
            {
                localResultsDev1.transmittedPackets += LocalPeekedRecord.Transmitted;
                localResultsDev1.receivedPackets += LocalPeekedRecord.Received;
                localResultsDev1.missedPackets += LocalPeekedRecord.Missing;
                localResultsDev1.errorsInTransmission += LocalPeekedRecord.Errors;
                USBdev1Controller.TransmissionResults.Dequeue();
            }
            Dev1textboxT.Text = localResultsDev1.transmittedPackets.ToString();
            Dev1textboxR.Text = localResultsDev1.receivedPackets.ToString();
            Dev1textboxM.Text = localResultsDev1.missedPackets.ToString();
            Dev1textboxE.Text = localResultsDev1.errorsInTransmission.ToString();

            while (USBdev2Controller.TransmissionResults.Peek(out Record LocalPeekedRecord))
            {
                localResultsDev2.transmittedPackets += LocalPeekedRecord.Transmitted;
                localResultsDev2.receivedPackets += LocalPeekedRecord.Received;
                localResultsDev2.missedPackets += LocalPeekedRecord.Missing;
                localResultsDev2.errorsInTransmission += LocalPeekedRecord.Errors;
                USBdev2Controller.TransmissionResults.Dequeue();
            }
            Dev2textboxT.Text = localResultsDev2.transmittedPackets.ToString();
            Dev2textboxR.Text = localResultsDev2.receivedPackets.ToString();
            Dev2textboxM.Text = localResultsDev2.missedPackets.ToString();
            Dev2textboxE.Text = localResultsDev2.errorsInTransmission.ToString();

            while (USBmodemController.TransmissionResults.Peek(out Record LocalPeekedRecord))
            {
                localResultsModem.transmittedPackets += LocalPeekedRecord.Transmitted;
                localResultsModem.receivedPackets += LocalPeekedRecord.Received;
                localResultsModem.missedPackets += LocalPeekedRecord.Missing;
                localResultsModem.errorsInTransmission += LocalPeekedRecord.Errors;
                USBmodemController.TransmissionResults.Dequeue();
            }
            RS232TTLtextboxT.Text = localResultsModem.transmittedPackets.ToString();
            RS232TTLtextboxR.Text = localResultsModem.receivedPackets.ToString();
            RS232TTLtextboxM.Text = localResultsModem.missedPackets.ToString();
            RS232TTLtextboxE.Text = localResultsModem.errorsInTransmission.ToString();

            while (RS485controller.TransmissionResults.Peek(out Record LocalPeekedRecord))
            {
                localResultsRS485.transmittedPackets += LocalPeekedRecord.Transmitted;
                localResultsRS485.receivedPackets += LocalPeekedRecord.Received;
                localResultsRS485.missedPackets += LocalPeekedRecord.Missing;
                localResultsRS485.errorsInTransmission += LocalPeekedRecord.Errors;
                if (LocalPeekedRecord.Id != 1 || LocalPeekedRecord.Id!= 7) //Diffrentiate from simple calls
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
                            else if (localResultsRS485.controlUsbHostResponse == GenConstants.GenConstants.controlUsbHostNth)
                            {
                                UsbHostStatusText.Text = GenConstants.GenConstants.controlUsbHostNthText;
                            }
                            break;
                        // ControlDisplayResponse 
                        case GenConstants.GenConstants.ControlDisplayTaskId:

                            localResultsRS485.controlDisplayResponse = LocalPeekedRecord.Status;

                            if (localResultsRS485.controlDisplayResponse == GenConstants.GenConstants.controlDisplayResponseNui)
                            {
                                displayStatusTextBox.Text = GenConstants.GenConstants.controlDisplayResponseNuiText;
                            }
                            else if (localResultsRS485.controlDisplayResponse == GenConstants.GenConstants.controlDisplayResponseTui)
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
                                    keyPressEventText.Text = GenConstants.GenConstants.keyPressStatusNoEvText;
                                }
                                else if (localResultsRS485.keyPressStatus == GenConstants.GenConstants.keyPressStatusPrNotRel)
                                {
                                    keyPressEventText.Text = GenConstants.GenConstants.keyPressStatusPrNotRelText;
                                }
                                else if (localResultsRS485.keyPressStatus == GenConstants.GenConstants.keyPressStatusPrRel)
                                {
                                    keyPressEventText.Text = GenConstants.GenConstants.keyPressStatusPrRelText;
                                }
                            }
                            break;
                        //RequestDisplayState
                        case GenConstants.GenConstants.RequestDisplayStateTaskId:

                            localResultsRS485.displayState = LocalPeekedRecord.Status;

                            if (localResultsRS485.displayState == GenConstants.GenConstants.displayStateConnected)
                            {
                                displayConnStatusText.Text = GenConstants.GenConstants.displayStateConnectedText;
                            }
                            else if (localResultsRS485.displayState == GenConstants.GenConstants.displayStateNotConn)
                            {
                                displayConnStatusText.Text = GenConstants.GenConstants.displayStateNotConnText;
                            }
                            break;
                        //RequestUsbHostStatus
                        case GenConstants.GenConstants.RequestUsbHostStatusTaskId:

                            localResultsRS485.usbHostStatus = LocalPeekedRecord.Status;
                            localResultsRS485.usbHostErrorCount = LocalPeekedRecord.OpErrors;

                            if (localResultsRS485.usbHostStatus == GenConstants.GenConstants.usbHostStatusNA)
                            {
                                UsbHostStatusText.Text = GenConstants.GenConstants.usbHostStatusNAtext;
                            }
                            else if (localResultsRS485.usbHostStatus == GenConstants.GenConstants.usbHostStatusNotAcc)
                            {
                                UsbHostStatusText.Text = GenConstants.GenConstants.usbHostStatusNotAccText;
                            }
                            else if (localResultsRS485.usbHostStatus == GenConstants.GenConstants.usbHostStatusRWrunn)
                            {
                                UsbHostStatusText.Text = GenConstants.GenConstants.usbHostStatusRWrunnText;
                            }

                            usbHostErrorCountText.Text = localResultsRS485.usbHostErrorCount.ToString();
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
            rs232Started = false;
        }

        private void RS485stopBtn_Click(object sender, EventArgs e)
        {
            RS485controller.TaskManager.CreateTask(StopAllTask);
            rs485Started = false;
        }

        private void USBdev1StopBtn_Click(object sender, EventArgs e)
        {
            USBdev1Controller.TaskManager.CreateTask(StopAllTask);
            dev1Started = false;
        }

        private void USBdev2StopBtn_Click(object sender, EventArgs e)
        {
            USBdev2Controller.TaskManager.CreateTask(StopAllTask);
            dev2Started = false;
        }

        private void USBmodemStopBtn_Click(object sender, EventArgs e)
        {
            USBmodemController.TaskManager.CreateTask(StopAllTask);
            rs232TtlStarted = false;
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
            if (!rs232Started && rs232Connected)
            {
                RS232controller.TaskQueueSize = GenConstants.GenConstants.taskQueueSize; //deal with this one
                RS232controller.TransmissionResultsSize = GenConstants.GenConstants.transmissionResultsSize; //deal with this one
                RS232controller.TaskManager.CreateTask(GenCallingTask);
                //RS232controller.MyTasker.CreateTask(UsbControlTask);
                RS232controller.TaskManager.InitiateTaskProcessing();
                rs232Started = !rs232Started;
            }
        }

        private void RS485TaskInit()
        {
            if (rs485Connected)    ///!rs485Started && 
            {
                UsbControlTask = new TaskLUP
                {
                    AddedTask = EmcProtocol.ControlUsbHost.StartUsbReadWrite(),
                    id = GenConstants.GenConstants.ControlUsbHostTaskId,
                    Repetition = 10,
                    IsContinuous = false
                };
                RS485controller.TaskQueueSize = GenConstants.GenConstants.taskQueueSize; //deal with this one
                RS485controller.TransmissionResultsSize = GenConstants.GenConstants.transmissionResultsSize; //deal with this one
                //RS485controller.TaskManager.CreateTask(GenCallingTask);
                RS485controller.TaskManager.CreateTask(UsbControlTask);
                RS485controller.TaskManager.InitiateTaskProcessing();
                rs485Started = !rs485Started;
            }
        }

        private void SerialOverDev1TaskInit()
        {
            if (!dev1Started && dev1Connected)
            {
                USBdev1Controller.TaskQueueSize = GenConstants.GenConstants.taskQueueSize;
                USBdev1Controller.TransmissionResultsSize = GenConstants.GenConstants.transmissionResultsSize;
                USBdev1Controller.TaskManager.CreateTask(GenCallingTask);
                USBdev1Controller.TaskManager.InitiateTaskProcessing();
                dev1Started = !dev1Started;
            }
        }

        private void SerialOverDev2TaskInit()
        {
            if (!dev2Started && dev2Connected)
            {
                USBdev2Controller.TaskQueueSize = GenConstants.GenConstants.taskQueueSize;
                USBdev2Controller.TransmissionResultsSize = GenConstants.GenConstants.transmissionResultsSize;
                USBdev2Controller.TaskManager.CreateTask(GenCallingTask);
                USBdev2Controller.TaskManager.InitiateTaskProcessing();
                dev2Started = !dev2Started;
            }
        }

        private void ModemRS232TtlTaskInit()
        {
            if (!rs232TtlStarted && rs232TtlConnected)
            {
                USBmodemController.TaskQueueSize = GenConstants.GenConstants.taskQueueSize;
                USBmodemController.TransmissionResultsSize = GenConstants.GenConstants.transmissionResultsSize;
                USBmodemController.TaskManager.CreateTask(GenCallingTask);
                USBmodemController.TaskManager.InitiateTaskProcessing();
                rs232TtlStarted = !rs232TtlStarted;
            }
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

        private void CtrlStartAllBtn_Click(object sender, EventArgs e)
        {
            SerialOverDev1TaskInit();
            SerialOverDev2TaskInit();
            ModemRS232TtlTaskInit();
            RS485TaskInit();
            RS232TaskInit();
        }

        private void CtrlStopAllBtn_Click(object sender, EventArgs e)
        {
            StopAllTransmissions();
        }

        private void StopAllTransmissions()
        {
            USBmodemController.TaskManager.CreateTask(StopAllTask);
            USBdev2Controller.TaskManager.CreateTask(StopAllTask);
            USBdev1Controller.TaskManager.CreateTask(StopAllTask);
            RS485controller.TaskManager.CreateTask(StopAllTask);
            RS232controller.TaskManager.CreateTask(StopAllTask);
            rs232TtlStarted = false;
            rs485Started = false;
            rs232Started = false;
            dev1Started = false;
            dev2Started = false;
        }

        private void CtrlResetAllBtn_Click(object sender, EventArgs e)
        {
            localResultsModem.ZeroCounters();
            localResultsDev2.ZeroCounters();
            localResultsDev1.ZeroCounters();
            localResultsRS485.ZeroCounters();
            localResultsRS232.ZeroCounters();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StopAllTransmissions();
            portsInUse.Clear();
            Thread.Sleep(1000);
            USBmodemController.Disconnect();
            USBdev2Controller.Disconnect();
            USBdev1Controller.Disconnect();
            RS485controller.Disconnect();
            RS232controller.Disconnect();
            RS485comBox.Enabled = true;
            RS232comBox.Enabled = true;
            USBdev1ComBox.Enabled = true;
            USBdev2ComBox.Enabled = true;
            USBmodemComBox.Enabled = true;
            rs232StatusLbl.Text = "";
            rs485StatusLbl.Text = "";
            dev1StatusLbl.Text = "";
            dev2StatusLbl.Text = "";
            rs232ttlStatusLbl.Text = "";
            rs232Connected = false;
            rs485Connected = false;
            dev1Connected = false;
            dev2Connected = false;
            rs232TtlConnected = false;
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
