using System;

namespace EmcProtocol
{
    public static class Ack
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.Ack;
        public const byte DataLength = 0;
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
        public static byte[] Create()
        {
            var data = new byte[ByteSize];
            Protocol.V1.CreateHeader(ref data, Type, DataLength);
            Protocol.V1.AddCrc(ref data);
            return data;
        }
    }
}
