using System;

namespace EmcProtocol
{
    public static class DisplayState
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.DisplayState;
        public const byte ConnectedPosition = Protocol.V1.LengthPosition + 1;
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

        public static bool Connected(ref byte[] data) => data[ConnectedPosition] == 1 ? true : false;

        public static byte[] CreateTestResponseMessage(byte state)
        {
            var data = new byte[ByteSize];
            Protocol.V1.CreateHeader(ref data, Type, DataLength);
            data[ConnectedPosition] = state;
            Protocol.V1.AddCrc(ref data);
            return data;
        }
    }
}
