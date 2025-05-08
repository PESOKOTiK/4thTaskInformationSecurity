using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// Generate RSA key pair
using RSA rsa = RSA.Create(2048);
string publicKeyXml = rsa.ToXmlString(false);

Console.WriteLine("=== RSA Digital Signature: Signer ===");
Console.Write("Enter message to sign: ");
string message = Console.ReadLine();

// Compute signature
byte[] data = Encoding.UTF8.GetBytes(message);
byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
string signatureB64 = Convert.ToBase64String(signature);

// Package data
var payload = new { message, signature = signatureB64, publicKey = publicKeyXml };
string json = JsonSerializer.Serialize(payload);

// Send to Proxy (App2)
using TcpClient client = new TcpClient("127.0.0.1", 5000);
NetworkStream stream = client.GetStream();
byte[] bytes = Encoding.UTF8.GetBytes(json);
stream.Write(bytes, 0, bytes.Length);

Console.WriteLine("Signature and message sent to Proxy.");
Console.WriteLine("Public Key:\n" + publicKeyXml);