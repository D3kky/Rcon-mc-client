using BenchmarkDotNet.Attributes;
using Client.Encoder;
using Client.Models;

namespace Benchmarks.Models;

[MemoryDiagnoser]
public class EncoderBenchmark
{
  private Packet _packet = default!;
  private byte[] _packetsRecieved = default!;

  private readonly Encoder _encoder = new();

  [Params(10, 100, 1000)]
  public int StringCount { get; set; }

  [GlobalSetup]
  public void Setup()
  {
    SetUpEncoder();
    SetUpDecoder();
  }
  [Benchmark]
  public ReadOnlySpan<byte> Encode()
    => _encoder.Encode(_packet);

  [Benchmark]
  public Packet Decode()
    => _encoder.Decode(_packetsRecieved);

  void SetUpEncoder()
    => _packet = new()
        {
          Id = (uint)StringCount,
          Type = (uint)StringCount,
          Payload = new string('i', StringCount)
        };

  void SetUpDecoder()
    => _packetsRecieved = _encoder.Encode(_packet).ToArray();
}