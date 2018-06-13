using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMC_SW.DataHandlers
{

    public class SerialDataHandler : BaseDataHandler
    {
        private SerialPort _serialPort;

        public SerialDataHandler()
        {
            _serialPort = new SerialPort();
            _serialPort.BaudRate = 9600;
        }

        public override void Open(String port)
        {
            try
            {
                _serialPort.PortName = port;
                _serialPort.Open();

                base.Open(port);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "ERROR");
            }
        }

        public override void Close()
        {
            _serialPort.Close();
        }

        protected override byte[] ReadData()
        {
            byte[] buffer = new byte[1024];
            _serialPort.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        protected override void WriteData(byte[] buffer)
        {
            _serialPort.Write(buffer, 0, buffer.Length);
        }

    }
}
