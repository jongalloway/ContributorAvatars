# C# Modernization Assessment

**Project:** ContributorAvatars
**Current version:** C# 14 (net10.0); code originated on .NET 8 / C# 12
**Target version:** C# 15 (net11.0)
**Date:** 2026-11-05

## Summary

One source file, Program.cs (190 lines), was reviewed. C# 15 is enabled by the target framework; no LangVersion override or preview feature flag is required. No C# 15-specific rewrite improves the current program. Keep the existing top-level statements, async/await, target-typed new, nullable annotations, and using declaration for the image collection.

| Category | Files | Method |
|----------|-------|--------|
| Breaking changes | 0 affected | No with() methods, span escape patterns, dynamic Boolean operations, or affected attributes |
| Safe automated modernizations specific to C# 13-15 | 0 candidates | No applicable diagnostic fixes; do not add an empty .editorconfig |
| Earlier syntactic improvements | 1 candidate | Recommendations only, not applied |
| Behavioral or architectural changes | 1 candidate | Separate decisions, not part of this upgrade |

## Phase 0: Breaking changes

No affected constructs were found in the single source file. The framework assessment flags HttpContent.ReadAsByteArrayAsync as a potential behavioral change, not a C# source incompatibility. Validate byte-for-byte response reads and the avatar threshold on the new runtime instead of replacing that API speculatively.

## Phase 1: Automated transformations

No C# 13-15 auto-fixer applies: the code has no locks, explicit lambda parameter types, property accessors/backing fields, or generic nameof expressions. No formatter pass or style configuration is necessary for the requested review-only scope.

## Phase 2: Recommendations

| Recommendation | Version | Location | Rationale / constraints |
|----------------|---------|----------|-------------------------|
| Empty collection expressions: HashSet<string> usernames = []; | C# 12 | LoadFromTextFileAsync and ParseJsonContributors | Concise initialization; preserve the existing default comparer. Already available on .NET 8, not new in C# 15. |
| Range slicing: link[(slashIndex + 1)..] | C# 8 | ParseJsonContributors | Shorter equivalent of Substring(slashIndex + 1); retain the existing trailing-slash behavior. |
| Null patterns: data is not null, contributors is not null, link is not null | C# 9 | ParseJsonContributors | Express null checks directly; no need to change parsing behavior. |
| Static local helpers | C# 8 | LoadFromTextFileAsync, LoadFromJsonFileAsync, ParseJsonContributors | These helpers do not need captured top-level state. Do not mark LoadFromJsonUrlAsync static without passing its HttpClient. |

These are optional readability changes, not requirements for C# 15 compatibility. They are intentionally not applied during this framework upgrade.

## Phase 3: C# 15 features not applied

| Feature | Why not apply here |
|---------|--------------------|
| Collection expression arguments with(...) | Both HashSet instances use default constructors. No custom capacity/comparer arguments need preserving. Adding StringComparer.OrdinalIgnoreCase would change contributor deduplication and is a separate behavior decision, not a syntactic modernization. |
| Union types and closed hierarchies | No result/state type hierarchy exists; introducing one would enlarge this small script without solving a current problem. |
| Extension indexers | JsonNode and image collections already expose indexers; extensions would add indirection. |
| Labeled break and continue | No nested-loop exit flags or goto workarounds to replace. Existing loops have straightforward control flow. |
| Static interface members on older runtimes | No interfaces or old-runtime consumers; the project targets .NET 11. |
| Memory safety preview | No unsafe/pointer code; do not enable preview flags. |

C# 13 Lock/params collections and C# 14 field/extension blocks/null-conditional assignments likewise have no suitable current use site.

## Separate functional follow-ups (not changed)

- double ratio = 16 / 9 performs integer division, yielding 1 instead of 16:9. Use floating-point operands in a separate layout fix and verify the resulting grid and surplus selection.
- Existing cached images bypass the 3500-byte download filter; changing this needs a deliberate cache-cleanup policy.
- Relative paths such as ../../../images depend on the working directory. Consider resolving data paths explicitly before relying on dotnet run from arbitrary directories.
- Retry/counting behavior on HTTP failures and disposal of HttpClient/HttpResponseMessage deserve a separate reliability pass; these are not C# 15 features.

## Execution results

- Framework/language selection updated to net11.0 / C# 15.
- No optional language transformations or architectural features applied.
- Preserve the 3500-byte threshold and current collection comparison semantics.
- Compatibility/warning fixes, if required by validation, are recorded in the task progress report.

## References

- [What's new in C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15)
- [SDK selection and global.json](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json)
- [Runtime version selection](https://learn.microsoft.com/en-us/dotnet/core/versions/selection)
