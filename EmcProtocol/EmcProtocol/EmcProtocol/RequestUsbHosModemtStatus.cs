using System;

namespace EmcProtocol
{
    public static class RequestUsbHostModemStatus
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.RequestUsbHostModemStatus;
        public const byte DataLength = 0;
        public const byte ByteSize = Protocol.HeaderLength + Protocol.CrcLength + DataLength;

        public static byte[] Create()
        {
            var data = new byte[ByteSize];
            Protocol.V1.CreateHeader(ref data, Type, DataLength);
            Protocol.V1.AddCrc(ref data);
            return data;
        }
    }
}
