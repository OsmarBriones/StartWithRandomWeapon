#nullable enable
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StartWithRandomWeapon.Configuration;
using UnityEngine;

namespace StartWithRandomWeapon;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class StartWithRandomWeaponPlugin : BaseUnityPlugin
{
	// The GUID names the config file (BepInEx/config/<GUID>.cfg); keep it stable so existing settings survive.
	public const string PluginGuid = "com.osmarbriones.startwithrandomweapon";
	public const string PluginName = "StartWithRandomWeapon";
	public const string PluginVersion = "1.2.0";

	internal Harmony? Harmony { get; set; }
	internal static new ManualLogSource Logger { get; private set; } = null!;

	private void Awake()
	{
		Logger = base.Logger;

		// Prevent the plugin from being deleted
		gameObject.transform.parent = null;
		gameObject.hideFlags = HideFlags.HideAndDontSave;

		StartWithRandomWeaponConfig.Initialize(Config);

		Harmony = new Harmony(Info.Metadata.GUID);
		Harmony.PatchAll();

		Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
	}

	internal void Unpatch()
	{
		Harmony?.UnpatchSelf();
	}
}
