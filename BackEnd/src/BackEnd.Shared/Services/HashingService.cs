using System.Security.Cryptography;
using System.Text;

namespace BackEnd.Shared.Services;

public interface IHashingService
{
    string HashString(string text, string salt = "");
}

public class HashingService : IHashingService
{
    public string HashString(string text, string salt = "")
    {
        using var sha = SHA256.Create();
        
        var textBytes = Encoding.UTF8.GetBytes(text + salt);
        var hashBytes = sha.ComputeHash(textBytes);
    
        var hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", string.Empty);

        return hash;
        
    }
}