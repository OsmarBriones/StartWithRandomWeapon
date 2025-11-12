using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StartWithRandomWeapon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[BepInPlugin("com.osmarbriones.startwithrandomweapon", "Start With Random Weapon", "1.0.0")]
public class StartWithRandomWeaponPlugin : BaseUnityPlugin
{
	internal static ManualLogSource Log;
	internal static List<string> ConfigItemKeys = new List<string>();

	private const string DefaultFileName = "StartWithRandomWeaponItems_weaponsList.txt";
	private static string _configFilePath;

	private void Awake()
	{
		Log = Logger;
		Logger.LogInfo("StartWithRandomWeaponPlugin initializing...");
		_configFilePath = Path.Combine(BepInEx.Paths.ConfigPath, DefaultFileName);
		StartWithRandomWeaponConfig.BindConfig(this);

		var harmony = new Harmony("com.osmarbriones.startwithrandomweapon");
		harmony.PatchAll();
		Logger.LogInfo("StartWithRandomWeaponPlugin initialized.");
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
					"Item Cart Cannon",
					"Item Cart Laser",
					"Item Gun Handgun",
					"Item Gun Laser",
					"Item Gun Shockwave",
					"Item Gun Shotgun",
					"Item Gun Stun",
					"Item Gun Tranq",
					"Item Melee Baseball Bat",
					"Item Melee Frying Pan",
					"Item Melee Inflatable Hammer",
					"Item Melee Sledge Hammer",
					"Item Melee Stun Baton",
					"Item Melee Sword",
					"Item Rubber Duck"
				};

				File.WriteAllLines(_configFilePath, new[]
				{
					"# One item name per line.",
					"# Lines starting with # are comments.",
					"# Blank lines are ignored.",
					"# Press F6 in-game after editing to reload.",
					""
				}.Concat(defaults));

				Log?.LogInfo("[StartWithRandomWeapon] Created default item list file: " + _configFilePath);
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

				Log?.LogInfo($"[StartWithRandomWeapon] Reloaded {ConfigItemKeys.Count} item keys from: {_configFilePath}");
			}

			if (ConfigItemKeys.Count == 0)
			{
				Log?.LogWarning("[StartWithRandomWeapon] Item list is empty after reload.");
			}
		}
		catch (Exception ex)
		{
			Log?.LogError("[StartWithRandomWeapon] Reload failed: " + ex.Message);
			ConfigItemKeys = new List<string>();
		}
	}
}
