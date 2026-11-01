using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UseNetNewsReader.Services;

namespace UseNetNewsReader.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private IConnection? _connection;
    public event Action? LoginSuccess;
    
    [ObservableProperty]
    public partial string ServerIP { get; set; }
    
    [ObservableProperty]
    public partial string? ServerPort { get; set; }
    
    [ObservableProperty]
    public partial string Username { get; set; }
    
    [ObservableProperty]
    public partial string Password { get; set; }
    
    
    [RelayCommand]
    private async Task AuthenticateUser()
    {
        if (_connection != null)
        {
            await _connection.AuthenticateAsync(Username, Password);
            LoginSuccess?.Invoke();
        }
    }
    
    [RelayCommand]
    private async Task ConnectToServer()
    {
        _connection = new NntpService();
        
        if (ServerPort == null)
        {
            //Ping bruger om at felt skal udfyldes?
            return;
        }
        
        var serverPort = int.Parse(ServerPort);
        
        
        try
        {
            await _connection.ConnectAsync(ServerIP, serverPort);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
            //Mræeklgit lag. Virker som om at det ikke køre asynkront endnu
        }
        //objektet vil altid være null, fortsat null, indtil metode kaldt, fordi metode opretter tcpclient. Hvordan laver jeg secure tjek
        
        //Label-> Textblock i stedet til bruger som fortæller at server ikke er tilængelig/nægtede adgang.
    }
}