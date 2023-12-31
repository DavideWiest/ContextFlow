﻿using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;
using ContextFlow.Application.Storage.Json;
using Newtonsoft.Json;

namespace ContextFlow.Application.Storage.Async.Json;

public class JsonRequestLoaderAsync : RequestLoaderAsync
{
    private readonly string FileName;
    public bool ConsiderLLMConfig { get; set; }

    public JsonRequestLoaderAsync(string fileName, bool considerLLMConfig = true)
    {
        FileName = fileName;
        ConsiderLLMConfig = considerLLMConfig;
    }

    public override async Task<bool> MatchExistsAsync(LLMRequestAsync request)
    {
        // Generate the key based on the request's Prompt, and generate the key based on LLMConfig
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        // Load data from the JSON file
        var data = await LoadDataFromFileAsync();

        if (data == null)
            return false;
        if (data.ContainsKey(key1))
        {
            if (!ConsiderLLMConfig && data[key1].Count > 0 || data[key1].ContainsKey(key2))
            {
                return true;
            }
        }
        return false;
    }

    public override async Task<RequestResult> LoadMatchAsync(LLMRequestAsync request)
    {
        // Generate the key based on the request's Prompt, and generate the key based on LLMConfig
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        // Load data from the JSON file
        var data = await LoadDataFromFileAsync();

        request.RequestConfig.Logger.Information("Loading the match in {fileName}, with prompt-key {promptKey}, with llmconfig-key {llmconfigKey}", FileName, key1, key2);

        if (data == null)
        {
            throw new InvalidOperationException($"Could not load from file {FileName}");
        }

        string LLMConfigKey = ConsiderLLMConfig ? key2 : data[key1].Keys.First();

        if (!ConsiderLLMConfig)
        {
            request.RequestConfig.Logger.Information("Picking the first available option with this prompt-key as ConsiderLLMConfig is set to false. This option has llmconfig-key {llmconfigkey}", data[key1].Keys.First());
        }

        // Data found for the given key
        return LoaderUtil.ConvertToRequestResult(data[key1][LLMConfigKey]["response"]);
    }

    private async Task<Dictionary<string, Dictionary<string, Dictionary<string, object>>>?> LoadDataFromFileAsync()
    {
        if (File.Exists(FileName))
        {
            string jsonData = await File.ReadAllTextAsync(FileName);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(jsonData);
        }
        else
        {
            return null;
        }
    }
}