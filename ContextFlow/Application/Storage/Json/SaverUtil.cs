using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using Newtonsoft.Json;

namespace ContextFlow.Application.Storage.Json;

internal static class SaverUtil
{
    public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> CreateFileStructureFromData(string key1, string key2, LLMRequestBase request, RequestResult result, bool storeKeysPlainlyToo)
    {
        var dict =  new Dictionary<string, Dictionary<string, Dictionary<string, object>>>
        {
            {key1, new Dictionary<string, Dictionary<string, object>>
                {
                    {key2, new Dictionary<string, object>()}
                }
            }
        };

        if (storeKeysPlainlyToo)
        {
            dict[key1][key2]["prompt"] = request.Prompt;
            dict[key1][key2]["llmconfig"] = request.LLMConfig;
        }
        dict[key1][key2]["response"] = result;
        dict[key1][key2]["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        return dict;
    }

    public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> MergeDataWithExisting(string FileName, Dictionary<string, Dictionary<string, Dictionary<string, object>>> data, string key1, string key2)
    {
        var existingData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(File.ReadAllText(FileName));

        existingData ??= new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

        if (!existingData.ContainsKey(key1))
        {
            existingData[key1] = new Dictionary<string, Dictionary<string, object>>();
        }

        existingData[key1][key2] = data[key1][key2]; // Merge or overwrite data with the same key

        data = existingData;
        return data;
    }
}
