using System.Threading.Tasks;

namespace UseNetNewsReader;

public interface IConnection
{
    public Task ConnectAsync(string ip, int port);

    public void Disconnect();
}