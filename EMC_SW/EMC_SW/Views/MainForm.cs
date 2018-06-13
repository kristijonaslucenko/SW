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

namespace EMC_SW
{
    public partial class MainForm : Form
    {
        MainController RS232controller = null;
        MainController RS485controller = null;
        MainController USBhostController = null;
        MainController USBdeviceController = null;
        MainController USBmodemController = null;
        private byte count = 0;

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

            RS232controller.SampleSize = 10;
            RS485controller.SampleSize = 10;
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
                timer1.Enabled = true;
            }

            if (RS485port != "")
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
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Byte[] buffer1, buffer2;
            /*buffer1 = new Byte[textBox17.Text.Length];
            buffer2 = new Byte[textBox18.Text.Length];
            ASCIIEncoding.ASCII.GetBytes(textBox17.Text, 0, textBox17.Text.Length, buffer1, 0);
            ASCIIEncoding.ASCII.GetBytes(textBox18.Text, 0, textBox18.Text.Length, buffer2, 0);
            RS232controller.SendData(buffer1);
            RS485controller.SendData(buffer2);*/
            RS232controller.SendData(ConstructPacket(this.count));
            

        }

        private byte[] ConstructPacket(byte increment)
        {
            byte StartByte = 0x58;
            byte StopByte = 0x59;
            Byte[] PacketToSend = new byte[3];
            PacketToSend[0] = StartByte;
            PacketToSend[1] = increment;
            PacketToSend[2] = StopByte;
            this.count++;
            return PacketToSend;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < RS232controller.CurrentSample.Limit; ++i)
            {
                byte[] value = RS232controller.CurrentSample.Get(i);

                if (value != null)
                {
                    //string s = Encoding.GetEncoding("Windows-1252").GetString(value);
                    //Debug.Print(s);
                    Debug.Print(BitConverter.ToString(value));
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
