using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

Console.WriteLine("=== RSA Digital Signature: Verifier ===");
TcpListener listener = new TcpListener(IPAddress.Loopback, 6000);
listener.Start();
Console.WriteLine("Waiting for Proxy...");
using TcpClient client = listener.AcceptTcpClient();
NetworkStream stream = client.GetStream();

byte[] buffer = new byte[8192];
int len = stream.Read(buffer, 0, buffer.Length);
string json = Encoding.UTF8.GetString(buffer, 0, len);

var doc = JsonDocument.Parse(json);
string message = doc.RootElement.GetProperty("message").GetString();
string signatureB64 = doc.RootElement.GetProperty("signature").GetString();
string publicKeyXml = doc.RootElement.GetProperty("publicKey").GetString();

// Import public key and verify
using RSA rsa = RSA.Create();
rsa.FromXmlString(publicKeyXml);
byte[] data = Encoding.UTF8.GetBytes(message);
byte[] signature = Convert.FromBase64String(signatureB64);

bool valid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
Console.WriteLine(valid ? "Signature is VALID." : "Signature is INVALID or has been tampered.");

listener.Stop();