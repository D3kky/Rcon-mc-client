// See https://aka.ms/new-console-template for more information
using System.Net;
using Client.Models;

var encoder = new Encoder();

var ip = IPAddress.Loopback.ToString();
var client = new RconClient(encoder, ip, "lmaolol123");

var res = client.SendCommand("whitelist list");
Console.WriteLine(res);

Console.ReadLine();