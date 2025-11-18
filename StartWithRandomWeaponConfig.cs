using BepInEx.Configuration;
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

		public static Dictionary<string, ConfigEntry<bool>> WeaponToggles =
			new Dictionary<string, ConfigEntry<bool>>();

		public static Dictionary<string, bool> WeaponDefaults { get; } = new Dictionary<string, bool>
		{
			{ "Item Rubber Duck", true },
			{ "Item Melee Inflatable Hammer", true },
			{ "Item Melee Frying Pan", true },
			{ "Item Melee Sword", true },
			{ "Item Melee Stun Baton", true },
			{ "Item Melee Baseball Bat", true },
			{ "Item Melee Sledge Hammer", false },
			{ "Item Gun Shockwave", false },
			{ "Item Gun Stun", false },
			{ "Item Gun Handgun", false },
			{ "Item Gun Tranq", false },
			{ "Item Gun Laser", false },
			{ "Item Gun Shotgun", false },
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