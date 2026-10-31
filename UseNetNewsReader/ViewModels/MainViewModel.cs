using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace UseNetNewsReader.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty]
    private ViewModelBase _currentPage;
    
    private static MainViewModel _thisWindow;
    private LoginViewModel _loginViewModel;
    private HomeViewModel _homeViewModel;
    

    public MainViewModel()
    {
        _loginViewModel = App.Current.Services.GetService<LoginViewModel>();
        _loginViewModel.LoginSuccess += ChangeViewToHome;
        
        
        
        _thisWindow = this;
        CurrentPage = _loginViewModel;
    }

    [RelayCommand]
    private void ChangeSceneryToLogin()
    {
        CurrentPage = App.Current.Services.GetService<LoginViewModel>();
    }

    [RelayCommand]
    private void ChangeSceneryToHome()
    {
        CurrentPage = App.Current.Services.GetService<HomeViewModel>();
    }

    private void ChangeViewToHome()
    {
        _homeViewModel = App.Current.Services.GetService<HomeViewModel>();
        CurrentPage = _homeViewModel;
    }
}