using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UseNetNewsReader.Services;

public class NntpService
{
    private TcpClient? _client;
    
    public bool IsConnected;
    public async Task ConnectAsync(string ip, int port)
    {
        _client = new TcpClient();
        
        await _client.ConnectAsync(ip, port);
        
        if (!_client.Connected)
        {
            Console.WriteLine("Nntp connection failed");
            IsConnected = false;
        }
        IsConnected = true;
        Console.WriteLine("Nntp connected");
    }
    
}