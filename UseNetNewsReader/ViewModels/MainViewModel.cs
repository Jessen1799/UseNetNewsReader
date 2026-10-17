using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using UseNetNewsReader.Services;

namespace UseNetNewsReader.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private IConnection? _connection;
    

    [RelayCommand]
    private async Task ConnectToServer()
    {
        _connection = new NntpService();
        await _connection.ConnectAsync("127.0.0.1", 4119);
    }

    [RelayCommand]
    private void DisconnectFromServer()
    {
        if (_connection == null)
        {
            Console.WriteLine("No connection established");
            return;
        }
        _connection.Disconnect();
    }
}