using System;

namespace EmcProtocol
{
    public static class UsbHostModemStatus
    {
        public const Protocol.V1.Type Type = Protocol.V1.Type.UsbHostModemStatus;
        public const byte StatusPosition = Protocol.V1.LengthPosition + 1;
        public const byte ErrorPosition = Protocol.V1.LengthPosition + 2;
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

        public static bool IsRunning(ref byte[] data)
        {
            if (data[StatusPosition] == 0)
            {
                return false;
            }
            else if (data[StatusPosition] == 1)
            {
                return true;
            }
            return false;
        }

        public static bool CanAccessMemoryStick(ref byte[] data) => data[StatusPosition] == 2 ? true : false; //sync protocol with MHK

        public static byte ErrorCount(ref byte[] data)
        {
            if (data[ErrorPosition] != 0)
            {
                return data[ErrorPosition];
            }
            return 0;
        }

        public static byte[] CreateTestResponseMessage(byte status, byte errorCount)
        {
            var data = new byte[ByteSize];
            Protocol.V1.CreateHeader(ref data, Type, DataLength);
            data[StatusPosition] = status;
            data[ErrorPosition] = errorCount;
            Protocol.V1.AddCrc(ref data);
            return data;
        }
    }
}

