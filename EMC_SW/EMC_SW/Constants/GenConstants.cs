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
        public const int BaudRate = 20000000;
        public const int SentPacketListSize = 256;
        public const int ReceivedPacketListSize = 256;
        public const int ResultListSize = 256;
        public const int SerialBufferReadSize = 256;
        public const byte StartByte = 0x58;
        public const byte StopByte = 0x59;
        public static readonly byte[] ErrorValue = new byte[] { 0x10, 0x11, 0x12, 0x13 };
        public const int CallTaskId = 1;
        public const int ControlDisplayTaskId = 2;
        public const int ControlUsbHostTaskId = 3;
        public const int RequestLastSeenKeyTaskId = 4;
        public const int RequestDisplayStateTaskId = 5;
        public const int RequestUsbHostStatusTaskId = 6;
        public const int StopTaskId = 7;
        public const int SerialReadTimeout = 200; //ms
        public const int taskQueueSize = 200;
        public const int transmissionResultsSize = 200;

        public const int controlUsbHostNth = 0;
        public const string controlUsbHostNthText = "-";
        public const int controlUsbHostResponseRWrunning = 1;
        public const int controlUsbHostResponseNotAccessible = 2;
        public const string controlUsbHostTextBoxValueRWRunning = "R/W Running";
        public const string controlUsbHostTextBoxValueNotAcc = "Not Accessible";
        public const int controlDisplayResponseNui = 0;
        public const int controlDisplayResponseTui = 1;
        public const string controlDisplayResponseNuiText = "Normal UI";
        public const string controlDisplayResponseTuiText = "Test UI";
        public const int requestedLastKeyNA = 0;
        public const string requestedLastKeyNAtext = "N/A";
        public const int keyPressStatusNoEv = 0;
        public const string keyPressStatusNoEvText = "N/A";
        public const int keyPressStatusPrNotRel = 1;
        public const string keyPressStatusPrNotRelText = "Pressed, not released";
        public const int keyPressStatusPrRel = 2;
        public const string keyPressStatusPrRelText = "Pressed, released";
        public const int displayStateNotConn = 0;
        public const string displayStateNotConnText = "Not Connected";
        public const int displayStateConnected = 1;
        public const string displayStateConnectedText = "Connected";
        public const int usbHostStatusNA = 0;
        public const string usbHostStatusNAtext = "Not Accessible";
        public const int usbHostStatusRWrunn = 1;
        public const string usbHostStatusRWrunnText = "R/W running";
        public const int usbHostStatusNotAcc = 2;
        public const string usbHostStatusNotAccText = "Not Accessible";
    }
}