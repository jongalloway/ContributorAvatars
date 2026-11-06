# .NET 11 Upgrade Plan

## Overview

**Target**: ContributorAvatars on .NET 11 RC3 with roll-forward to installed .NET 11 GA.
**Scope**: One SDK-style console project, one source file, compatible dependencies.

### Selected Strategy
**All-At-Once** - All projects upgraded simultaneously in a single operation.
**Rationale**: One project already on .NET 10 with no project dependencies.

Projects: `ContributorAvatars.csproj`.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | Single small modern .NET project; no staged migration needed. |

## Tasks

### 01-upgrade-dotnet-11: Upgrade runtime and SDK selection and review C# modernizations

Verify the installed RC3 toolchain and upgrade ContributorAvatars from net10.0 to net11.0. Configure SDK and framework-dependent runtime selection to use RC3 now and prefer installed .NET 11 GA later without selecting .NET 12. Preserve the existing package versions and 3500-byte avatar threshold. The assessment flags one potential HttpContent behavioral change at the image download; validate byte-array reads rather than changing APIs speculatively.

Review Program.cs against C# 13 through 15 features and older opportunities left in the original .NET 8 code. Record useful recommendations and reasons not to introduce features that do not fit; do not perform unrelated source refactors. Update README prerequisites, dependency documentation, and roll-forward behavior.

**Done when**: SDK resolution selects the installed RC3; the project targets net11.0; generated runtime configuration has the RC3 minimum and a within-.NET-11 roll-forward policy; isolated resolution checks prefer a simulated GA candidate over RC3 and reject .NET 12-only candidates; the solution builds with zero warnings and errors; available tests or isolated smoke checks pass, including HttpContent bytes and avatar boundaries; C# review and runtime caveats are documented; unrelated behavior and local changes are preserved.
