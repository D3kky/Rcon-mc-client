using Client.Intefaces;
using Client.Models;

namespace Tests.Models;

public class EncoderTests
{
  private readonly IEncoder _sut = new Encoder();

  [Theory]
  [InlineData(new byte[] { 0b1101, 0b0, 0b0, 0b0, 0b1, 0b0, 0b0, 0b0, 0b1, 0b0, 0b0, 0b0, 0b1110100, 0b1100101, 0b1110011, 0b1110100, 0b0 }, 1, 1, "test")]
  [InlineData(new byte[] { 0b1101, 0b0, 0b0, 0b0, 0b11, 0b0, 0b0, 0b0, 0b10, 0b0, 0b0, 0b0, 0b01101100, 0b01101101, 0b01100001, 0b01101111, 0b0 }, 3, 2, "lmao")]
  [InlineData(new byte[] { 0b1110, 0b0, 0b0, 0b0, 0b1100, 0b0, 0b0, 0b0, 0b11, 0b0, 0b0, 0b0, 0b01010101, 0b01110000, 0b01110000, 0b01100101, 0b01110010, 0b0 }, 12, 3, "Upper")]
  [InlineData(new byte[] { 0b1110, 0b0, 0b0, 0b0, 0b1100, 0b0, 0b0, 0b0, 0b11, 0b0, 0b0, 0b0, 0b00110001, 0b00110010, 0b00110011, 0b00110100, 0b00110101, 0b0 }, 12, 3, "12345")]
  public void Encode_Encodes_ReturnsCorrectPacket(byte[] bytes, uint id, uint type, string payload)
  {
    Packet expected = new()
    {
      Id = id,
      Type = type,
      Payload = payload
    };

    var result = _sut.Decode(bytes);

    Assert.Equal(expected, result);
  }

  [Theory]
  [InlineData(1, 1, "test", new byte[] { 0b1101, 0b0, 0b0, 0b0, 0b1, 0b0, 0b0, 0b0, 0b1, 0b0, 0b0, 0b0, 0b1110100, 0b1100101, 0b1110011, 0b1110100, 0b0 })]
  [InlineData(3, 2, "lmao", new byte[] { 0b1101, 0b0, 0b0, 0b0, 0b11, 0b0, 0b0, 0b0, 0b10, 0b0, 0b0, 0b0, 0b01101100, 0b01101101, 0b01100001, 0b01101111, 0b0 })]
  [InlineData(12, 3, "Upper", new byte[] { 0b1110, 0b0, 0b0, 0b0, 0b1100, 0b0, 0b0, 0b0, 0b11, 0b0, 0b0, 0b0, 0b01010101, 0b01110000, 0b01110000, 0b01100101, 0b01110010, 0b0 })]
  [InlineData(12, 3, "12345", new byte[] { 0b1110, 0b0, 0b0, 0b0, 0b1100, 0b0, 0b0, 0b0, 0b11, 0b0, 0b0, 0b0, 0b00110001, 0b00110010, 0b00110011, 0b00110100, 0b00110101, 0b0 })]
  public void Decode_Decodes_ReturnsCorrectByteArray(uint id, uint type, string payload, byte[] expectedBytes)
  {
    Packet packet = new()
    {
      Id = id,
      Type= type,
      Payload = payload
    };

    var result = _sut.Encode(packet);

    Assert.Equal(expectedBytes, result);
  }
}