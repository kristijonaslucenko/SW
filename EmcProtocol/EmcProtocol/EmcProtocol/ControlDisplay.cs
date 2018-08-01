using System;

namespace EmcProtocol
{
  public static class ControlDisplay
  {
    public const Protocol.V1.Type Type = Protocol.V1.Type.ControlDisplay;
    public const byte UiPosition = Protocol.V1.LengthPosition + 1;
    public const byte DataLength = 1;
    public const byte ByteSize = Protocol.HeaderLength + Protocol.CrcLength + DataLength;

    public static byte[] CreateSwitchToTestUI()
    {
      return Create((byte)Protocol.V1.UiState.TestUi);
    }

    public static byte[] CreateSwitchToNormalUI()
    {
      return Create((byte)Protocol.V1.UiState.NormalUi);
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
