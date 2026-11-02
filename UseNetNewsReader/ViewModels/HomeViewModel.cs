using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UseNetNewsReader.Models;

namespace UseNetNewsReader.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    //Instance variables
    private readonly IConnection? _connection;

    #region ObservableProperties
    
    [ObservableProperty] 
    private List<string> _newsGroups = new();

    [ObservableProperty] 
    private ObservableCollection<NewsArticle> _articles = new();
    
    [ObservableProperty]
    private string? _selectedNewsGroup;

    [ObservableProperty] 
    private NewsArticle? _selectedArticle;

    [ObservableProperty] 
    private string? _articleContent;
    #endregion
    
    

    /// <summary>
    /// Dependency injection to ensure that it uses the proper connection throughout the lifespan.
    /// </summary>
    /// <param name="connection"></param>
    public HomeViewModel(IConnection connection)
    {
        _connection = connection;
    }
    

    
    [RelayCommand]
    private void DisconnectFromServer()
    {
        if (_connection == null)
        {
            Console.WriteLine("No connection established");
            return;
        }
        _connection?.Disconnect();
        //Succes scenarie med label som fortæller brugeren at forbindelsen blev dc'et succesfuldt.
        
    }

    [RelayCommand]
    private async Task DisplayNewsGroups()
    {
        
        if (_connection != null)
        {
            NewsGroups = await _connection.GetNewsGroups();
        }
    }

    [RelayCommand]
    private async Task DisplayAllArticles()
    {
        if (_connection != null && SelectedNewsGroup != null)
        {
            var result = await _connection.GetAllArticles(SelectedNewsGroup);
            
            Articles = new ObservableCollection<NewsArticle>(result);
        }
    }

    [RelayCommand]
    private async Task DisplayArticleContent()
    {
        if (_connection != null && SelectedArticle != null)
        {
            ArticleContent = await _connection.GetArticleContent(SelectedArticle.ArticleId.ToString());
        }
    }
}