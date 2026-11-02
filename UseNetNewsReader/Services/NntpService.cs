using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UseNetNewsReader.Models;

namespace UseNetNewsReader.Services;


/// <summary>
/// Service class that handles the implementations regarding requests and responses from and to server.
/// Implements 'IConnection' interface.
/// </summary>
public class NntpService : IConnection
{
    private TcpClient? _client;
    private NetworkStream? _data;
    private StreamReader? _reader;
    
    public string? ValidationMessageFromServer { get; private set; } 
    public bool IsConnected { get; private set; }
    private bool IsAuthenticated { get; set; }
    
    /// <summary>
    /// Connects asynchronously to server. Uses 'NetworkStream' and 'StreamReader'
    /// to read the stream of data received from server and reads chars from byte stream. Receives confirmation msg upon connection success from server.
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
        if (ValidationMessageFromServer == null || !ValidationMessageFromServer.StartsWith("200"))
        {
            Console.WriteLine("Connection failed");
            return;
        }
        IsConnected = true;
        Console.WriteLine(ValidationMessageFromServer);
    }
    
    
    /// <summary>
    /// Closes NNTP connection and closes resources properly
    /// Resets the connection
    /// </summary>
    public void Disconnect()
    {
        _reader?.Close();
        _client?.Close();

        //Fjerner referencer til objekterne, fordi de ikke skal 'eksistere' mere.
        _reader = null;
        _client = null;
        
        IsConnected = false;
        Console.WriteLine($"Connection status: {IsConnected}");
    }
    
    /// <summary>
    /// Send command to server to get a specific article based on articel ID.
    /// Reads line after line and builds the full article content using 'StringBuilder'.
    /// </summary>
    /// <param name="articleId"></param>
    /// <returns></returns>
    public async Task<string> GetArticleContent(string articleId)
    {
        if (_data == null || _reader == null)
        {
            return  string.Empty;
        }

        var commando = Encoding.UTF8.GetBytes($"article {articleId}\r\n");

        await _data.WriteAsync(commando);

        var response = await _reader.ReadLineAsync();

        if (response == null || !response.StartsWith("220"))
        {
            return $"Server error: {response}";
        }

        StringBuilder articleContent = new();

        string? line;
        while ((line = await _reader.ReadLineAsync()) != null && line != ".")
        {
            if (line.StartsWith(".."))
            {
                line = line[1..];
            }
            articleContent.AppendLine(line);
        }
        return articleContent.ToString();
    }

    /// <summary>
    /// Send command to server that retrieves all news groups available on server.
    /// Takes full message from server and splits by ' ' and takes only the name.
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetNewsGroups()
    {
        List<string> resultList = new List<string>();
        
        if (_data == null || _reader == null)
        {
            return resultList;
        }
        
        var listCommando = Encoding.UTF8.GetBytes("LIST\r\n");
        
        await _data.WriteAsync(listCommando);
        var resultSet = await _reader.ReadLineAsync();
        Console.WriteLine($"Modtaget fra server: {resultSet}");

        if (resultSet != null && resultSet.StartsWith("215"))
        {
            while (!resultSet.StartsWith("."))
            {
                resultSet = await _reader.ReadLineAsync();
                Console.WriteLine($"Modtaget fra server: {resultSet}");
                if (resultSet != null)
                {
                    resultList.Add(resultSet.Split(' ')[0]);
                }
            }
        }
        return resultList;
    }
    
    /// <summary>
    /// Sends two commands to server. First it sends and handles group command which by NNTP protocol is seperated with space.
    /// After it sends xover command to get all articles inside the news group which is by NNTP protocol seperated with \t TAB.
    /// Lastly it uses 'NewsArticle' model to create objects of a specific type.
    /// </summary>
    /// <param name="groupname"></param>
    /// <returns></returns>
    public async Task<List<NewsArticle>> GetAllArticles(string groupname)
    {
        List<NewsArticle> articles = new List<NewsArticle>();
        var groupCommando = Encoding.UTF8.GetBytes($"group {groupname}\r\n");

        if (_data == null || _reader == null)
        {
            return articles;
        }
        await _data.WriteAsync(groupCommando);
        
        var groupResponse = await _reader.ReadLineAsync();
        Console.WriteLine($"Modtaget fra server: {groupResponse}");

        if (groupResponse == null)
        {
            return articles;
        }
        
        var groupPartsFromServer = groupResponse.Split(' ');
        //Gjort for at ramme det rigtige i responsen fra serveren '211(0) 25(1) 24463(2) første artikelid 30242(3) sidste artikel nummer.
        var firstArticle = int.Parse(groupPartsFromServer[2]);
        var lastArticle = int.Parse(groupPartsFromServer[3]);

        var xoverCommando = Encoding.UTF8.GetBytes($"xover {firstArticle}-{lastArticle}\r\n");
        await _data.WriteAsync(xoverCommando);
        var xoverResponse = await _reader.ReadLineAsync();

        if (xoverResponse == null || !xoverResponse.StartsWith("224"))
        {
            return articles;
        }
        
        string? serverMsgLine;
        while ((serverMsgLine = await _reader.ReadLineAsync()) != null && serverMsgLine != ".")
        {
            var parts = serverMsgLine.Split('\t');
            
            var article = new NewsArticle
            {
                ArticleId = int.Parse(parts[0]),
                Subject = parts[1],
                Date = parts[3]
            };
            articles.Add(article);
        }
        return articles;
    }

    /// <summary>
    /// Handles authentication process to server.
    /// Uses AUTHINFO user and AUTHINFO pass to send to server to validate credentials.
    /// Upon success response from server returns true and sets state.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        if (IsAuthenticated == true)
        {
            Console.WriteLine("Already authenticated");
            return true;
        }
        
        
        var authenticateUserCommando =  Encoding.UTF8.GetBytes($"authinfo user {username}\r\n");
        var authenticatePassCommando = Encoding.UTF8.GetBytes($"authinfo pass {password}\r\n");
        
        if (_data == null)
        {
            return false;
        }
        
        await _data.WriteAsync(authenticateUserCommando);
        
        var serverResponseUser = await _reader.ReadLineAsync();
        Console.WriteLine($"Server response user: {serverResponseUser}");

        if (serverResponseUser == null || !serverResponseUser.StartsWith("381"))
        {
            return false;
        }
        
        await _data.WriteAsync(authenticatePassCommando);
        var serverResponsePass = await _reader.ReadLineAsync();
        Console.WriteLine($"Server response pass: {serverResponsePass}");

        if (serverResponsePass == null || !serverResponsePass.StartsWith("281"))
        {
            return false;
        }
        IsAuthenticated = true;
        Console.WriteLine(serverResponsePass);
        return true;
    }
}