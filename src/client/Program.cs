// See https://aka.ms/new-console-template for more information
using IdentityModel.Client;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

Console.WriteLine("Hello, World!");

// Client calling discoveryDocumentResponse
var client = new HttpClient();
var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if(discoveryDocumentResponse.IsError)
    throw new Exception(discoveryDocumentResponse.Error);



// Client getting accessToken

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest

{
    Address = discoveryDocumentResponse.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1"
});
if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

Console.WriteLine(tokenResponse.AccessToken);


// Client calling the projected API
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);
var response = await apiClient.GetAsync("http://localhost:5157/WeatherForecast");
if(!response.IsSuccessStatusCode) return;

var doc=JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented=true}));

 