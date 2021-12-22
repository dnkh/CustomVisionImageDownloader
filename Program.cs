using System.Linq;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using CustomVisionImageDownloader;

Console.WriteLine("Start downloading images");

var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", false, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

var trainingKey = configuration.GetValue<string>("CustomVisionSettings:TrainingKey");
var projectId = configuration.GetValue<string>("CustomVisionSettings:ProjectId");
var client = new HttpClient();

client.BaseAddress = new Uri("https://westeurope.api.cognitive.microsoft.com/customvision/v3.0/");
client.DefaultRequestHeaders.Add("Training-Key",trainingKey);

var trainingImageList = await client.GetFromJsonAsync<IEnumerable<TraingImageData>>($"training/projects/{projectId}/images/tagged");

if( trainingImageList is not null )
{
    await Parallel.ForEachAsync(trainingImageList, new ParallelOptions{MaxDegreeOfParallelism = 5}, async (traingImage, token) =>
    {
        var image = await client.GetStreamAsync(traingImage.originalImageUri,token);
        
        var imageName = traingImage.originalImageUri.Split('/').Last();
        imageName = imageName.Split('?').First();
        var imagePath = $"c:\\temp\\images\\{imageName}.jpg";

        Console.WriteLine($"Downloading {traingImage.originalImageUri}");

        using var fileStream = new FileStream(imagePath, FileMode.Create);
        await image.CopyToAsync(fileStream, token);
    });
}
