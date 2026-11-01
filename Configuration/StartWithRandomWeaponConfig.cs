#nullable enable
using BepInEx.Configuration;
using RepoAPI.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StartWithRandomWeapon.Configuration;

/// <summary>
/// Binds the mod settings: how many weapons to give and one on/off toggle per weapon.
/// </summary>
internal static class StartWithRandomWeaponConfig
{
	private static ConfigFile? configFile;

	// Number of weapons to spawn (ignored if SpawnOnePerPlayer is true).
	private static ConfigEntry<int>? weaponsToSpawn;

	// If true, ignore WeaponsToSpawn and spawn one weapon per current player.
	private static ConfigEntry<bool>? spawnOnePerPlayer;

	// Self-resetting switches: set to true (e.g. from the in-game config menu) to apply the action once.
	private static ConfigEntry<bool>? resetWeaponsToDefault;

	private static ConfigEntry<bool>? disableAllWeapons;

	private static readonly Dictionary<string, ConfigEntry<bool>> weaponToggles = new();

	// Defaults follow the shop price (RepoKit/references/item-prices.md): enabled up to $20K,
	// disabled from $22K up, like the hard enemy tier in EnemyDrops.
	// Config keys are the game's item keys, so renaming an entry would drop users' saved settings.
	private static readonly (ItemName Item, bool Enabled)[] weaponDefaults =
	{
		(ItemName.RubberDuck, true),
		(ItemName.MeleeInflatableHammer, true),
		(ItemName.MeleeFryingPan, true),
		(ItemName.MeleeSword, true),
		(ItemName.MeleeStunBaton, false),
		(ItemName.MeleeBaseballBat, false),
		(ItemName.MeleeWhip, true),
		(ItemName.MeleeSledgeHammer, false),
		(ItemName.GunShockwave, true),
		(ItemName.GunStun, true),
		(ItemName.GunHandgun, true),
		(ItemName.GunRevolver, false),
		(ItemName.GunTranq, true),
		(ItemName.GunLaser, false),
		(ItemName.GunShotgun, false),
		(ItemName.StaffZeroGravity, true),
		(ItemName.StaffTorque, false),
		(ItemName.StaffVoid, false),
		(ItemName.CartCannon, false),
		(ItemName.CartLaser, false),
	};

	internal static int WeaponsToSpawn => weaponsToSpawn?.Value ?? 1;

	internal static bool SpawnOnePerPlayer => spawnOnePerPlayer?.Value ?? false;

	/// <summary>
	/// Binds every setting. Call once from the plugin's Awake.
	/// </summary>
	internal static void Initialize(ConfigFile config)
	{
		configFile = config ?? throw new ArgumentNullException(nameof(config));

		weaponsToSpawn = config.Bind(
			"General",
			"WeaponsToSpawn",
			3,
			new ConfigDescription(
				"Number of starting weapons to spawn (ignored if SpawnOnePerPlayer is true).",
				new AcceptableValueRange<int>(1, 50)));

		spawnOnePerPlayer = config.Bind(
			"General",
			"SpawnOnePerPlayer",
			true,
			new ConfigDescription(
				"If true, ignore WeaponsToSpawn and spawn one starting weapon per current player."));

		resetWeaponsToDefault = config.Bind(
			"General",
			"ResetWeaponsToDefault",
			false,
			new ConfigDescription(
				"Set to true to restore every weapon toggle to its default value. The switch turns itself off afterwards."));
		resetWeaponsToDefault.SettingChanged += OnResetWeaponsToDefaultChanged;

		disableAllWeapons = config.Bind(
			"General",
			"DisableAllWeapons",
			false,
			new ConfigDescription(
				"Set to true to turn every weapon toggle off, so you can enable only the ones you want. The switch turns itself off afterwards."));
		disableAllWeapons.SettingChanged += OnDisableAllWeaponsChanged;

		foreach (var (item, enabled) in weaponDefaults)
		{
			var key = item.GetGameKey();
			weaponToggles[key] = config.Bind(
				"Weapons",
				key,
				enabled,
				new ConfigDescription($"Enable spawning {key} weapon at start."));
		}

		StartWithRandomWeaponPlugin.Logger.LogInfo(
			$"Config loaded: WeaponsToSpawn = {WeaponsToSpawn}, " +
			$"SpawnOnePerPlayer = {SpawnOnePerPlayer}, " +
			$"WeaponToggles = {string.Join(", ", weaponToggles.Select(kvp => $"{kvp.Key} = {kvp.Value.Value}"))}");
	}

	/// <summary>
	/// Re-reads the config file so edits made while the game runs apply to the next run.
	/// </summary>
	internal static void Reload() => configFile?.Reload();

	/// <summary>
	/// Game keys of every weapon whose toggle is currently on.
	/// </summary>
	internal static List<string> GetEnabledWeaponKeys() =>
		weaponToggles
			.Where(kvp => kvp.Value.Value)
			.Select(kvp => kvp.Key)
			.ToList();

	private static void OnResetWeaponsToDefaultChanged(object sender, EventArgs e)
	{
		if (resetWeaponsToDefault is not { Value: true }) return;

		foreach (var (item, enabled) in weaponDefaults)
			weaponToggles[item.GetGameKey()].Value = enabled;

		resetWeaponsToDefault.Value = false;
		configFile?.Save();
		StartWithRandomWeaponPlugin.Logger.LogInfo("All weapon toggles were reset to their defaults.");
	}

	private static void OnDisableAllWeaponsChanged(object sender, EventArgs e)
	{
		if (disableAllWeapons is not { Value: true }) return;

		foreach (var entry in weaponToggles.Values)
			entry.Value = false;

		disableAllWeapons.Value = false;
		configFile?.Save();
		StartWithRandomWeaponPlugin.Logger.LogInfo("All weapon toggles were turned off.");
	}
}
