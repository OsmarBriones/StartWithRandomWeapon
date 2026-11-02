# Changelog

## [Unreleased]
### Added
- New weapons from the Sunstone update: Melee Whip and Gun Revolver.
- The three staffs (Zero Gravity, Torque, Void) can now be chosen as starting weapons.
- An `Enabled` setting to turn the mod off without uninstalling it.
- Two switches in the config menu: `ResetWeaponsToDefault` and `DisableAllWeapons`.

### Changed
- Default weapons now follow the shop price: weapons up to $20K start enabled, $22K and up start disabled. Existing config files keep their choices; use `ResetWeaponsToDefault` to apply the new defaults.
- Settings are saved under the same file as before; no action is needed after updating.

## 1.1.0
### Changed
- Unified all configuration into a single `.cfg` file.  
  The legacy `weapons configuration` file has been removed.
- Updated the logging system to use `Logger` instead of `Debug.Log`.

### Notes
- This update changes how configuration works.  
  Users may need to adjust their settings after updating.

## 1.0.1
### Fixed
- Corrected the mod title in the README file.

## 1.0.0
### Added
- First release!
