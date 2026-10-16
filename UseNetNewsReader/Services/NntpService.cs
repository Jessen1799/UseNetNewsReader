using System.Collections.Generic;
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
    
    /// <summary>
    /// Connects asynchronously to server. Uses 'NetworkStream' and 'StreamReader'
    /// to read the stream of data from server. Receives confirmation msg upon connection success.
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public async Task ConnectAsync(string ip, int port)
    {
        _client = new TcpClient();
        
        await _client.ConnectAsync(ip, port);
        _data =  _client.GetStream();
        _reader = new StreamReader(_data);
        
        Result = await _reader.ReadLineAsync();
        IsConnected = true;
    }

    /// <summary>
    /// Disconnects service if there is a connection. Closes resources properly
    /// Secures references to used objects disappears when invoked.
    /// State changed on 'IsConnected'
    /// </summary>
    public void Disconnect()
    {
        //Lukker bare
        _reader?.Close();
        _client?.Close();

        //Fjerner referencer til objekterne, fordi de ikke skal 'eksisterer' mere.
        _reader = null;
        _client = null;
        
        IsConnected = false;
    }

    public async Task<List<string>> GetNewsGroups()
    {
        
        
    }
    
}