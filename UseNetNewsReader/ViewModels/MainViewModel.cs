using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UseNetNewsReader.Services;

namespace UseNetNewsReader.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private IConnection? _connection;

    [ObservableProperty] 
    private List<string> testListe = new();
    

    [RelayCommand]
    private async Task ConnectToServer()
    {
        _connection = new NntpService();
        try
        {
            await _connection.ConnectAsync("127.0.0.1", 4119);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
            //Mræeklgit lag. Virker som om at det ikke køre asynkront endnu
        }
        //objektet vil altid være null, fortsat null, indtil metode kaldt, fordi metode opretter tcpclient. Hvordan laver jeg secure tjek
        
        //Label-> Textblock i stedet til bruger som fortæller at server ikke er tilængelig/nægtede adgang.
        
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

    [RelayCommand]
    private async Task DisplayNewsGroups()
    {
        
        if (_connection != null)
        {
            TestListe = await _connection.GetNewsGroups();
        }
        
    }
}