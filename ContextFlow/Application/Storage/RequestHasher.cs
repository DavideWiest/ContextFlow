using ContextFlow.Application.Request;
using System.Security.Cryptography;
using System.Text;

namespace ContextFlow.Application.Storage;

/// <summary>
/// Creates hashes for 1) the prompt, and 2) the LLM-configuration. Use this class inside custom implementations of RequestLoader/RequestSaver.
/// </summary>
public static class RequestHasher
{
    public static Tuple<string, string> GenerateKeys(LLMRequestBase request)
    {
        return Tuple.Create(GenerateHashKey(request.Prompt.ToPlainText()), GenerateHashKey(request.LLMConfig.ToString()));
    }

    private static string GenerateHashKey(string input)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }
}