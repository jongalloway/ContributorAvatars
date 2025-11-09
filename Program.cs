using ImageMagick;
using Spectre.Console;
using System.Text.Json;
using System.Text.Json.Nodes;

HttpClient client = new();

// Load contributors from available source
HashSet<string> usernames = await LoadContributorsAsync();

AnsiConsole.WriteLine($"Total contributors: {usernames.Count}");

string directory = "../../../images";
Directory.CreateDirectory(directory);

int missingAvatars = 0;

await AnsiConsole.Progress().StartAsync(async ctx =>
{
    var downloadTask = ctx.AddTask("Downloading...", maxValue: usernames.Count);

    foreach (var username in usernames)
    {
        // check if the file already exists and skip
        if (File.Exists($"{directory}/{username}.png"))
        {
            downloadTask.Increment(1);
            continue;
        }

        var response = await client.GetAsync($"https://www.github.com/{username}.png?size=200");
        if (response.IsSuccessStatusCode)
        {
            var image = await response.Content.ReadAsByteArrayAsync();
            //Console.WriteLine(image.Length);
            if (image.Length > 1800)
            {
                //await File.WriteAllBytesAsync($"{directory}/{Random.Shared.Next(10000)}-{username}.png", image);
                await File.WriteAllBytesAsync($"{directory}/{username}.png", image);
            }
            else
            {
                missingAvatars++;
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            // write a message and delay for 2 minutes
            AnsiConsole.WriteLine("Rate limited. Waiting for 2 minutes.");
            await Task.Delay(120000);
        }
        downloadTask.Increment(1);
    }
});

AnsiConsole.WriteLine($"Missing avatars: {missingAvatars}");
AnsiConsole.WriteLine($"Total avatars downloaded: {usernames.Count - missingAvatars}");

using var images = new MagickImageCollection();

// get the files from the directory
var files = Directory.GetFiles(directory);
    //.OrderBy(x => Random.Shared.Next());
foreach (var file in files)
{
    images.Add(file);
}

int actualAvatars = images.Count;
double ratio = 16 / 9;
int h = (int)Math.Round(Math.Sqrt(actualAvatars / ratio));
int w = (int)Math.Round(h * ratio);

var settings = new MontageSettings
{
    Geometry = new MagickGeometry(100, 100),
    TileGeometry = new MagickGeometry($"{w}x{h}"),
    BorderWidth = 0,
    BackgroundColor = MagickColors.Transparent
};

int surplus = images.Count - (w * h);
if (surplus > 0)
{
    AnsiConsole.WriteLine($"Removing {surplus} random avatars to fit an even multiple.");
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

async Task<HashSet<string>> LoadContributorsAsync()
{
    // Check for text file first (preferred method)
    string txtPath = "../../../contributors.txt";
    if (File.Exists(txtPath))
    {
        AnsiConsole.WriteLine($"Reading contributors from {Path.GetFileName(txtPath)}");
        return await LoadFromTextFileAsync(txtPath);
    }

    // Fall back to JSON file
    string jsonPath = "../../../core.json";
    if (File.Exists(jsonPath))
    {
        AnsiConsole.WriteLine($"Reading contributors from {Path.GetFileName(jsonPath)}");
        return await LoadFromJsonFileAsync(jsonPath);
    }

    // Fall back to downloading JSON
    AnsiConsole.WriteLine("Downloading contributors from dotnet.microsoft.com");
    return await LoadFromJsonUrlAsync("https://dotnet.microsoft.com/blob-assets/json/thanks/core.json");
}

async Task<HashSet<string>> LoadFromTextFileAsync(string filePath)
{
    HashSet<string> usernames = new();
    string[] lines = await File.ReadAllLinesAsync(filePath);
    foreach (var line in lines)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            usernames.Add(line.Trim());
        }
    }
    return usernames;
}

async Task<HashSet<string>> LoadFromJsonFileAsync(string filePath)
{
    string json = await File.ReadAllTextAsync(filePath);
    return ParseJsonContributors(json, "8.0.0");
}

async Task<HashSet<string>> LoadFromJsonUrlAsync(string url)
{
    string json = await client.GetStringAsync(url);
    return ParseJsonContributors(json, "8.0.0");
}

HashSet<string> ParseJsonContributors(string json, string version)
{
    HashSet<string> usernames = new();
    JsonNode? data = JsonNode.Parse(json);

    if (data != null)
    {
        foreach (JsonNode node in data.AsArray())
        {
            if (node["Version"]?.GetValue<string>() == version)
            {
                var contributors = node["Contributors"]?.AsArray();
                if (contributors != null)
                {
                    foreach (var contributor in contributors)
                    {
                        string? link = contributor["Link"]?.GetValue<string>();
                        if (link != null)
                        {
                            int slashIndex = link.LastIndexOf('/');
                            string username = link.Substring(slashIndex + 1);
                            usernames.Add(username);
                        }
                    }
                }
            }
        }
    }
    return usernames;
}
