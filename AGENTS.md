# StartWithRandomWeapon — Local Agent Context

This is an independent BepInEx 5.x mod repository for R.E.P.O. It patches
`RunManager.SetRunLevel` to grant random starting weapons. Its entry point is
`StartWithRandomWeaponPlugin.cs`, configuration lives in
`Configuration/StartWithRandomWeaponConfig.cs`, and it builds
`StartWithRandomWeapon.dll` for .NET Framework 4.8. See `ARCHITECTURE.md` for
the data flow and design decisions.

## Working rules

- This repository must remain usable after a normal clone. Do not require its
  parent workspace for normal builds or documentation.
- Build with `dotnet build StartWithRandomWeapon.csproj`. The post-build step
  deploys the DLL to the Steam plugins folder and to the local r2modman Debug
  profile (`BepInEx/plugins`, as a loose DLL).
- Configuration and paths:
  - Game paths and target framework are configured in `Directory.Build.props`.
  - Override the default Steam path with the `REPO_GAME_DIR` environment variable.
- Shared code and guidance:
  - `external/RepoAPI` provides `Items/ItemName.cs` and `Game/**`, compiled in via
    `<Compile Include>` in the `.csproj`. Add new item keys to RepoAPI, not here.
  - `external/RepoKit` holds the canonical workspace guidance.
  - Clone with `git clone --recurse-submodules`, or run `git submodule update --init`.
- Invariants:
  - Keep the plugin GUID `com.osmarbriones.startwithrandomweapon`: it names the
    user's `.cfg` file.
  - `[Weapons]` config keys are the game's item keys; do not rename them.
  - Defaults follow the shop price (enabled up to $20K), see `ARCHITECTURE.md`.
- Preserve existing local modifications. Do not reset, discard, or overwrite
  unrelated work.
- Author vs. Technical Identifier: the human author is **always** `Osmar Briones` (README credits, `<Authors>` tag). Technical reverse-DNS `com.osmar` is reserved for GUIDs, config file names and namespaces; this mod keeps its legacy GUID `com.osmarbriones.startwithrandomweapon` on purpose (see Invariants).
- Player-friendly README: `## Features` describes gameplay in accessible language. Never leak Unity hooks, Harmony patches, class/method names or architecture into `README.md`; those belong in `ARCHITECTURE.md`.
- Spec Kit (SDD): pre-configured in `.specify/`; its constitution at `.specify/memory/constitution.md` enforces RepoKit, host authority and the coding standards. Use `/speckit-specify`, `/speckit-plan` and `/speckit-tasks` when developing features.
- Testing: follow the 3-tier strategy in `external/RepoKit/REPO_MODS_METHODOLOGY.md` §8.
- Coding standards: follow `external/RepoKit/REPO_MODS_METHODOLOGY.md` §6.
- Maintain documentation with implementation: update this file, `README.md`,
  `CHANGELOG.md`, and `ARCHITECTURE.md` when runtime or design facts change.
- Documentation style: in `README.md`, keep features player-facing and
  non-technical. In Credits and `<Authors>`, use **Osmar Briones**.

Before the first edit of every task, synchronize
`external/RepoKit` once and follow its synchronization gate. Read
its `VERSION.md`, `REPO_MODS_WORKSPACE.md`, and
`REPO_MODS_METHODOLOGY.md`; do not repeat that check during the same task
unless shared guidance changes.
