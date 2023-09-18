using System;
using System.IO;

namespace VegaX.Classes.DLL
{
	class DLLFileSystem
	{
		public static string DLLPath = Directory.GetCurrentDirectory() + "\\bin\\VegaX.dll";

		public static string RBPath = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Packages\\ROBLOXCORPORATION.ROBLOX_55nm5eh3cm0pr\\AC\\";

		public static string AutoexecPath = DLLFileSystem.RBPath + "autoexec\\";

		public static string WorkspacePath = DLLFileSystem.RBPath + "workspace\\";
	}
}
