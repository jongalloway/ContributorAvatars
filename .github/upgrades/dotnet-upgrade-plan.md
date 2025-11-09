# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade ContributorAvatars.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Aggregate NuGet packages modifications across all projects

No NuGet package updates are required for this upgrade.

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### ContributorAvatars.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

Feature upgrades:
  - None required

Other changes:
  - None required
