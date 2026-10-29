using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace UseNetNewsReader.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty]
    private ViewModelBase _currentPage;
    
    private static MainViewModel _thisWindow;

    public MainViewModel()
    {
        
        _thisWindow = this;
        
        CurrentPage = App.Current.Services.GetService<LoginViewModel>();
    }

    [RelayCommand]
    public void ChangeSceneryToLogin()
    {
        CurrentPage = App.Current.Services.GetService<LoginViewModel>();
    }

    [RelayCommand]
    public void ChangeSceneryToHome()
    {
        CurrentPage = App.Current.Services.GetService<HomeViewModel>();
    }
}