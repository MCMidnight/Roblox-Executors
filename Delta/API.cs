using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows;

namespace Delta
{
	public static class API
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr OpenProcess(uint access, bool inhert_handle, int pid);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

		[DllImport("./bin/Fluxteam_net_API.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool run_script(IntPtr proc, int pid, string path, [MarshalAs(UnmanagedType.LPWStr)] string script);

		[DllImport("./bin/Fluxteam_net_API.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool is_injected(IntPtr proc, int pid, string path);

		public static API.Result r_inject(string dll_path)
		{
			FileInfo fileInfo = new FileInfo(dll_path);
			FileSecurity accessControl = fileInfo.GetAccessControl();
			SecurityIdentifier identity = new SecurityIdentifier("S-1-15-2-1");
			accessControl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
			fileInfo.SetAccessControl(accessControl);
			Process[] processesByName = Process.GetProcessesByName("Windows10Universal");
			if (processesByName.Length == 0)
			{
				return API.Result.ProcNotOpen;
			}
			uint num = 0U;
			while ((ulong)num < (ulong)((long)processesByName.Length))
			{
				Process process = processesByName[(int)num];
				if (API.pid != process.Id)
				{
					IntPtr intPtr = API.OpenProcess(1082U, false, process.Id);
					if (intPtr == API.NULL)
					{
						return API.Result.OpenProcFail;
					}
					IntPtr intPtr2 = API.VirtualAllocEx(intPtr, API.NULL, (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 12288U, 64U);
					if (intPtr2 == API.NULL)
					{
						return API.Result.AllocFail;
					}
					byte[] bytes = Encoding.Default.GetBytes(dll_path);
					int num2 = API.WriteProcessMemory(intPtr, intPtr2, bytes, (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 0);
					if (num2 == 0 || (long)num2 == 6L)
					{
						return API.Result.Unknown;
					}
					if (API.CreateRemoteThread(intPtr, API.NULL, API.NULL, API.GetProcAddress(API.GetModuleHandle("kernel32.dll"), "LoadLibraryA"), intPtr2, 0U, API.NULL) == API.NULL)
					{
						return API.Result.LoadLibFail;
					}
					API.pid = process.Id;
					API.phandle = intPtr;
					return API.Result.Success;
				}
				else
				{
					if (API.pid == process.Id)
					{
						return API.Result.AlreadyInjected;
					}
					num += 1U;
				}
			}
			return API.Result.Unknown;
		}

		public static bool is_injected(string dll_path)
		{
			return API.is_injected(API.phandle, API.pid, dll_path);
		}

		public static bool run_script(string dll_path, string script)
		{
			if (API.pid == 0)
			{
				MessageBox.Show("The specified DLL is not attached to the target process.", "Furk Ultra - Injection Error");
				return false;
			}
			if (script == string.Empty)
			{
				return API.is_injected(dll_path);
			}
			return API.run_script(API.phandle, API.pid, dll_path, script);
		}

		public static void create_files()
		{
			string text = "";
			foreach (string text2 in Directory.GetDirectories(Environment.GetEnvironmentVariable("LocalAppData") + "\\Packages"))
			{
				if (text2.Contains("OBLOXCORPORATION"))
				{
					if (Directory.GetDirectories(text2 + "\\AC").Any((string dir) => dir.Contains("Temp")))
					{
						text = text2 + "\\AC";
					}
				}
			}
			if (text == "")
			{
				return;
			}
			try
			{
				if (Directory.Exists("workspace"))
				{
					Directory.Move("workspace", "old_workspace");
				}
				if (Directory.Exists("autoexec"))
				{
					Directory.Move("autoexec", "old_autoexec");
				}
			}
			catch
			{
			}
			string path = Path.Combine(text, "workspace");
			string path2 = Path.Combine(text, "autoexec");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
			}
		}

		static IntPtr phandle;

		static int pid = 0;

		static readonly IntPtr NULL = (IntPtr)0;

		public enum Result : uint
		{
			Success,
			DLLNotFound,
			OpenProcFail,
			AllocFail,
			LoadLibFail,
			AlreadyInjected,
			ProcNotOpen,
			Unknown
		}
	}
}
