using Client.Models;

namespace Client.Intefaces;

public interface IEncoder
{
  public byte[] Encode(Packet packet);
  public Packet Decode(ReadOnlySpan<byte> response);
}