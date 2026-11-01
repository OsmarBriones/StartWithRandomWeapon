using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StartWithRandomWeapon
{
	// Separate configurable settings holder (does not touch the existing patch file).
	internal static class StartWithRandomWeaponConfig
	{
		public static StartWithRandomWeaponPlugin Plugin => StartWithRandomWeaponPlugin.Instance;

		// Number of weapons to spawn (ignored if SpawnOnePerPlayer is true).
		public static ConfigEntry<int> WeaponsToSpawn;

		// If true, ignore WeaponsToSpawn and spawn one weapon per current player.
		public static ConfigEntry<bool> SpawnOnePerPlayer;

		// Self-resetting switches: set to true (e.g. from the in-game config menu) to apply the action once.
		public static ConfigEntry<bool> ResetWeaponsToDefault;

		public static ConfigEntry<bool> DisableAllWeapons;

		public static Dictionary<string, ConfigEntry<bool>> WeaponToggles =
			new Dictionary<string, ConfigEntry<bool>>();

		// Defaults follow the shop price (RepoKit/references/item-prices.md): enabled up to $20K,
		// disabled from $22K up, like the hard enemy tier in EnemyDrops.
		public static Dictionary<string, bool> WeaponDefaults { get; } = new Dictionary<string, bool>
		{
			{ "Item Rubber Duck", true },
			{ "Item Melee Inflatable Hammer", true },
			{ "Item Melee Frying Pan", true },
			{ "Item Melee Sword", true },
			{ "Item Melee Stun Baton", false },
			{ "Item Melee Baseball Bat", false },
			{ "Item Melee Whip", true },
			{ "Item Melee Sledge Hammer", false },
			{ "Item Gun Shockwave", true },
			{ "Item Gun Stun", true },
			{ "Item Gun Handgun", true },
			{ "Item Gun Revolver", false },
			{ "Item Gun Tranq", true },
			{ "Item Gun Laser", false },
			{ "Item Gun Shotgun", false },
			{ "Item Staff Zero Gravity", true },
			{ "Item Staff Torque", false },
			{ "Item Staff Void", false },
			{ "Item Cart Cannon", false },
			{ "Item Cart Laser", false },
		};

		// Call this once from your Plugin Awake (after Log is set).
		public static void BindConfig()
		{
			WeaponsToSpawn = Plugin.Config.Bind(
				"General",
				"WeaponsToSpawn",
				3,
				new ConfigDescription(
					"Number of starting weapons to spawn (ignored if SpawnOnePerPlayer is true).",
					new AcceptableValueRange<int>(1, 50))
			);

			SpawnOnePerPlayer = Plugin.Config.Bind(
				"General",
				"SpawnOnePerPlayer",
				true,
				new ConfigDescription(
					"If true, ignore WeaponsToSpawn and spawn one starting weapon per current player.")
			);

			ResetWeaponsToDefault = Plugin.Config.Bind(
				"General",
				"ResetWeaponsToDefault",
				false,
				new ConfigDescription(
					"Set to true to restore every weapon toggle to its default value. The switch turns itself off afterwards.")
			);
			ResetWeaponsToDefault.SettingChanged += OnResetWeaponsToDefaultChanged;

			DisableAllWeapons = Plugin.Config.Bind(
				"General",
				"DisableAllWeapons",
				false,
				new ConfigDescription(
					"Set to true to turn every weapon toggle off, so you can enable only the ones you want. The switch turns itself off afterwards.")
			);
			DisableAllWeapons.SettingChanged += OnDisableAllWeaponsChanged;

			BindWeaponToggles();


			StartWithRandomWeaponPlugin.Log(
				$"Config loaded: WeaponsToSpawn = {WeaponsToSpawn.Value}, " +
				$"SpawnOnePerPlayer = {SpawnOnePerPlayer.Value}, " +
				$"WeaponToggles = {string.Join(", ", WeaponToggles.Select(kvp => $"{kvp.Key} = {kvp.Value.Value}"))}");
		}

		public static void BindWeaponToggles()
		{
			foreach (var kvp in WeaponDefaults)
			{
				var weaponName = kvp.Key;
				var defaultEnabled = kvp.Value;

				var entry = Plugin.Config.Bind(
					"Weapons",
					weaponName,
					defaultEnabled,
					new ConfigDescription($"Enable spawning {weaponName} weapon at start.")
				);

				WeaponToggles[weaponName] = entry;
			}
		}

		private static void OnResetWeaponsToDefaultChanged(object sender, EventArgs e)
		{
			if (!ResetWeaponsToDefault.Value) return;

			foreach (var kvp in WeaponDefaults)
				WeaponToggles[kvp.Key].Value = kvp.Value;

			ResetWeaponsToDefault.Value = false;
			Plugin.Config.Save();
			StartWithRandomWeaponPlugin.Log("All weapon toggles were reset to their defaults.");
		}

		private static void OnDisableAllWeaponsChanged(object sender, EventArgs e)
		{
			if (!DisableAllWeapons.Value) return;

			foreach (var entry in WeaponToggles.Values)
				entry.Value = false;

			DisableAllWeapons.Value = false;
			Plugin.Config.Save();
			StartWithRandomWeaponPlugin.Log("All weapon toggles were turned off.");
		}

		public static void ReloadConfig()
		{
			Plugin.Config.Reload();
		}

		public static List<string> GetEnabledWeaponKeys()
		{
			return WeaponToggles
				.Where(kvp => kvp.Value.Value)
				.Select(kvp => kvp.Key)
				.ToList();
		}
	}
}