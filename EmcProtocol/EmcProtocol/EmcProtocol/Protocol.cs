using System;

namespace EmcProtocol
{
    public static class Protocol
    {
        public const byte HeaderLength = 3;
        public const byte CrcLength = 4;
        public const byte VersionPosition = 0;

        public static class V1
        {
            public const byte VersionCode = 1;
            public const byte TypePosition = 1;
            public const byte LengthPosition = 2;

            public enum Type : byte
            {
                Call = 0x01,//
                Ack = 0x02,//
                RequestLastSeenKey = 0x03,//
                LastKeySeen = 0x04,//
                RequestDisplayState = 0x05,//
                DisplayState = 0x06,//
                ControlDisplay = 0x07,//
                ControlDisplayResponse = 0x08,//
                ControlUsbHost = 0x09,
                ControlUsbHostResponse = 0x0A,
                RequestUsbHostStatus = 0x0B,
                UsbHostStatus = 0x0C,
                RequestUsbHostModemStatus = 0x0D,
                UsbHostModemStatus = 0x0E,
                ControlUsbHostModem = 0x0F,
                ControlUsbHostModemResponse = 0x10,
            }

            public enum UiState : byte
            {
                NormalUi = 0,
                TestUi = 1
            }

            public enum UsbState : byte
            {
                Start = 0,
                Stop = 1
            }

            public static void CreateHeader(ref byte[] data, Type type, byte Length)
            {
                data[Protocol.VersionPosition] = Protocol.V1.VersionCode;
                data[Protocol.V1.TypePosition] = (byte)type;
                data[Protocol.V1.LengthPosition] = Length;
            }

            public static void AddCrc(ref byte[] data)
            {
                Crc32 crc32 = new Crc32();
                crc32.Initialize();
                var offset = data.Length - Protocol.CrcLength;
                var result = crc32.ComputeHash(data, 0, offset);
                for (var i = 0; i + offset < data.Length && i < result.Length; ++i)
                {
                    data[i + offset] = result[i];
                }
            }

            public static bool CompareCrc(ref byte[] data)
            {
                bool result = false;
                Crc32 crc32 = new Crc32();
                crc32.Initialize();
                var offset = data.Length - Protocol.CrcLength;
                var calculatedCrc = crc32.ComputeHash(data, 0, offset);
                var calculated = BitConverter.ToUInt32(calculatedCrc, 0);
                var sent = BitConverter.ToUInt32(data, offset);
                if (calculated == sent)
                {
                    result = true;
                }
                return result;
            }
        }
    }
}
