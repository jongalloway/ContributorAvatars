# ContributorAvatars

A .NET console application that creates a visual mosaic of contributor avatars for .NET Core releases. This tool downloads GitHub profile pictures of contributors to a specific .NET Core version and arranges them into a beautiful mosaic image.

## What it does

ContributorAvatars:
1. Fetches contributor data for .NET Core 8.0.0 from Microsoft's official contributor JSON
2. Downloads GitHub avatar images (200x200px) for each contributor
3. Creates a mosaic image arranging all avatars in a grid layout
4. Automatically calculates optimal grid dimensions to create a 16:9 aspect ratio

## Features

- **Smart Grid Layout**: Automatically calculates the optimal grid dimensions (width × height) to create a visually appealing 16:9 aspect ratio
- **Progress Tracking**: Uses Spectre.Console to display download progress in the terminal
- **Intelligent Caching**: Skips downloading avatars that already exist locally
- **Random Arrangement**: Shuffles avatar positions for unique results each run
- **Error Handling**: Validates downloaded images (minimum 3.5 KB / 3500 bytes) to filter out placeholder images
- **Image Size Optimization**: Automatically adjusts the number of avatars to fit evenly into the calculated grid

## Prerequisites

- [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0): RC3 (`11.0.100-rc.3.26522.148`) or a newer .NET 11.0 SDK, including GA
- [ImageMagick](https://imagemagick.org/) (optional, for manual image manipulation)

### .NET version selection

`global.json` selects the latest installed .NET 11.0 SDK feature band at or above RC3 (`rollForward: latestFeature`, `allowPrerelease: true`). Targeting `net11.0` enables C# 15 by default, without a preview language setting.

The project uses RC3 (`11.0.0-rc.3.26522.148`) as its minimum runtime and `RollForward=LatestMinor`. Framework-dependent launches prefer the highest installed .NET 11 runtime, including GA even when RC3 remains installed. Neither SDK nor runtime selection rolls forward to .NET 12.

Install the .NET 11 GA SDK/runtime when available; roll-forward selects installed versions, it does not download them. Self-contained deployments bundle their runtime and must be republished to update it.

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/jongalloway/ContributorAvatars.git
   cd ContributorAvatars
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

## Usage

Run the application:
```bash
dotnet run
```

The program will:
1. Display the total number of contributors found
2. Show a progress bar while downloading avatars
3. Generate the mosaic image
4. Display statistics about the generated image
5. Save the final image as `contributors.png` in the project directory

### Output

- **Individual Avatars**: Downloaded to the `images/` directory
- **Final Mosaic**: Saved as `contributors.png` in the project root

### Manual Customization

The program outputs an ImageMagick command that you can use to manually adjust the mosaic:
```bash
magick montage -geometry 200x200+0+0 -tile {w}x{h} images/*.png contributors.png
```

## Technical Details

### Dependencies

- **[Magick.NET-Q16-AnyCPU](https://github.com/dlemstra/Magick.NET)** (v15.0.0): .NET wrapper for ImageMagick, used for image processing and montage creation
- **[Spectre.Console](https://spectreconsole.net/)** (v0.57.2): Library for creating beautiful console applications with progress bars and status displays

### C# modernization review

See the [C# modernization assessment](.github/upgrades/scenarios/dotnet-version-upgrade/csharp-language-modernization-assessment.md) for recommendations and features that do not benefit this application. The framework upgrade does not introduce unrelated source refactors.

### How it Works

1. **Data Retrieval**: Fetches contributor information from `https://dotnet.microsoft.com/blob-assets/json/thanks/core.json`
2. **Filtering**: Extracts contributors specifically for version 8.0.0
3. **Avatar Download**: Downloads profile pictures from `https://www.github.com/{username}.png?size=200`
4. **Grid Calculation**: Computes grid dimensions to approximate 16:9 ratio using the formula:
   - Height = √(total_avatars / 1.7778)
   - Width = Height × 1.7778
5. **Mosaic Creation**: Uses ImageMagick's montage functionality to arrange avatars with:
   - Geometry: 200x200 pixels per avatar
   - Border width: 0
   - Background: Transparent

## Example Output

```
Total contributors: 627
Downloading... [████████████████████████████] 627/627
Removing 3 random avatars to fit an even multiple.
Generating image...
Total avatars downloaded: 627
Image written to contributors.png
Use the following to tweak and generate a mosaic:
magick montage -geometry 200x200+0+0 -tile 33x19 images/*.png contributors.png
Press any key to quit.
```

## License

MIT License - see [LICENSE.txt](LICENSE.txt) for details.

## Acknowledgments

This project celebrates the amazing contributors to .NET Core. Thank you to everyone who has contributed to making .NET better!

## Resources

- [ImageMagick Montage Documentation](https://imagemagick.org/script/montage.php)
- [Quick & Easy Mosaics with ImageMagick](https://sinestesia.co/blog/tutorials/quick-n-easy-mosaics-with-imagemagick/)