using System;

namespace EmcProtocol
{
    public static class ControlUsbHostModem
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.ControlUsbHostModem;
        public const byte UiPosition = Protocol.V1.LengthPosition + 1;
        public const byte DataLength = 1;
        public const byte ByteSize = Protocol.HeaderLength + Protocol.CrcLength + DataLength;

        public static byte[] StartUsbReadWrite()
        {
            return Create((byte)Protocol.V1.UsbState.Start);
        }

        public static byte[] StopUsbReadWrite()
        {
            return Create((byte)Protocol.V1.UsbState.Stop);
        }

        private static byte[] Create(byte uiTarget)
        {
            var data = new byte[ByteSize];
            Protocol.V1.CreateHeader(ref data, Type, DataLength);
            data[UiPosition] = uiTarget;
            Protocol.V1.AddCrc(ref data);
            return data;
        }
    }
}
