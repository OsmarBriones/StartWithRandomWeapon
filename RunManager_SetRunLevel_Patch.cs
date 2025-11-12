using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StartWithRandomWeapon
{
	[HarmonyPatch(typeof(RunManager), nameof(RunManager.SetRunLevel))]
	public class RunManager_SetRunLevel_Patch
	{
		static void Postfix(RunManager __instance)
		{
			if (!CanSpawn(__instance))
				return;

			StartWithRandomWeaponPlugin.EnsureItemsLoaded();

			StatsManager statsManager = StatsManager.instance;
			List<string> available = BuildAvailableList(statsManager);

			if (available.Count == 0)
			{
				Debug.Log("[StartWithItemPatch] No configured starting items are valid/present.");
				return;
			}

			int numberOfWeaponsToSpawn = GetSpawnCount(available.Count);
			List<string> selectedItems = SelectItems(available, numberOfWeaponsToSpawn);

			PurchaseItems(statsManager, selectedItems);

			Debug.Log($"[StartWithItemPatch] Spawned {selectedItems.Count} unique starting item(s): {string.Join(", ", selectedItems)}");
		}

		private static bool CanSpawn(RunManager runManager)
		{
			if (runManager.levelsCompleted != 0)
				return false;
			return StatsManager.instance != null;
		}

		private static List<string> BuildAvailableList(StatsManager stats)
		{
			return StartWithRandomWeaponPlugin.ConfigItemKeys
				.Where(k => stats.itemDictionary.ContainsKey(k))
				.Where(k =>
				{
					var itm = stats.itemDictionary[k];
					if (itm == null) return false;
					if (!itm.physicalItem) return false;
					if (itm.disabled) return false;
					if (stats.itemsPurchased.ContainsKey(k) && stats.itemsPurchased[k] > 0) return false;
					return true;
				})
				.Distinct()
				.ToList();
		}

		private static int GetSpawnCount(int availableCount)
		{
			// If config says spawn one per player, override numeric setting
			if (StartWithRandomWeaponConfig.SpawnOnePerPlayer != null &&
				StartWithRandomWeaponConfig.SpawnOnePerPlayer.Value)
			{
				int playerCount = 1;
				if (GameDirector.instance != null && GameDirector.instance.PlayerList != null)
				{
					playerCount = GameDirector.instance.PlayerList.Count;
				}
				if (playerCount < 1) playerCount = 1;
				if (playerCount > availableCount) playerCount = availableCount;
				return playerCount;
			}

			int requested = StartWithRandomWeaponConfig.WeaponsToSpawn?.Value ?? 1;
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
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				if (j == i) continue;
				var tmp = list[i];
				list[i] = list[j];
				list[j] = tmp;
			}
		}
	}
}
