using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UseNetNewsReader.Services;

public class NntpService
{
    private TcpClient? _client;
    public string? Result { get; private set; } 
    public bool IsConnected { get; private set; }
    public async Task ConnectAsync(string ip, int port)
    {
        _client = new TcpClient();
        
        await _client.ConnectAsync(ip, port);
        NetworkStream data = _client.GetStream();
        StreamReader dataReader = new StreamReader(data);
        
        Result = await dataReader.ReadLineAsync();
        IsConnected = true;
    }
}