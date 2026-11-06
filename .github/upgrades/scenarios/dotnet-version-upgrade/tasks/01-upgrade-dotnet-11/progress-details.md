# .NET 11 Upgrade Results

## Changes

- ContributorAvatars.csproj now targets net11.0, with RuntimeFrameworkVersion=11.0.0-rc.3.26522.148 and RollForward=LatestMinor. C# 15 is the evaluated default; no preview language setting added.
- global.json selects SDK 11.0.100-rc.3.26522.148 or a newer installed .NET 11.0 feature band, with allowPrerelease=true and rollForward=latestFeature.
- Preserve Magick.NET-Q16-AnyCPU 15.0.0, Spectre.Console 0.57.2, and the image.Length >= 3500 download predicate.
- Resolve three existing nullable warnings in Program.cs: nullable release elements and null contributor elements now fail explicitly with JsonException instead of incidental NullReferenceException. Valid JSON behavior is preserved.
- README documents RC3 prerequisites, separate SDK/runtime selection, installing GA updates, self-contained republishing, current package versions, and the C# review.
- csharp-language-modernization-assessment.md reviews current C# 15 features and older opportunities. No optional refactors applied; collection expressions, slicing, null patterns and static helpers remain recommendations. Layout/path/cache/retry issues are separate follow-ups.
- Remove IDE-generated explicit None includes for workflow Markdown; SDK default items already cover repository content.

## Validation

- Installed SDK: 11.0.100-rc.3.26522.148. Installed runtime: 11.0.0-rc.3.26522.148. SDK installation and global.json validation succeeded.
- Clean full solution build: 0 warnings, 0 errors. NETSDK1057 is the expected informational prerelease SDK message, not a warning.
- Evaluated properties: TargetFramework=net11.0, LangVersion=15.0, RuntimeFrameworkVersion=11.0.0-rc.3.26522.148, RollForward=LatestMinor.
- Generated runtimeconfig.json independently verified for net11.0, RC3 minimum and LatestMinor.
- Disposable isolated host checks passed for SDK and runtime: RC3-only selects RC3; RC3 + simulated GA + .NET 12 selects .NET 11 GA; .NET 12-only fails rather than rolling forward across the major boundary.
- GA version-directory simulations reused installed RC binaries. These prove host/SDK version selection, not actual GA binary compatibility; GA was not installed or downloaded.
- Seven HttpClient/HttpContent byte-array cases passed with exact byte equality at 1800, 3250, 3328, 3499, 3500, 3501 and 4096 bytes. Production predicate checked unchanged; only >=3500 cases are accepted.
- Reflection calls to the compiled production JSON parser passed version filtering, duplicate removal, case-sensitive distinction, missing/null links, empty array, top-level null, and explicit errors for null release and contributor entries.
- Production application launched on RC3 with one generated cached avatar in an isolated directory and successfully produced contributors.png without network downloads or modifying repository images.
- No Test Explorer tests or test projects exist. Disposable probe and smoke checks supplied validation without adding a persistent test project or test dependencies.

## Issues and Deviations

- The first smoke fixture contained an invalid PNG CRC. Regenerated the fixture using Windows image encoding; all checks then passed. No production change was needed for that fixture failure.
- The clean rebuild exposed existing nullable warnings hidden by the initial IDE incremental build. Applied the narrowly scoped explicit JSON errors required for warning-free completion.
- No SDKs, runtimes, or packages were installed. Roll-forward only selects installed versions; self-contained applications need republishing.
- Disposable verification files, host junctions and generated montage were cleaned up after validation.
