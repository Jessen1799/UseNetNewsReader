using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UseNetNewsReader.Services;

public class NntpService
{
    private TcpClient? _client;
    private NetworkStream? _data;
    private StreamReader? _reader;
    
    public string? Result { get; private set; } 
    public bool IsConnected { get; private set; }
    public async Task ConnectAsync(string ip, int port)
    {
        _client = new TcpClient();
        
        await _client.ConnectAsync(ip, port);
        _data =  _client.GetStream();
        _reader = new StreamReader(_data);
        
        Result = await _reader.ReadLineAsync();
        IsConnected = true;
    }
}