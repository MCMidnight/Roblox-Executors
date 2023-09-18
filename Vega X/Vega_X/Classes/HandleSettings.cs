using System;
using Microsoft.Win32;

namespace Vega_X.Classes
{
	class HandleSettings
	{
		public static void SaveString(string name, string value)
		{
			HandleSettings.RegistrySettings.SetValue(name, value);
		}

		public static string ReadSString(string name)
		{
			return (string)HandleSettings.RegistrySettings.GetValue(name);
		}

		public static void SaveBool(string name, bool value)
		{
			HandleSettings.RegistrySettings.SetValue(name, value);
		}

		public static bool ReadSBool(string name)
		{
			return Convert.ToBoolean(HandleSettings.RegistrySettings.GetValue(name));
		}

		public static void SaveValue(string name, double value)
		{
			HandleSettings.RegistrySettings.SetValue(name, value);
		}

		public static double ReadSValue(string name)
		{
			return Convert.ToDouble(HandleSettings.RegistrySettings.GetValue(name));
		}

		public static void SetDefaultSettings()
		{
			string keyName = "HKEY_CURRENT_USER\\Software\\VegaX";
			string[] array = new string[]
			{
				"Always_on_Top",
				"Check_Inject",
				"ScriptBlox_ScriptHub_DefaultPosture",
				"ToggleSetClipboard"
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (HandleSettings.RegistrySettings.GetValue(array[i], null) == null)
				{
					HandleSettings.SaveBool(array[i], true);
				}
				if (Registry.GetValue(keyName, array[i], null) == null)
				{
					HandleSettings.SaveBool(array[i], true);
				}
			}
			string[] array2 = new string[]
			{
				"Auto_Inject",
				"Currently_Installing_DLL"
			};
			for (int j = 0; j < array2.Length; j++)
			{
				if (HandleSettings.RegistrySettings.GetValue(array2[j], null) == null)
				{
					HandleSettings.SaveBool(array2[j], false);
				}
				if (Registry.GetValue(keyName, array2[j], null) == null)
				{
					HandleSettings.SaveBool(array2[j], false);
				}
			}
			if (Registry.GetValue(keyName, "Window_Transparency", null) == null)
			{
				HandleSettings.SaveValue("Window_Transparency", 1.0);
			}
			if (Registry.GetValue(keyName, "Accent_Color", null) == null)
			{
				HandleSettings.SaveString("Accent_Color", "#FFC33939");
			}
			if (Registry.GetValue(keyName, "Image_Background", null) == null)
			{
				HandleSettings.SaveString("Image_Background", "");
			}
		}

		static RegistryKey RegistrySettings = Registry.CurrentUser.CreateSubKey("SOFTWARE\\VegaX");
	}
}
