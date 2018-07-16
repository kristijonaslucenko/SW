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

        TaskLUP InitiateLCDinTestMode = null;
        TaskLUP InitiateUsbWriting = null;
        TaskLUP CallingTask = null;
        TaskLUP RequestLastSeenKey = null;
        TaskLUP RequestDisplayState = null;
        TaskLUP RequestUsbHostStatus = null;
        TaskLUP ReturnLcdToNormalMode = null;
        TaskLUP StopUsbWriting = null;

        public MainForm()
        {
            InitializeComponent();
            RS232controller = new MainController();
            RS485controller = new MainController();
            USBhostController = new MainController();
            USBdeviceController = new MainController();
            USBmodemController = new MainController();


            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                RS232comBox.Items.Add(s);
                RS485comBox.Items.Add(s);
                USBmodemComBox.Items.Add(s);
                USBhostComBox.Items.Add(s);
                USBdeviceComBox.Items.Add(s);
            }

            RS232controller.SampleSize = 1024;
            RS232controller.SentSampleSize = 1024;
            //RS485controller.SampleSize = 10;
        }

        private void RS232startBtn_Click(object sender, EventArgs e)
        {

        }

        private void COMconnectBtn_Click(object sender, EventArgs e)
        {
            String RS232port = RS232comBox.GetItemText(RS232comBox.SelectedItem);
            String RS485port = RS485comBox.GetItemText(RS485comBox.SelectedItem);
            String USBhostPort = USBhostComBox.GetItemText(USBhostComBox.SelectedItem);
            String USBdevicePort = USBdeviceComBox.GetItemText(USBdeviceComBox.SelectedItem);
            String USBmodemPort = USBmodemComBox.GetItemText(USBmodemComBox.SelectedItem);

            if (RS232port != "")
            {
                RS232controller.Start(RS232port);
                
                //GUIrefreshTimer.Enabled = true;
            }

            /*if (RS485port != "")
            {
                RS485controller.Start(RS485port);
            }

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
         
        private void button1_Click(object sender, EventArgs e)
        {
            //Byte[] buffer1, buffer2;
            /*buffer1 = new Byte[textBox17.Text.Length];
            buffer2 = new Byte[textBox18.Text.Length];
            ASCIIEncoding.ASCII.GetBytes(textBox17.Text, 0, textBox17.Text.Length, buffer1, 0);
            ASCIIEncoding.ASCII.GetBytes(textBox18.Text, 0, textBox18.Text.Length, buffer2, 0);
            RS232controller.SendData(buffer1);
            RS485controller.SendData(buffer2);*/
            //RS232controller.SendData(ConstructPacket(this.count));
            /*Byte[] buffer1;
            buffer1 = EmcProtocol.Call.Create();
            RS232controller.SendData(buffer1);*/


            CallingTask = new TaskLUP
            {
                AddedTask = EmcProtocol.Call.Create(),
                id = GenConstants.GenConstants.CallingTaskId,
                Repetition = 10,
                IsContinuous = true
            };

            StopUsbWriting = new TaskLUP
            {
                AddedTask = null,
                id = GenConstants.GenConstants.StopUsbWritingTaskId,
                Repetition = 0,
                IsContinuous = false
            };
            RS232controller.TaskQueueSize = 2;
            RS232controller.MyTasker.CreateTask(CallingTask);
            RS232controller.MyTasker.CreateTask(StopUsbWriting);
            RS232controller.MyTasker.InitiateTaskProcessing();
            //RS232controller.MyTasker.ProcessTasks();
            //Debug.Print();

        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
           /*for (int i = 0; i < RS232controller.CurrentSample.Limit; ++i)
            {
                byte[] receivedPacket = RS232controller.CurrentSample.Get(i);
                byte[] sentPacket = RS232controller.SentSample.Get(i);

                if (receivedPacket != null)
                {
                    //string s = Encoding.GetEncoding("Windows-1252").GetString(value);
                    //Debug.Print(s);
                   
                    Debug.Print("Received sample list: ");
                    Debug.Print(BitConverter.ToString(receivedPacket));
                }

                if(sentPacket != null)
                {
                    Debug.Print("Sent sample list: ");
                    Debug.Print(BitConverter.ToString(sentPacket));
                }
            }/*
            //Debug.Print("out of rs232");
            for (int i = 0; i < RS485controller.CurrentSample.Limit; ++i)
            {
                byte[] value = RS485controller.CurrentSample.Get(i);
                //Debug.Print("In rs485");

                if (value != null)
                {
                    string s = Encoding.GetEncoding("Windows-1252").GetString(value);
                    Debug.Print(s);
                }
            }*/

        }

        
    }
}
