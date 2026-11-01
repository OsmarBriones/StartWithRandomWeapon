#nullable enable
using HarmonyLib;
using StartWithRandomWeapon.Configuration;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StartWithRandomWeapon.Patches;

/// <summary>
/// Gives the configured starting weapons as free shop purchases when a new run begins.
/// </summary>
[HarmonyPatch(typeof(RunManager), nameof(RunManager.SetRunLevel))]
internal static class RunManager_SetRunLevel_Patch
{
	private static void Postfix(RunManager __instance)
	{
		if (!CanSpawn(__instance))
			return;

		StartWithRandomWeaponConfig.Reload();

		var statsManager = StatsManager.instance;
		var available = BuildAvailableList(statsManager);

		if (available.Count == 0)
		{
			StartWithRandomWeaponPlugin.Logger.LogInfo("No configured starting items are valid/present.");
			return;
		}

		var count = GetSpawnCount(available.Count);
		var selectedItems = SelectItems(available, count);

		PurchaseItems(statsManager, selectedItems);

		StartWithRandomWeaponPlugin.Logger.LogInfo(
			$"Spawned {selectedItems.Count} unique starting item(s): {string.Join(", ", selectedItems)}");
	}

	private static bool CanSpawn(RunManager runManager)
	{
		if (runManager.levelsCompleted != 0)
			return false;
		return StatsManager.instance != null;
	}

	// Skips keys the running game version does not have, WIP/disabled items and ones already owned.
	private static List<string> BuildAvailableList(StatsManager stats)
	{
		return StartWithRandomWeaponConfig.GetEnabledWeaponKeys()
			.Where(key => stats.itemDictionary.ContainsKey(key))
			.Where(key =>
			{
				var item = stats.itemDictionary[key];
				if (item == null) return false;
				if (!item.physicalItem) return false;
				if (item.disabled) return false;
				if (stats.itemsPurchased.ContainsKey(key) && stats.itemsPurchased[key] > 0) return false;
				return true;
			})
			.Distinct()
			.ToList();
	}

	private static int GetSpawnCount(int availableCount)
	{
		// One per player overrides the numeric setting.
		if (StartWithRandomWeaponConfig.SpawnOnePerPlayer)
		{
			var playerCount = 1;
			if (GameDirector.instance != null && GameDirector.instance.PlayerList != null)
			{
				playerCount = GameDirector.instance.PlayerList.Count;
			}
			if (playerCount < 1) playerCount = 1;
			if (playerCount > availableCount) playerCount = availableCount;
			return playerCount;
		}

		var requested = StartWithRandomWeaponConfig.WeaponsToSpawn;
		if (requested < 1) requested = 1;
		if (requested > 50) requested = 50;
		if (requested > availableCount) requested = availableCount;
		return requested;
	}

	private static List<string> SelectItems(List<string> available, int count)
	{
		Shuffle(available);
		return available.Take(count).ToList();
	}

	private static void PurchaseItems(StatsManager statsManager, List<string> selectedItems)
	{
		foreach (var key in selectedItems)
		{
			statsManager.ItemPurchase(key);
		}
	}

	private static void Shuffle(List<string> list)
	{
		for (var i = list.Count - 1; i > 0; i--)
		{
			var j = Random.Range(0, i + 1);
			if (j == i) continue;
			(list[i], list[j]) = (list[j], list[i]);
		}
	}
}
