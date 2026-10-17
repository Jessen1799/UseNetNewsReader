using CommunityToolkit.Mvvm.Input;
using UseNetNewsReader.Services;

namespace UseNetNewsReader.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private IConnection _connection;

    [RelayCommand]
    private void ConnectToServer()
    {
        _connection = new NntpService();
        _connection.ConnectAsync("127.0.0.1", 4119);
    }

    [RelayCommand]
    private void DisconnectFromServer()
    {
        _connection.Disconnect();
    }
}