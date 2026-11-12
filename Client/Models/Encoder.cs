using System.Buffers.Binary;
using System.Text;
using Client.Intefaces;
using static Client.Constants.PacketSizes;

namespace Client.Models;

public sealed class Encoder : IEncoder
{
  public Packet Decode(ReadOnlySpan<byte> response)
  {
    var size = BinaryPrimitives.ReadUInt32LittleEndian(response[0..4]) - SizeBase;

    return new()
    {
      Id = BinaryPrimitives.ReadInt32LittleEndian(response[4..8]),
      Type = BinaryPrimitives.ReadInt32LittleEndian(response[8..12]),
      Payload = Encoding.ASCII.GetString(response.Slice(12, (int)size))
    };
  }

  public byte[] Encode(Packet packet)
  {
    Span<byte> buffer = stackalloc byte[LengthBase + packet.Payload.Length];

    BinaryPrimitives.TryWriteInt32LittleEndian(buffer[..4], SizeBase + packet.Payload.Length);
    BinaryPrimitives.TryWriteInt32LittleEndian(buffer[4..8], packet.Id);
    BinaryPrimitives.TryWriteInt32LittleEndian(buffer[8..12], packet.Type);
    Encoding.ASCII.TryGetBytes(packet.Payload, buffer.Slice(12, packet.Payload.Length), out var _);
    buffer[^1] = 0;    

    return buffer.ToArray();
  }
}