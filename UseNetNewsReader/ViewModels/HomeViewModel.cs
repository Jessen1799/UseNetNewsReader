using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UseNetNewsReader.Services;

namespace UseNetNewsReader.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly IConnection? _connection;

    [ObservableProperty] 
    private List<string> testListe = new();

    public HomeViewModel(IConnection connection)
    {
        _connection = connection;
    }
    

    [RelayCommand]
    private void DisconnectFromServer()
    {
        _connection?.Disconnect();
        //Succes scenarie med label som fortæller brugeren at forbindelsen blev dc'et succesfuldt.
        
    }

    [RelayCommand]
    private async Task DisplayNewsGroups()
    {
        
        if (_connection != null)
        {
            TestListe = await _connection.GetNewsGroups();
        }
    }
}