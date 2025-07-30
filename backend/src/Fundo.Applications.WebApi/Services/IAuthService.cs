using System.Threading.Tasks;
using Fundo.Applications.WebApi.DTOs;

namespace Fundo.Applications.WebApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<bool> UserExistsAsync(string username, string email);
    }
} 