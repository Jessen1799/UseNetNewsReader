using System.Collections.Generic;
using System.Threading.Tasks;

namespace UseNetNewsReader;

public interface IConnection
{
    public Task ConnectAsync(string ip, int port);

    public void Disconnect();

    public Task<List<string>> GetNewsGroups();
    
    public Task AuthenticateAsync(string username, string password);
    
}