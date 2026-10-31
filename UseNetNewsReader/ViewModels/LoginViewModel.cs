using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using UseNetNewsReader.Services;

namespace UseNetNewsReader.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private IConnection? _connection;
    public event Action? LoginSuccess;
    
    
    [RelayCommand]
    private async Task AuthenticateUser()
    {
        if (_connection != null)
        {
            await _connection.AuthenticateAsync("Unknown1717", "kontor1337");
            LoginSuccess?.Invoke();
        }
    }
    
    [RelayCommand]
    private async Task ConnectToServer()
    {
        _connection = new NntpService();
        try
        {
            await _connection.ConnectAsync("news.eternal-september.org", 119);
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