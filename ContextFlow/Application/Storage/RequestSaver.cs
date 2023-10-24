using ContextFlow.Application.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ContextFlow.Application.Storing;

namespace ContextFlow.Application.Storing;

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
        string key = RequestHasher.GenerateKey(request);

        // Create a dictionary with the request and response
        var data = new Dictionary<string, Dictionary<string, object>>
        {
            {
                key, new Dictionary<string, object>
                {
                    { "request", request },
                    { "response", result }
                }
            }
        };

        // Load existing data if the file already exists
        if (File.Exists(FileName))
        {
            var existingData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(FileName));
            existingData[key] = data[key]; // Merge or overwrite data with the same key
            data = existingData;
        }

        // Serialize the data to JSON
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Create or open the JSON file and write the data
        File.WriteAllText(FileName, jsonData);
    }
}