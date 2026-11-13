using System.Net.Sockets;
using Client.Enums;
using Client.Intefaces;
using static Client.Constants.PacketSizes;

namespace Client.Models;

public sealed class RconClient : IRconClient, IDisposable
{
  private const int _rconDefaultPort = 25575;
  private int _packetId = 0;

  private readonly string _password;
  private readonly IEncoder _encoder;
  private readonly TcpClient _tcpClient;
  private readonly NetworkStream _stream;

  public RconClient(IEncoder encoder, string ip, int port, string password)
  {
    _encoder = encoder;
    _tcpClient = new(ip, port);
    _stream = _tcpClient.GetStream();
    _password = password;

    Connect();
  }

  // Default port
  public RconClient(IEncoder encoder, string ip, string password)
  {
    _encoder = encoder;
    _tcpClient = new(ip, _rconDefaultPort);
    _stream = _tcpClient.GetStream();
    _password = password;

    Connect();
  }

  public string SendCommand(string command)
  {
    var id = Interlocked.Increment(ref _packetId);

    _stream.Write(_encoder.Encode(new() { Id = id, Type = (int)RequestTypes.COMMAND, Payload = command }));
    
    Span<byte> buffer = stackalloc byte[MaxPacketSize];
    var bytesWritten = _stream.Read(buffer);
    
    var packet = _encoder.Decode(buffer[0..bytesWritten]);
    if (id != packet.Id) throw new Exception("Response didn't return original request");

    return packet.Payload;
  }

  private void Connect()
  {
    var connect = new Packet()
    {
      Id = 1,
      Type = (int)RequestTypes.AUTH,
      Payload = _password
    };

    _stream.Write(_encoder.Encode(connect));

    Span<byte> buffer = stackalloc byte[EmptySize];
    _ = _stream.Read(buffer);

    var res = _encoder.Decode(buffer);
    if (res.Id == -1) throw new Exception("Connecting to the server failed, wrong password");
    if (res.Id != connect.Id) throw new Exception("Resposne didn't match original request");    
  }

  public void Dispose()
    => _tcpClient.Dispose();
}