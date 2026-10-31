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
    
    public string? ValidationMessageFromServer { get; private set; } 
    public bool IsConnected { get; private set; }
    public bool IsAuthenticated { get; private set; }
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
        
        ValidationMessageFromServer = await _reader.ReadLineAsync();
        IsConnected = true;
        Console.WriteLine(ValidationMessageFromServer);
    }

    public async Task AuthenticateAsync(string username, string password)
    {
        Console.WriteLine("AuthenticateAsync kaldt");
        if (IsAuthenticated == true)
        {
            Console.WriteLine("Already authenticated");
            return;
        }
        
        
        var authenticateUserCommando =  Encoding.UTF8.GetBytes($"authinfo user {username}\r\n");
        var authenticatePassCommando = Encoding.UTF8.GetBytes($"authinfo pass {password}\r\n");
        
        if (_data != null)
        {
            await _data.WriteAsync(authenticateUserCommando);
        }
        var serverResponseUser = await _reader.ReadLineAsync();
        Console.WriteLine($"Server response user: {serverResponseUser}");

        if (serverResponseUser != null && serverResponseUser.StartsWith("381"))
        {
            await _data.WriteAsync(authenticatePassCommando);
        }
        var serverResponsePass = await _reader.ReadLineAsync();
        Console.WriteLine($"Server response pass: {serverResponsePass}");

        if (serverResponsePass != null && serverResponsePass.StartsWith("281"))
        {
            IsAuthenticated = true;
        }

        Console.WriteLine(serverResponsePass);
    }
}