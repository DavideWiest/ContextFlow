using ContextFlow.Application.Request;
using Newtonsoft.Json;

namespace ContextFlow.Application.Storage;

public abstract class RequestSaver
{
    public abstract void SaveRequest(LLMRequest request, RequestResult result);
}

public class JsonRequestSaver : RequestSaver
{
    private readonly string FileName;
    private RequestHasher RequestHasher;

    public JsonRequestSaver(string fileName)
    {
        FileName = fileName;
        RequestHasher = new RequestHasher();
    }

    public override void SaveRequest(LLMRequest request, RequestResult result)
    {
        // Generate a unique key for the saved data
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        request.RequestConfig.Logger.Information("Storing request with prompt-key {promptKey} and llmconfig-key {llmconfigKey}", key1, key2);

        // Create a dictionary with the request and response
        var data = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>
        {
            {key1, new Dictionary<string, Dictionary<string, object>>
                {
                    {key2, new Dictionary<string, object> 
                        {
                            { "prompt", request.Prompt },
                            { "llmconfig", request.LLMConfig },
                            { "response", result },
                            { "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                        } 
                    }
                }
            }
        };

        // Load existing data if the file already exists
        if (File.Exists(FileName))
        {
            var existingData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(File.ReadAllText(FileName));
            if (existingData == null)
            {
                existingData = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            }
            if (!existingData.ContainsKey(key1))
            {
                existingData[key1] = new Dictionary<string, Dictionary<string, object>>();
            } 

            existingData[key1][key2] = data[key1][key2]; // Merge or overwrite data with the same key

            data = existingData;
        } else
        {
            request.RequestConfig.Logger.Debug("File {fileName} does not seem to exist. Creating it now.", FileName);
        }

        // Serialize the data to JSON
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Create or open the JSON file and write the data
        File.WriteAllText(FileName, jsonData);
    }
}