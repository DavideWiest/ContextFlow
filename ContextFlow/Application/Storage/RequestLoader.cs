using ContextFlow.Application.Request;
using Newtonsoft.Json;

namespace ContextFlow.Application.Storage;

public abstract class RequestLoader
{
    public RequestResult? LoadMatchIfExists(LLMRequest request)
    {
        return MatchExists(request) ? LoadMatch(request) : null;
    }
    public abstract bool MatchExists(LLMRequest request);
    public abstract RequestResult LoadMatch(LLMRequest request);
}

public class JsonRequestLoader : RequestLoader
{
    private string FileName;
    private RequestHasher RequestHasher;
    public bool ConsiderLLMConfig { get; set; }

    public JsonRequestLoader(string fileName)
    {
        FileName = fileName;
        RequestHasher = new RequestHasher();
        ConsiderLLMConfig = false;
    }

    public override bool MatchExists(LLMRequest request)
    {
        // Generate the key based on the request's Prompt, and generate the key based on LLMConfig
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        // Load data from the JSON file
        var data = LoadDataFromFile();

        if (data == null)
            return false;

        if (data.ContainsKey(key1))
        {
            if ((!ConsiderLLMConfig && data.Count > 0) || data.ContainsKey(key2))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// works on the premise that a match exists in the given file. Use the method MatchExists to verify.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public override RequestResult LoadMatch(LLMRequest request)
    {
        // Generate the key based on the request's Prompt, and generate the key based on LLMConfig
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        // Load data from the JSON file
        var data = LoadDataFromFile()!;

        string LLMConfigKey = ConsiderLLMConfig ? key2 : data[key1].Keys.First();
        // Data found for the given key
        return (RequestResult)data[key1][LLMConfigKey]["response"];
    }

    private Dictionary<string, Dictionary<string, Dictionary<string, object>>>? LoadDataFromFile()
    {
        if (File.Exists(FileName))
        {
            string jsonData = File.ReadAllText(FileName);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(jsonData);
        }
        else
        {
            return null;
        }
    }
}