// See https://aka.ms/new-console-template for more information
using System.Net;
using Client.Models;

var encoder = new Encoder();

var localPass = "lmaolol123";
var ip = IPAddress.Loopback.ToString();
if (true)
{
  using var client = new RconClient(encoder, IPAddress.Loopback.ToString(), localPass);

  var res = client.SendCommand("time set day");

  Console.WriteLine(res);
}

Console.ReadLine();