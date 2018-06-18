using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMC_SW.GenConstants
{
    static class GenConstants
    {
        public const int PacketSize = 3;
        public const int BaudRate = 115200;
        public const int SentPacketListSize = 256;
        public const int ReceivedPacketListSize = 256;
        public const int ResultListSize = 256;
        public const int SerialBufferReadSize = 256;
        public const byte StartByte = 0x58;
        public const byte StopByte = 0x59;
        public static readonly byte[] ErrorValue = new byte[] {0x10, 0x11, 0x12, 0x13};
    }
}
