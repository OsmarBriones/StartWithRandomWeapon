using BepInEx.Configuration;
using UnityEngine;

namespace StartWithRandomWeapon
{
	// Separate configurable settings holder (does not touch the existing patch file).
	internal static class StartWithRandomWeaponConfig
	{
		// Number of weapons to spawn (ignored if SpawnOnePerPlayer is true).
		internal static ConfigEntry<int> WeaponsToSpawn;

		// If true, ignore WeaponsToSpawn and spawn one weapon per current player.
		internal static ConfigEntry<bool> SpawnOnePerPlayer;

		// Call this once from your plugin Awake (after Log is set). 
		internal static void BindConfig(StartWithRandomWeaponPlugin plugin)
		{
			WeaponsToSpawn = plugin.Config.Bind(
				"General",
				"WeaponsToSpawn",
				3,
				new ConfigDescription(
					"Number of starting weapons to spawn (ignored if SpawnOnePerPlayer is true).",
					new AcceptableValueRange<int>(1, 50))
			);

			SpawnOnePerPlayer = plugin.Config.Bind(
				"General",
				"SpawnOnePerPlayer",
				true,
				new ConfigDescription(
					"If true, ignore WeaponsToSpawn and spawn one starting weapon per current player.")
			);

			Debug.Log($"[StartWithRandomWeapon] Config loaded: WeaponsToSpawn = {WeaponsToSpawn.Value}, SpawnOnePerPlayer = {SpawnOnePerPlayer.Value}");
		}
	}
}