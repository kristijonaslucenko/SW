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
            _serialPort = new SerialPort();
        }

        public override void Open(String port, int baudrate)
        {
            try
            {
                _serialPort.PortName = port;
                _serialPort.BaudRate = baudrate;
                _serialPort.ReadTimeout = GenConstants.GenConstants.SerialReadTimeout;
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), _serialPort.PortName);
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
                int bToRead = _serialPort.BytesToRead;
                if (bToRead >= BufferSize)
                {
                    _serialPort.Read(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException)
                {
                    timeout = true;
                    return null;
                }
                else
                {
                    MessageBox.Show("Error: " + ex.ToString(), _serialPort.PortName);
                    timeout = true;
                    return null;
                }
            }
            timeout = false;
            return buffer;
        }

        protected override void WriteData(byte[] buffer)
        {
            try
            {
                _serialPort.Write(buffer, 0, buffer.Length);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), _serialPort.PortName);
            }
        }

    }
}
