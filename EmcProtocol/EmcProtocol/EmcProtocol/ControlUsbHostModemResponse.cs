using System;

namespace EmcProtocol
{
    public static class ControlUsbHostModemResponse
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.ControlUsbHostModemResponse;
        public const byte UsbStatePosition = Protocol.V1.LengthPosition + 1;
        public const byte DataLength = 1;
        public const byte ByteSize = Protocol.HeaderLength + Protocol.CrcLength + DataLength;
        public const byte UiPosition = Protocol.V1.LengthPosition + 1;


        public static bool IsValid(ref byte[] data)
        {
            bool result = false;
            if (data.Length >= ByteSize &&
                data[Protocol.VersionPosition] == Protocol.V1.VersionCode &&
                data[Protocol.V1.TypePosition] == (byte)Type &&
                data[Protocol.V1.LengthPosition] == DataLength)
            {
                result = Protocol.V1.CompareCrc(ref data);
            }
            return result;
        }

        public static Protocol.V1.UsbState UsbStatus(ref byte[] data) => (Protocol.V1.UsbState)data[UsbStatePosition];

        //0 = Nothing, 1 = Read/Write cycle running, -1 if could not access memory stick
        public static byte[] CreateTestResponseMessage(byte dataByte)
        {
            var data = new byte[ByteSize];
            Protocol.V1.CreateHeader(ref data, Type, DataLength);
            data[UiPosition] = dataByte;
            Protocol.V1.AddCrc(ref data);
            return data;
        }
    }
}
