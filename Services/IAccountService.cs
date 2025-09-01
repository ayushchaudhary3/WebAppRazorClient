using System.Threading;
using System.Threading.Tasks;
using WebAppRazorClient.Models;

namespace WebAppRazorClient.Services
{
    public interface IAccountService
    {
        Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}