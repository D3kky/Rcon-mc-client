// See https://aka.ms/new-console-template for more information
using Client.Models;

var encoder = new Encoder();

var client = new RconClient(encoder, "192.168.0.19", "lmaolol123");

var res = client.SendCommand("whitelist list");
Console.WriteLine(res);

Console.ReadLine();