using System.Buffers;
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
    await client.ConnectAsync(ip, port);

    var stream = client.GetStream();

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

  public async Task<string> SendCommandAsync(string command)
  {
    await _messageSemaphore.WaitAsync();
    var buffer = ArrayPool<byte>.Shared.Rent(MaxPacketSize);

    try
    {
      await _stream.WriteAsync(_encoder.Encode(new() { Id = _packetId++, Type = (int)RequestTypes.COMMAND, Payload = command }));
      await _stream.ReadAsync(buffer);

      return _encoder.Decode(buffer).Payload;
    }
    finally
    {
      _messageSemaphore.Release();
      ArrayPool<byte>.Shared.Return(buffer);
    }
  }

  public void Dispose()
  => _tcpClient.Dispose();

}