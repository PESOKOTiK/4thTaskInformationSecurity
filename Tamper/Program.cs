using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

Console.WriteLine("=== RSA Digital Signature: Tampering Proxy ===");
TcpListener listener = new TcpListener(IPAddress.Loopback, 5000);
listener.Start();
Console.WriteLine("Waiting for Signer...");
using TcpClient client = listener.AcceptTcpClient();
NetworkStream inStream = client.GetStream();

// Read JSON
byte[] buffer = new byte[8192];
int len = inStream.Read(buffer, 0, buffer.Length);
string json = Encoding.UTF8.GetString(buffer, 0, len);

// Deserialize
var doc = JsonDocument.Parse(json);
string message = doc.RootElement.GetProperty("message").GetString();
string signature = doc.RootElement.GetProperty("signature").GetString();
string publicKey = doc.RootElement.GetProperty("publicKey").GetString();

Console.WriteLine($"Received message: {message}");
Console.WriteLine($"Original signature: {signature}");
Console.Write("Enter new signature (or press Enter to keep unchanged): ");
string input = Console.ReadLine();
if (!string.IsNullOrWhiteSpace(input)) signature = input;

// Forward to Verifier
var payload = new { message, signature, publicKey };
string forwardJson = JsonSerializer.Serialize(payload);
using TcpClient forward = new TcpClient("127.0.0.1", 6000);
byte[] outBytes = Encoding.UTF8.GetBytes(forwardJson);
forward.GetStream().Write(outBytes, 0, outBytes.Length);

Console.WriteLine("Data forwarded to Verifier.");
listener.Stop();