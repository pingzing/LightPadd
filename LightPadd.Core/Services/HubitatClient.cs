using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LightPadd.Core.Models.Hubitat;
using PolyJson;

namespace LightPadd.Core.Services;

// Each "API" app in Hubitat has a different set of devices,
// which each get a different ID per-app.
// Each API also has a different access token.
// We're going to treat each "API" as a room.

public class HubitatClient
{
    private readonly HubitatGenerationContext _jsonContext;

    private string _accessToken { get; init; }
    private HttpClient _client { get; init; }

    public HubitatClient(HttpClient client, string accessToken)
    {
        _accessToken = accessToken;
        _client = client;

        JsonSerializerOptions opts = new(JsonSerializerDefaults.Web);

        var deviceConverterAttribute = new PolyJsonConverterAttribute("dataType");
        JsonConverter deviceAttributeConverter = deviceConverterAttribute.CreateConverter(
            typeof(BaseDeviceAttribute)
        );
        opts.Converters.Add(deviceAttributeConverter);

        _jsonContext = new HubitatGenerationContext(opts);
    }

    public async Task<Device?> GetDevice(string id)
    {
        Device[]? response = await GetDevicesInternal(id);
        if (response == null)
        {
            return null;
        }

        return response.Single();
    }

    public Task<Device[]?> GetDevices() => GetDevicesInternal("*");

    private async Task<Device[]?> GetDevicesInternal(string filter)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"devices/{filter}?access_token={_accessToken}"
        );
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        if (filter == "*")
        {
            return JsonSerializer.Deserialize(json, _jsonContext.DeviceArray);
        }

        var singleDevice = JsonSerializer.Deserialize(json, _jsonContext.Device)!;
        return [singleDevice];
    }

    // TODO: Implement this.
    public async Task GetEventHistory(string deviceId)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"devices/{deviceId}/events?access_token={_accessToken}"
        );
    }

    // TODO: Implement this.
    public async Task GetCommands(string deviceId)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"devices/{deviceId}/commands?access_token={_accessToken}"
        );
    }

    // TODO: Implement this.
    public async Task GetCapabilities(string deviceId)
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"devices/{deviceId}/capabilities?access_token={_accessToken}"
        );
    }

    public async Task<Device?> SendCommand(
        string deviceId,
        string command,
        params string[] secondary
    )
    {
        string url;
        if (secondary != null && secondary.Length != 0)
        {
            url =
                $"devices/{deviceId}/{command}/{string.Join(',', secondary)}?access_token={_accessToken}";
        }
        else
        {
            url = $"devices/{deviceId}/{command}?access_token={_accessToken}";
        }
        HttpResponseMessage response = await _client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var latestState = await GetDevice(deviceId);
        return latestState;
    }

    public async Task<bool> SendPostbackUrl(string postbackUrl)
    {
        string encodedUrl = WebUtility.UrlEncode(postbackUrl);
        HttpResponseMessage response = await _client.GetAsync(
            $"postURL/{encodedUrl}?access_token={_accessToken}"
        );
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                $"Failed to set Postback URL to {postbackUrl}. Attempted to call {response?.RequestMessage?.RequestUri}."
                    + $"Received HTTP {response?.StatusCode} with content: {response?.Content.ToString()}"
            );
            return false;
        }

        return true;
    }
}
