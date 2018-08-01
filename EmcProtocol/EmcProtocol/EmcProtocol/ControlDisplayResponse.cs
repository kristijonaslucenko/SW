using System;

namespace EmcProtocol
{
    public static class ControlDisplayResponse
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.ControlDisplayResponse;
        public const byte UiPosition = Protocol.V1.LengthPosition + 1;
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

        public static Protocol.V1.UiState UiState(ref byte[] data) => (Protocol.V1.UiState)data[UiPosition];

        //0 = Currently in Normal UI, 1 = Currently in Test UI
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
