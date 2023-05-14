using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SHB1RandomCamera.models;
using System.Net.Http.Json;

HttpClient client = new HttpClient();

int id = 1;
string serial = "11111";

string postAddress = "https://localhost:7156/restapi/SHB1PostImageJson";

string GenerateImage()
{
    // Vytvoření bitmapy
    Bitmap bitmap = new Bitmap(100, 100);

    // Náhodná barva
    Random random = new Random();
    Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

    // Vyplnění bitmapy náhodnou barvou
    Graphics graphics = Graphics.FromImage(bitmap);
    graphics.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, 100, 100));

    // Uložení bitmapy do streamu
    MemoryStream stream = new MemoryStream();
    bitmap.Save(stream, ImageFormat.Jpeg);

    // Převod streamu na řetězec
    string imageString = Convert.ToBase64String(stream.ToArray());

    // Výpis řetězce
    return imageString;
}


async Task PostRandomData()
{
    Console.WriteLine("posting random data");

    Random random = new Random();
    postmodel model = new postmodel()
    {
        Id = id,
        Serial = serial,
        Image = GenerateImage()
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

    System.Threading.Thread.Sleep(5000);
}