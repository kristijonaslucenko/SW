using System;

namespace EmcProtocol
{
    public static class ControlUsbHostResponse
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.ControlUsbHostResponse;
        public const byte UsbStatePosition = Protocol.V1.LengthPosition + 1;
        public const byte DataLength = 1;
        public const byte ByteSize = Protocol.HeaderLength + Protocol.CrcLength + DataLength;

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
    }
}
