using System.Net.Sockets;
using Client.Enums;
using Client.Intefaces;
using static Client.Constants.PacketSizes;

namespace Client.Models;

public sealed class RconClient : IRconClient, IDisposable
{
  private const int _rconDefaultPort = 25575;
  private int _packetId = 0;

  private readonly IEncoder _encoder;
  private readonly TcpClient _tcpClient;
  private readonly NetworkStream _stream;

  // Hopefully goes away in the future, plan is to use system.io.channels
  private readonly SemaphoreSlim _messageSemaphore;

  private RconClient(IEncoder encoder, TcpClient tcpClient)
  {
    _messageSemaphore = new(1, 1);

    _encoder = encoder;
    _tcpClient = tcpClient;
    _stream = tcpClient.GetStream();
  }

  public static async Task<RconClient> CreateAsync(IEncoder encoder, string ip, string password)
    => await CreateAsync(encoder, ip, _rconDefaultPort, password);

  public static async Task<RconClient> CreateAsync(IEncoder encoder, string ip, int port, string password)
  {
    var client = new TcpClient();
    var stream = client.GetStream();

    await client.ConnectAsync(ip, port);

    await stream.WriteAsync(encoder.Encode(new() { Id = 0, Type = (int)RequestTypes.AUTH, Payload = password }));
    var buffer = new byte[EmptySize];

    await stream.ReadAsync(buffer);
    var res = encoder.Decode(buffer);

    if (res.Id != 0) throw new ArgumentException("Connection failed, password wrong");

    return new(encoder, client);
  }

  public string SendCommand(string command)
  {
    _messageSemaphore.Wait();

    try
    {
      _stream.Write(_encoder.Encode(new() { Id = _packetId++, Type = (int)RequestTypes.COMMAND, Payload = command }));

      Span<byte> buffer = stackalloc byte[MaxPacketSize];
      var bytesWritten = _stream.Read(buffer);

      var packet = _encoder.Decode(buffer[0..bytesWritten]);

      return packet.Payload;
    }
    finally
    {
      _messageSemaphore.Release();
    }
  }

  public void Dispose()
    => _tcpClient.Dispose();
}