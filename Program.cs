using ImageMagick;
using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Nodes;

HttpClient client = new();

//string json = await File.ReadAllTextAsync(@"core.json");
string json = await client.GetStringAsync("https://dotnet.microsoft.com/blob-assets/json/thanks/core.json");

JsonNode data = JsonNode.Parse(json);

HashSet<string> usernames = new();

if (data != null)
{
    foreach (JsonNode node in data.AsArray())
    {
        if (node["Version"].GetValue<string>() == "8.0.0")
        {
            var contributors = node["Contributors"].AsArray();
            foreach (var contributor in contributors)
            {
                string link = contributor["Link"].GetValue<string>();
                int slashIndex = link.LastIndexOf('/');
                string username = link.Substring(slashIndex + 1);
                usernames.Add(username);
                //Console.WriteLine(username);
            }
        }
    }
}

AnsiConsole.WriteLine($"Total contributors:{usernames.Count}");

string directory = "images";
Directory.CreateDirectory(directory);

int actualAvatars = 0;

var downloadResults = AnsiConsole.Progress().Start(async ctx =>
{
    var downloadTask = ctx.AddTask("Downloading...", maxValue: usernames.Count);

    foreach (var username in usernames)
    {
        var response = await client.GetAsync($"https://www.github.com/{username}.png?size=200");
        if (response.IsSuccessStatusCode)
        {
            var image = await response.Content.ReadAsByteArrayAsync();
            //AnsiConsole.WriteLine(image.Length);
            if (image.Length > 1800)
            {
                //await File.WriteAllBytesAsync($"{directory}/{Random.Shared.Next(10000)}-{username}.png", image);
                await File.WriteAllBytesAsync($"{directory}/{username}.png", image);
                actualAvatars++;
            }
        }
        downloadTask.Increment(1);
    }
});

int h = (int)Math.Round(Math.Sqrt(actualAvatars / 1.7778));
int w = (int)Math.Round(h * 1.7778);

using var images = new MagickImageCollection();

// get the files from the directory in random order
var files = Directory.GetFiles(directory).OrderBy(x => Random.Shared.Next());
foreach (var file in files)
{
    images.Add(file);
}

var settings = new MontageSettings
{
    Geometry = new MagickGeometry(200, 200),
    TileGeometry = new MagickGeometry($"{w}x{h}"),
    BorderWidth = 0,
    BackgroundColor = MagickColors.Transparent
};

int surplus = images.Count - (w * h);
if (surplus > 0)
{
    AnsiConsole.WriteLine($"Removing {surplus} images from the end.");
    for (int i = 0; i < surplus; i++)
    {
        // remove a random image from the middle
        int index = Random.Shared.Next(w * h - surplus);
        images.RemoveAt(index);
    }
}

AnsiConsole.Status()
    .Start("Generating image...", ctx =>
    {
        using var result = images.Montage(settings);
        result.Write("contributors.png");
    });

AnsiConsole.WriteLine($"Total avatars downloaded: {actualAvatars}");
AnsiConsole.WriteLine($"Image written to contributors.png");
AnsiConsole.WriteLine("Use the following to tweak and generate a mosaic:");
AnsiConsole.WriteLine($"magick montage -geometry 200x200+0+0 -tile {w}x{h} {directory}/*.png contributors.png");
AnsiConsole.WriteLine("Press any key to quit.");
Console.ReadLine();

//magick montage -geometry 200x200+0+0 -tile 33x19 C:/Users/Jon/Documents/ContributorAvatars/bin/Debug/net7.0/images/*.png output.png
//https://sinestesia.co/blog/tutorials/quick-n-easy-mosaics-with-imagemagick/
//https://imagemagick.org/script/montage.php
