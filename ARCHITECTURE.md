# StartWithRandomWeapon Architecture

This document describes the runtime structure, data flow and design decisions of the mod.

---

## High-Level Concept

1. **Trigger:** a Harmony postfix on `RunManager.SetRunLevel`, which runs whenever a level is set up.
2. **Authority / Networking:** host-only (Category 1). Clients need nothing: the items are added through `StatsManager.ItemPurchase`, the same path a shop purchase uses, and the game's own sync replicates the result.
3. **Outcome:** on the first level of a run (`levelsCompleted == 0`) the mod "buys" N random weapons from the enabled list for free.

---

## Project Structure

```
StartWithRandomWeapon/
├── StartWithRandomWeaponPlugin.cs          # BepInEx entry point: binds config, applies Harmony patches
├── Configuration/
│   └── StartWithRandomWeaponConfig.cs      # Settings, weapon toggles, reset/disable-all switches
├── Patches/
│   └── RunManager_SetRunLevel_Patch.cs     # Picks and grants the starting weapons
└── external/
    ├── RepoAPI/                            # Submodule: only Items/ItemName.cs and Game/** are compiled in
    └── RepoKit/                            # Submodule: shared guidance and skills (not compiled)
```

---

## Main Data Flow

### 1. Startup
- `Awake` stores the logger, calls `StartWithRandomWeaponConfig.Initialize(Config)` and runs `Harmony.PatchAll()`.
- Initialization binds `[General]` settings and one `[Weapons]` toggle per entry of `weaponDefaults`.

### 2. Granting weapons (`RunManager_SetRunLevel_Patch`)
1. `CanSpawn`: only when `levelsCompleted == 0` and `StatsManager.instance` exists.
2. `StartWithRandomWeaponConfig.Reload()` re-reads the `.cfg` so edits made while the game is open apply to this run.
3. `BuildAvailableList` keeps the enabled keys that exist in `StatsManager.itemDictionary`, are `physicalItem`, are not `disabled` and are not already owned. Keys the running game version lacks are skipped silently.
4. `GetSpawnCount`: one per player when `SpawnOnePerPlayer`, otherwise `WeaponsToSpawn`, clamped to the available count.
5. The list is shuffled (Fisher-Yates with `UnityEngine.Random`) and the first N keys go to `StatsManager.ItemPurchase`.

### 3. Self-resetting switches
- `ResetWeaponsToDefault` and `DisableAllWeapons` are boolean entries with a `SettingChanged` handler. When set to `true` (config menu or file reload) the handler rewrites the toggles, sets itself back to `false` and saves the file.

---

## Key Design Decisions & Invariants

- **Plugin GUID stays `com.osmarbriones.startwithrandomweapon`.** BepInEx names the config file after it; changing it would orphan every user's saved settings. It is not the `com.osmar` identifier used by newer mods.
- **Config keys are the game's item keys** (`Item Melee Whip`, ...). The weapon list is declared with `RepoAPI.Items.ItemName` and converted with `GetGameKey()`, so a typo is a compile error and a key never drifts from RepoAPI.
- **Defaults follow the shop price** (`RepoKit/references/item-prices.md`): enabled up to $20K, disabled from $22K. Defaults only apply to entries missing from an existing `.cfg`.
- **What counts as a weapon** comes from the game's `itemType`: `melee` and `gun` items, plus the three `launcher` staffs and the Rubber Duck, which the mod has always offered. Grenades and mines are intentionally left out.
- **Newer items are WIP-aware:** an item the game marks `disabled`, or that does not exist in the running version, is never granted.
- **RepoAPI reuse decision:** only `ItemName` and the `GameKey` helpers are shared. The weapon classification and defaults stay in the mod because no other mod needs them.

---

## Testing Strategy

Follows `external/RepoKit/REPO_MODS_METHODOLOGY.md` §8. There is no off-game logic worth a unit-test project yet; the default check is a build plus launching the game as host and reading the `Spawned N unique starting item(s)` log line.
