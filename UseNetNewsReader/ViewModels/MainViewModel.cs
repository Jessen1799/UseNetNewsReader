using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace UseNetNewsReader.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    
    [ObservableProperty]
    private ViewModelBase _currentPage;
    
    private static MainViewModel _thisWindow;
    private readonly LoginViewModel _loginViewModel;
    private HomeViewModel _homeViewModel;
    

    public MainViewModel()
    {
        _loginViewModel = App.Current.Services.GetService<LoginViewModel>();
        _loginViewModel.LoginSuccess += ChangeViewToHome;
        
        _thisWindow = this;
        CurrentPage = _loginViewModel;
    }

    
    private void ChangeViewToHome()
    {
        _homeViewModel = App.Current.Services.GetService<HomeViewModel>();
        CurrentPage = _homeViewModel;
    }
}