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
        MainController USBhostController = null;
        MainController USBdeviceController = null;
        MainController USBmodemController = null;

        LocalResults localResultsRS232 = null;
        LocalResults localResultsRS485 = null;

        TaskLUP InitiateLCDinTestMode = null;
        TaskLUP InitiateUsbWriting = null;
        TaskLUP CallingTask = null;
        TaskLUP RequestLastSeenKey = null;
        TaskLUP RequestDisplayState = null;
        TaskLUP RequestUsbHostStatus = null;
        TaskLUP ReturnLcdToNormalMode = null;
        TaskLUP UsbControlTask = null;

        public MainForm()
        {
            InitializeComponent();
            RS232controller = new MainController();
            RS485controller = new MainController();
            USBhostController = new MainController();
            USBdeviceController = new MainController();
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
            CallingTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(),
                id = GenConstants.GenConstants.CallTaskId,
                Repetition = 1,
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
            RS232controller.MyTasker.CreateTask(CallingTask);
            //RS232controller.MyTasker.CreateTask(UsbControlTask);
            RS232controller.MyTasker.InitiateTaskProcessing();
        }

        private void RS485startBtn_Click(object sender, EventArgs e)
        {
            CallingTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(),
                id = GenConstants.GenConstants.CallTaskId,
                Repetition = 1,
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
            RS485controller.MyTasker.CreateTask(CallingTask);
            RS485controller.MyTasker.InitiateTaskProcessing();
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
                RS485controller.TransmissionResults.Dequeue();
            }
            RS485textboxT.Text = localResultsRS485.transmittedPackets.ToString();
            RS485textboxR.Text = localResultsRS485.receivedPackets.ToString();
            RS485textboxM.Text = localResultsRS485.missedPackets.ToString();
            RS485textboxE.Text = localResultsRS485.errorsInTransmission.ToString();
        }

        
    }
    public class LocalResults
    {
        public int transmittedPackets { get; set; }
        public int receivedPackets { get; set; }
        public int missedPackets { get; set; }
        public int errorsInTransmission { get; set; }
        public int status { get; set; }
        public int opErrors { get; set; }
    }

}
