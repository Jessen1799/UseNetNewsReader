using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using UseNetNewsReader.ViewModels;
using UseNetNewsReader.Views;

namespace UseNetNewsReader;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        return param switch
        {
            LoginViewModel => new LoginView(),
            HomeViewModel => new HomeView(),
            MainViewModel => new MainWindowView(),
            
            _ => new TextBlock { Text = "No view yet implemented" }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}