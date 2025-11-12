using BepInEx;
using HarmonyLib;
using StartWithRandomWeapon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[BepInPlugin("com.osmarbriones.startwithrandomweapon", "Start With Random Weapon", "1.0.0")]
public class StartWithRandomWeaponPlugin : BaseUnityPlugin
{
	internal static List<string> ConfigItemKeys = new List<string>();

	private const string DefaultFileName = "com.osmarbriones.startwithrandomweapon_weaponsList.txt";
	private static string _configFilePath;

	private void Awake()
	{
		Debug.Log("StartWithRandomWeaponPlugin initializing...");
		_configFilePath = Path.Combine(BepInEx.Paths.ConfigPath, DefaultFileName);
		StartWithRandomWeaponConfig.BindConfig(this);

		var harmony = new Harmony("com.osmarbriones.startwithrandomweapon");
		harmony.PatchAll();
		Debug.Log("StartWithRandomWeaponPlugin initialized.");
	}

	/// <summary>
	/// Ensures list is loaded at least once (called from patch lazily).
	/// </summary>
	internal static void EnsureItemsLoaded()
	{
		ReloadItemConfig();
	}

	/// <summary>
	/// Public manual reload (can be invoked via reflection, console integration, or keybind).
	/// </summary>
	public static void ReloadItemConfig()
	{
		try
		{
			if (!File.Exists(_configFilePath))
			{
				string[] defaults =
				{
					"#Item Cart Cannon",
					"#Item Cart Laser",
					"#Item Gun Handgun",
					"#Item Gun Laser",
					"#Item Gun Shockwave",
					"#Item Gun Shotgun",
					"#Item Gun Stun",
					"#Item Gun Tranq",
					"Item Melee Baseball Bat",
					"Item Melee Frying Pan",
					"Item Melee Inflatable Hammer",
					"#Item Melee Sledge Hammer",
					"Item Melee Stun Baton",
					"Item Melee Sword",
					"Item Rubber Duck"
				};

				File.WriteAllLines(_configFilePath, new[]
				{
					"# One item name per line.",
					"# Lines starting with # are comments.",
					"# Blank lines are ignored.",
					""
				}.Concat(defaults));

				Debug.Log("[StartWithRandomWeapon] Created default item list file: " + _configFilePath);
				ConfigItemKeys = new List<string>(defaults);
			}
			else
			{
				string[] lines = File.ReadAllLines(_configFilePath);
				ConfigItemKeys = lines
					.Select(l => l.Trim())
					.Where(l => l.Length > 0 && !l.StartsWith("#"))
					.Distinct()
					.ToList();

				Debug.Log($"[StartWithRandomWeapon] Reloaded {ConfigItemKeys.Count} item keys from: {_configFilePath}");
			}

			if (ConfigItemKeys.Count == 0)
			{
				Debug.LogWarning("[StartWithRandomWeapon] Item list is empty after reload.");
			}
			else
			{
				foreach (string itemKey in ConfigItemKeys)
				{
					Debug.Log($"[StartWithRandomWeapon]   Item Key: {itemKey}");
				}
			}

		}
		catch (Exception ex)
		{
			Debug.LogError("[StartWithRandomWeapon] Reload failed: " + ex.Message);
			ConfigItemKeys = new List<string>();
		}
	}
}
