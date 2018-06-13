using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        }

        private void RS232startBtn_Click(object sender, EventArgs e)
        {

        }

        private void COMconnectBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(RS232comBox.GetItemText(RS232comBox.SelectedItem), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            String RS232port = RS232comBox.GetItemText(RS232comBox.SelectedItem);
            String RS485port = RS485comBox.GetItemText(RS485comBox.SelectedItem);
            String USBhostPort = USBhostComBox.GetItemText(USBhostComBox.SelectedItem);
            String USBdevicePort = USBdeviceComBox.GetItemText(USBdeviceComBox.SelectedItem);
            String USBmodemPort = USBmodemComBox.GetItemText(USBmodemComBox.SelectedItem);

            if (RS232port != "")
            {
                RS232controller.Start(RS232port);
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

        }
    }
}
