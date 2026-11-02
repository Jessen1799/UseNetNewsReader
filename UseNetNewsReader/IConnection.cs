using System.Collections.Generic;
using System.Threading.Tasks;
using UseNetNewsReader.Models;

namespace UseNetNewsReader;

public interface IConnection
{
    public Task ConnectAsync(string ip, int port);

    public void Disconnect();

    public Task<List<string>> GetNewsGroups();
    
    public Task<bool> AuthenticateAsync(string username, string password);
    
    public Task<List<NewsArticle>> GetAllArticles(string groupname);
    
    public Task<string> GetArticleContent(string articleId); 
}