using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LightPadd.Core.Models.Hubitat;

namespace LightPadd.Core.Services;

public class HubitatClientService
{
    private readonly Uri _baseUrl = new("http://192.168.0.44/apps/api/3/devices/");
    private readonly string _accessToken = "24220d64-8ef2-4b44-a815-a41e64c23c49";
    private readonly HttpClient _client;

    public HubitatClientService()
    {
        _client = new HttpClient() { BaseAddress = _baseUrl };
    }

    public async Task GetDevicesBasic()
    {
        HttpResponseMessage response = await _client.GetAsync($"?access_token={_accessToken}");
    }

    /// <summary>
    /// Filter can be a device ID, 'all' or '*'.
    /// </summary>
    public async Task<JsonNode?> GetDevicesDetailed(string filter)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{filter}?access_token={_accessToken}"
        );
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonNode>(json);
    }

    public async Task GetEventHistory(string deviceId)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{deviceId}/events?access_token={_accessToken}"
        );
    }

    public async Task GetCommands(string deviceId)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{deviceId}/commands?access_token={_accessToken}"
        );
    }

    public async Task GetCapabilities(string deviceId)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"{deviceId}/capabilities?access_token={_accessToken}"
        );
    }

    public async Task<JsonNode?> SendCommand(
        string deviceId,
        string command,
        params string[] secondary
    )
    {
        string url;
        if (secondary != null && secondary.Length != 0)
        {
            url = $"{deviceId}/{command}/{string.Join(',', secondary)}?access_token={_accessToken}";
        }
        else
        {
            url = $"{deviceId}/{command}?access_token={_accessToken}";
        }
        HttpResponseMessage response = await _client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var latestState = await GetDevicesDetailed(deviceId);
        return latestState;
    }
}
