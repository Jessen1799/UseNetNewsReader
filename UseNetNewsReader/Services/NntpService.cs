using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UseNetNewsReader.Services;

public class NntpService : IConnection
{
    private TcpClient? _client;
    private NetworkStream? _data;
    private StreamReader? _reader;
    
    public string? ValidationMessage { get; private set; } 
    public bool IsConnected { get; private set; }
    
    /// <summary>
    /// Connects asynchronously to server. Uses 'NetworkStream' and 'StreamReader'
    /// to read the stream of data received from server. Receives confirmation msg upon connection success.
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public async Task ConnectAsync(string ip, int port)
    {
        _client = new TcpClient();
        
        await _client.ConnectAsync(ip, port);
        _data =  _client.GetStream();
        _reader = new StreamReader(_data);
        
        ValidationMessage = await _reader.ReadLineAsync();
        IsConnected = true;
        Console.WriteLine("Connected: " + IsConnected);
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
        Console.WriteLine("Disconnected: " + IsConnected);
        
    }

    public async Task<List<string>> GetNewsGroups()
    {
        List<string> resultList = new List<string>();
        var listCommando = Encoding.UTF8.GetBytes("LIST\r\n");

        //pointless? eller er det forbindelsen?
        if (_data != null)
        {
            await _data.WriteAsync(listCommando);
        }

        var resultSet = await _reader.ReadLineAsync();
        Console.WriteLine($"Modtaget fra server: {resultSet}");

        if (resultSet != null && resultSet.StartsWith("215"))
        {
            while (!resultSet.StartsWith("."))
            {
                resultSet = await _reader.ReadLineAsync();
                Console.WriteLine($"Modtaget fra server: {resultSet}");
                if (!resultSet.StartsWith("."))
                {
                    resultList.Add(resultSet.Split(' ')[0]);
                }
            }
        }
        return resultList;
    }
}