using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace UseNetNewsReader.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    //Instance variables
    private readonly IConnection? _connection;
    //Generic delegate
    public event Action? LoginSuccess;

    #region ObservableProperties

    [ObservableProperty]
    public partial string ServerIP { get; set; }
    
    [ObservableProperty]
    public partial string? ServerPort { get; set; }
    
    [ObservableProperty]
    public partial string Username { get; set; }
    
    [ObservableProperty]
    public partial string Password { get; set; }
    #endregion
    

    /// <summary>
    /// Dependency injection to ensure that it uses the proper connection throughout the lifespan.
    /// </summary>
    /// <param name="connection"></param>
    public LoginViewModel(IConnection? connection)
    {
        _connection = connection;
    }
    
    [RelayCommand]
    private async Task AuthenticateUser()
    {
        if (_connection != null)
        {
            var success = await _connection.AuthenticateAsync(Username, Password);
            
            if (success == true)
            {
                LoginSuccess?.Invoke();
            }
        }
    }
    
    [RelayCommand]
    private async Task ConnectToServer()
    {
        if (ServerPort == null)
        {
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
        }
    }
}