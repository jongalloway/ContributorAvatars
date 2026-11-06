# .NET 11 Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net11.0
- **SDK Baseline**: 11.0.100-rc.3.26522.148
- **Roll Forward**: Prefer newer installed .NET 11 SDKs and runtimes, including GA; do not roll forward to .NET 12. Updates must be installed separately.
- **C# Review**: Review useful C# 15 modernizations, including older opportunities from the original .NET 8 code; avoid unrelated rewrites. Recommend source changes before applying them.

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-11
- **Pending Changes**: Committed before branching, preserving the avatar filter and dependency changes.
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Key Decisions Log
- 2026-11-05: User confirmed target, roll-forward scope, review scope, automatic execution, and source-control settings.
- 2026-11-05: User confirmed All-at-Once and one atomic upgrade commit; use Single Commit at End, superseding the initial After Each Task preference.

## Upgrade Options
- **Upgrade Strategy**: All-at-Once

## Strategy
**Selected**: All-at-Once
**Rationale**: One SDK-style .NET 10 console project with compatible dependencies.

### Execution Constraints
- Upgrade the single project atomically, retaining compatible package versions.
- Validate SDK and runtime selection independently; SDK roll-forward does not configure runtime selection.
- Build the entire solution warning-free, then run isolated smoke checks; no test projects are present.
- Keep C# source changes review-only except necessary compatibility or warning fixes.
- **Effective Commit Strategy**: Single Commit at End

## Build Tool Decisions
- **ContributorAvatars.csproj**: Native IDE build; dotnet build for clean captured diagnostics and final validation (SDK-style console project, no special build requirements).
