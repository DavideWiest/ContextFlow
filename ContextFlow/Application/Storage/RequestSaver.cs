using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using Newtonsoft.Json;
using OpenAI_API.Moderation;

namespace ContextFlow.Application.Storage;

public abstract class RequestSaver
{
    public abstract void SaveRequest(LLMRequest request, RequestResult result);
}

public class JsonRequestSaver : RequestSaver
{
    private readonly string FileName;
    private readonly RequestHasher RequestHasher;

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
        var data = SaverUtil.CreateFileStructureFromData(key1, key2, request, result);

        // Load existing data if the file already exists
        if (File.Exists(FileName))
        {
            data = SaverUtil.MergeDataWithExisting(FileName, data, key1, key2);
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