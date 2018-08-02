using System;

namespace EmcProtocol
{
    public static class LastKeySeen
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.LastKeySeen;
        public const byte KeyPosition = Protocol.V1.LengthPosition + 1;
        public const byte EventPosition = Protocol.V1.LengthPosition + 2;
        public const byte DataLength = 2;
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

        public static char Key(ref byte[] data) => (char)data[KeyPosition];

        public static byte Event(ref byte[] data) => data[EventPosition];

        public static byte[] CreateTestResponseMessage(byte key, byte evt)
        {
            var data = new byte[ByteSize];
            Protocol.V1.CreateHeader(ref data, Type, DataLength);
            data[KeyPosition] = key;
            data[EventPosition] = evt;
            Protocol.V1.AddCrc(ref data);
            return data;
        }
    }
}
