using BepInEx;
using HarmonyLib;
using StartWithRandomWeapon;

[BepInPlugin("com.osmarbriones.startwithrandomweapon", "StartWithRandomWeapon", "1.1.0")]
public class StartWithRandomWeaponPlugin : BaseUnityPlugin
{
	public static StartWithRandomWeaponPlugin Instance { get; set; }

	private void Awake()
	{
		Instance = this;

		Log("StartWithRandomWeaponPlugin initializing...");
		StartWithRandomWeaponConfig.BindConfig();

		var harmony = new Harmony("com.osmarbriones.startwithrandomweapon");
		harmony.PatchAll();
		Log("StartWithRandomWeaponPlugin initialized.");
	}

	public static void Log(string Message)
	{
		Instance.Logger.LogInfo(Message);
	}
}
