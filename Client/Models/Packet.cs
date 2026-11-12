namespace Client.Models;

public sealed record Packet
{
  public required int Id { get; init; }
  public required int Type { get; init; }
  public string Payload { get; init; } = string.Empty;
}