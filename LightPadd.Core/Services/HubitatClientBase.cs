﻿using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LightPadd.Core.Models.Hubitat;

namespace LightPadd.Core.Services;

// Each "API" app in Hubitat has a different set of devices,
// which each get a different ID per-app.
// Each API also has a different access token.
// We express that here with one client
// implementation per access token.

// See LivingRoomClient for an example.

public abstract class HubitatClientBase
{
    protected string AccessToken { get; init; }
    protected HttpClient Client { get; init; }
    private JsonSerializerOptions _serOpts;

    protected HubitatClientBase(HttpClient client, string accessToken)
    {
        AccessToken = accessToken;
        Client = client;
        _serOpts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public virtual async Task<Device?> GetDevice(string id)
    {
        Device[]? response = await GetDevicesInternal(id);
        if (response == null)
        {
            return null;
        }

        return response.Single();
    }

    public virtual Task<Device[]?> GetDevices() => GetDevicesInternal("*");

    private async Task<Device[]?> GetDevicesInternal(string filter)
    {
        HttpResponseMessage response = await Client.GetAsync(
            $"{filter}?access_token={AccessToken}"
        );
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        if (filter == "*")
        {
            return JsonSerializer.Deserialize<Device[]>(json, _serOpts);
        }

        var singleDevice = JsonSerializer.Deserialize<Device>(json, _serOpts)!;
        return new[] { singleDevice };
    }

    // TODO: Implement this.
    public virtual async Task GetEventHistory(string deviceId)
    {
        HttpResponseMessage response = await Client.GetAsync(
            $"{deviceId}/events?access_token={AccessToken}"
        );
    }

    // TODO: Implement this.
    public virtual async Task GetCommands(string deviceId)
    {
        HttpResponseMessage response = await Client.GetAsync(
            $"{deviceId}/commands?access_token={AccessToken}"
        );
    }

    // TODO: Implement this.
    public virtual async Task GetCapabilities(string deviceId)
    {
        HttpResponseMessage response = await Client.GetAsync(
            $"{deviceId}/capabilities?access_token={AccessToken}"
        );
    }

    public virtual async Task<Device?> SendCommand(
        string deviceId,
        string command,
        params string[] secondary
    )
    {
        string url;
        if (secondary != null && secondary.Length != 0)
        {
            url = $"{deviceId}/{command}/{string.Join(',', secondary)}?access_token={AccessToken}";
        }
        else
        {
            url = $"{deviceId}/{command}?access_token={AccessToken}";
        }
        HttpResponseMessage response = await Client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var latestState = await GetDevice(deviceId);
        return latestState;
    }
}