using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Storage;

internal static class SaverUtil
{
    public static Dictionary<string, Dictionary<string, Dictionary<string, object>>> CreateFileStructureFromData(string key1, string key2, LLMRequestBase request, RequestResult result)
    {
        return new Dictionary<string, Dictionary<string, Dictionary<string, object>>>
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
