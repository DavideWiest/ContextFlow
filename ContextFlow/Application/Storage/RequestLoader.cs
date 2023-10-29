using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using ContextFlow.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContextFlow.Application.Storage;

public abstract class RequestLoader
{
    public RequestResult? LoadMatchIfExists(LLMRequest request)
    {
        bool matchExists = MatchExists(request);
        request.RequestConfig.Logger.Information("Trying to load the result of the current request - Match exists: {matchExists}", matchExists);
        return matchExists ? LoadMatch(request) : null;
    }
    public abstract bool MatchExists(LLMRequest request);
    public abstract RequestResult LoadMatch(LLMRequest request);
}

public class JsonRequestLoader : RequestLoader
{
    private string FileName;
    private RequestHasher RequestHasher = new RequestHasher();
    public bool ConsiderLLMConfig { get; set; }

    public JsonRequestLoader(string fileName, bool considerLLMConfig = true)
    {
        FileName = fileName;
        ConsiderLLMConfig = considerLLMConfig;
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
            if ((!ConsiderLLMConfig && data[key1].Count > 0) || data[key1].ContainsKey(key2))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// works on the premise that a match exists in the given file. Use the method MatchExists to verify. Alternatively, use LoadMatchIfExists
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public override RequestResult LoadMatch(LLMRequest request)
    {
        // Generate the key based on the request's Prompt, and generate the key based on LLMConfig
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        request.RequestConfig.Logger.Information("Loading the match in {fileName}, with prompt-key {promptKey}, with llmconfig-key {llmconfigKey}", FileName, key1, key2);

        // Load data from the JSON file
        var data = LoadDataFromFile()!;

        string LLMConfigKey = ConsiderLLMConfig ? key2 : data[key1].Keys.First();
        if (!ConsiderLLMConfig)
        {
            request.RequestConfig.Logger.Information("Picking the first available option with this prompt-key as ConsiderLLMConfig is set to false. This option has llmconfig-key {llmconfigkey}", data[key1].Keys.First());
        }
        // Data found for the given key
        return LoaderUtil.ConvertToRequestResult(data[key1][LLMConfigKey]["response"]);
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