namespace Client.Intefaces;

public interface IRconClient
{
  public string SendCommand(string command);
  public Task<string> SendCommandAsync(string command);
}