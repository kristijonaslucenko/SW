using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EMC_SW.GenConstants;

namespace EMC_SW.DataHandlers
{

    public class SerialDataHandler : BaseDataHandler
    {
        private SerialPort _serialPort;

        public SerialDataHandler()
        {
            _serialPort = new SerialPort
            {
                BaudRate = GenConstants.GenConstants.BaudRate
            };
        }

        public override void Open(String port)
        {
            try
            {
                _serialPort.PortName = port;
                _serialPort.ReadTimeout = GenConstants.GenConstants.SerialReadTimeout;
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

        protected override byte[] ReadData(int BufferSize, out bool timeout)
        {
            byte[] buffer = new byte[BufferSize];
            try
            {
                _serialPort.Read(buffer, 0, buffer.Length);
            }
            catch (TimeoutException)
            {
                timeout = true;
                return null;
            }
            timeout = false;
            return buffer;
        }

        protected override void WriteData(byte[] buffer)
        {
            _serialPort.Write(buffer, 0, buffer.Length);
        }

    }
}
