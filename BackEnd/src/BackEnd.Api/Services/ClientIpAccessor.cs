namespace BackEnd.Api.Services;

public interface IClientIpAccessor
{
    string GetClientIp();
}

public class ClientIpAccessor : IClientIpAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;
    
    public ClientIpAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetClientIp()
    {
        return httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }
}