using SHB1VirtualHive.Models;
using System.Net.Http.Json;

HttpClient client = new HttpClient();

int id = 2;
string serial = "307813";

string postAddress = "https://localhost:7156/restapi/SHB1PostDataJson";

async Task PostRandomData()
{
    Console.WriteLine("posting random data");

    Random random = new Random();
    PostModel model = new PostModel()
    {
        Id = id,
        Serial = serial,
        unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
        humidity = random.Next(1, 99),
        insideTemperature = random.Next(-10, 50),
        pressure = random.Next(4800, 5200),
        outsideTemperature = random.Next(-10, 40),
        weight = random.Next(1, 12)
    };

    HttpResponseMessage response = await client.PostAsJsonAsync(postAddress, model);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("post success");
        return;
    }
    Console.WriteLine("post Failed");
    Console.WriteLine(response);
    return;
}

while (true)
{
    Task postResult = PostRandomData();
    postResult.Wait();

    System.Threading.Thread.Sleep(1200000);
}