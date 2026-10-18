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
        if (_connection == null)
        {
            await _connection.ConnectAsync("127.0.0.1", 4119);
            //Label til bruger som fortæller at server ikke er tilængelig/nægtede adgang.
        }
    }

    [RelayCommand]
    private void DisconnectFromServer()
    {
        if (_connection == null)
        {
            //Label skal fremvises som fortæller at der ikke er en forbindelse aktiv at disconnecte
            return;
        }
        _connection.Disconnect();
        //Succes scenarie med label som fortæller brugeren at forbindelsen blev dc'et succesfuldt.
    }
}