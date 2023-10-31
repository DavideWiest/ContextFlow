using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;
using ContextFlow.Application.Storage.Json;
using Newtonsoft.Json;

namespace ContextFlow.Application.Storage.Async.Json;



public class JsonRequestSaverAsync : RequestSaverAsync
{
    private readonly string FileName;
    private readonly bool StoreKeysPlainlyToo;

    public JsonRequestSaverAsync(string fileName, bool storeKeysPlainlyToo)
    {
        FileName = fileName;
        StoreKeysPlainlyToo = storeKeysPlainlyToo;
    }

    public override async Task SaveRequestAsync(LLMRequestAsync request, RequestResult result)
    {
        // Generate a unique key for the saved data
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        // Create a dictionary with the request and response
        var data = SaverUtil.CreateFileStructureFromData(key1, key2, request, result, StoreKeysPlainlyToo);

        request.RequestConfig.Logger.Information("Storing request with prompt-key {promptKey} and llmconfig-key {llmconfigKey}", key1, key2);

        // Load existing data if the file already exists
        if (File.Exists(FileName))
        {
            data = SaverUtil.MergeDataWithExisting(FileName, data, key1, key2);
        }
        else
        {
            request.RequestConfig.Logger.Debug("File {fileName} does not seem to exist. Creating it now.", FileName);
        }

        // Serialize the data to JSON
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Create or open the JSON file and write the data asynchronously
        await File.WriteAllTextAsync(FileName, jsonData);
    }
}