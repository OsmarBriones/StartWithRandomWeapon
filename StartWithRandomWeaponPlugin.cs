#nullable enable
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StartWithRandomWeapon.Configuration;

namespace StartWithRandomWeapon;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class StartWithRandomWeaponPlugin : BaseUnityPlugin
{
	// The GUID names the config file (BepInEx/config/<GUID>.cfg); keep it stable so existing settings survive.
	public const string PluginGuid = "com.osmarbriones.startwithrandomweapon";
	public const string PluginName = "StartWithRandomWeapon";
	public const string PluginVersion = "1.1.0";

	internal static new ManualLogSource Logger { get; private set; } = null!;

	private void Awake()
	{
		Logger = base.Logger;

		StartWithRandomWeaponConfig.Initialize(Config);

		new Harmony(PluginGuid).PatchAll();

		Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
	}
}
