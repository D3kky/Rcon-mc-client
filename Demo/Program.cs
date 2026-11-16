using System.Net;
using Client.Models;

var encoder = new Encoder();

var localPass = "lmaolol123";
var ip = IPAddress.Loopback.ToString();

var client = await RconClient.CreateAsync(encoder, ip, localPass);

while (true)
{
  var userInput = Console.ReadLine();
  if (userInput == "q") return;

  _ = HandleCommandAsync(userInput ?? string.Empty);
}

async Task HandleCommandAsync(string command)
  => Console.WriteLine(await client.SendCommandAsync(command));