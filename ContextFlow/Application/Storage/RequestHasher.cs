using ContextFlow.Application.Request;
using System.Security.Cryptography;
using System.Text;

namespace ContextFlow.Application.Storage;

public class RequestHasher
{
    public Tuple<string, string> GenerateKeys(LLMRequestBase request)
    {
        return Tuple.Create(GenerateHashKey(request.Prompt.ToPlainText()), GenerateHashKey(request.LLMConfig.ToString()));
    }

    private string GenerateHashKey(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }
}