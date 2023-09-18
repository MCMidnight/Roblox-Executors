using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.CSharp.RuntimeBinder;
using Vega_X.Classes;

namespace VegaX.Classes.DLL
{
	static class FluxInteractionsAPI
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

		[DllImport("bin/FluxInteractions.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool run_script(IntPtr proc, int pid, string path, [MarshalAs(UnmanagedType.LPWStr)] string script);

		[DllImport("bin/FluxInteractions.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool is_injected(IntPtr proc, int pid, string path);

		static FluxInteractionsAPI.Result r_inject(string dll_path)
		{
			FileInfo fileInfo = new FileInfo(dll_path);
			FileSecurity accessControl = fileInfo.GetAccessControl();
			SecurityIdentifier identity = new SecurityIdentifier("S-1-15-2-1");
			accessControl.AddAccessRule(new FileSystemAccessRule(identity, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
			fileInfo.SetAccessControl(accessControl);
			Process[] processesByName = Process.GetProcessesByName("Windows10Universal");
			if (processesByName.Length == 0)
			{
				return FluxInteractionsAPI.Result.ProcNotOpen;
			}
			uint num = 0U;
			while ((ulong)num < (ulong)((long)processesByName.Length))
			{
				Process process = processesByName[(int)num];
				if (FluxInteractionsAPI.pid != process.Id)
				{
					IntPtr intPtr = FluxInteractionsAPI.OpenProcess(1082U, false, process.Id);
					if (intPtr == FluxInteractionsAPI.NULL)
					{
						return FluxInteractionsAPI.Result.OpenProcFail;
					}
					IntPtr intPtr2 = FluxInteractionsAPI.VirtualAllocEx(intPtr, FluxInteractionsAPI.NULL, (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 12288U, 64U);
					if (intPtr2 == FluxInteractionsAPI.NULL)
					{
						return FluxInteractionsAPI.Result.AllocFail;
					}
					byte[] bytes = Encoding.Default.GetBytes(dll_path);
					int num2 = FluxInteractionsAPI.WriteProcessMemory(intPtr, intPtr2, bytes, (IntPtr)((dll_path.Length + 1) * Marshal.SizeOf(typeof(char))), 0);
					if (num2 == 0 || (long)num2 == 6L)
					{
						return FluxInteractionsAPI.Result.Unknown;
					}
					if (FluxInteractionsAPI.CreateRemoteThread(intPtr, FluxInteractionsAPI.NULL, FluxInteractionsAPI.NULL, FluxInteractionsAPI.GetProcAddress(FluxInteractionsAPI.GetModuleHandle("kernel32.dll"), "LoadLibraryA"), intPtr2, 0U, FluxInteractionsAPI.NULL) == FluxInteractionsAPI.NULL)
					{
						return FluxInteractionsAPI.Result.LoadLibFail;
					}
					FluxInteractionsAPI.pid = process.Id;
					FluxInteractionsAPI.phandle = intPtr;
					return FluxInteractionsAPI.Result.Success;
				}
				else
				{
					if (FluxInteractionsAPI.pid == process.Id)
					{
						return FluxInteractionsAPI.Result.AlreadyInjected;
					}
					num += 1U;
				}
			}
			return FluxInteractionsAPI.Result.Unknown;
		}

		public static FluxInteractionsAPI.Result inject_custom()
		{
			FluxInteractionsAPI.Result result;
			try
			{
				if (!File.Exists(FluxInteractionsAPI.dll_path))
				{
					result = FluxInteractionsAPI.Result.DLLNotFound;
				}
				else
				{
					result = FluxInteractionsAPI.r_inject(FluxInteractionsAPI.dll_path);
				}
			}
			catch
			{
				result = FluxInteractionsAPI.Result.Unknown;
			}
			return result;
		}

		public static void inject()
		{
			if (FluxInteractionsAPI.GetCorrectDLL())
			{
				switch (FluxInteractionsAPI.inject_custom())
				{
				case FluxInteractionsAPI.Result.DLLNotFound:
					System.Windows.Forms.MessageBox.Show("Injection Failed! DLL not found!\n", "Injection");
					return;
				case FluxInteractionsAPI.Result.OpenProcFail:
					System.Windows.Forms.MessageBox.Show("Injection Failed - OpenProcFail failed!\n", "Injection");
					return;
				case FluxInteractionsAPI.Result.AllocFail:
					System.Windows.Forms.MessageBox.Show("Injection Failed - AllocFail failed!\n", "Injection");
					return;
				case FluxInteractionsAPI.Result.LoadLibFail:
					System.Windows.Forms.MessageBox.Show("Injection Failed - LoadLibFail failed!\n", "Injection");
					return;
				case FluxInteractionsAPI.Result.AlreadyInjected:
					break;
				case FluxInteractionsAPI.Result.ProcNotOpen:
					System.Windows.Forms.MessageBox.Show("Failure to find UWP game!\n\nPlease make sure you are using the game from the Microsoft Store and not the browser!", "Injection");
					return;
				case FluxInteractionsAPI.Result.Unknown:
					System.Windows.Forms.MessageBox.Show("Injection Failed - Unknown!\n", "Injection");
					break;
				default:
					return;
				}
			}
		}

		public static bool is_injected()
		{
			return FluxInteractionsAPI.is_injected(FluxInteractionsAPI.phandle, FluxInteractionsAPI.pid, FluxInteractionsAPI.dll_path);
		}

		public static bool run_script(string script)
		{
			if (script == string.Empty)
			{
				return FluxInteractionsAPI.is_injected();
			}
			return FluxInteractionsAPI.run_script(FluxInteractionsAPI.phandle, FluxInteractionsAPI.pid, FluxInteractionsAPI.dll_path, script);
		}

		public static void create_files(string dll_path_)
		{
			FluxInteractionsAPI.dll_path = dll_path_;
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
			string text3 = Path.Combine(text, "workspace");
			string text4 = Path.Combine(text, "autoexec");
			if (!Directory.Exists(text3))
			{
				Directory.CreateDirectory(text3);
			}
			if (!Directory.Exists(text4))
			{
				Directory.CreateDirectory(text4);
			}
			if (!File.Exists("workspace.lnk"))
			{
				WshShell wshShell = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
				if (FluxInteractionsAPI.<>o__18.<>p__0 == null)
				{
					FluxInteractionsAPI.<>o__18.<>p__0 = CallSite<Func<CallSite, object, IWshShortcut>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IWshShortcut), typeof(FluxInteractionsAPI)));
				}
				IWshShortcut wshShortcut = FluxInteractionsAPI.<>o__18.<>p__0.Target(FluxInteractionsAPI.<>o__18.<>p__0, wshShell.CreateShortcut("workspace.lnk"));
				wshShortcut.TargetPath = text3;
				wshShortcut.Save();
			}
			if (!File.Exists("autoexec.lnk"))
			{
				WshShell wshShell2 = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
				if (FluxInteractionsAPI.<>o__18.<>p__1 == null)
				{
					FluxInteractionsAPI.<>o__18.<>p__1 = CallSite<Func<CallSite, object, IWshShortcut>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IWshShortcut), typeof(FluxInteractionsAPI)));
				}
				IWshShortcut wshShortcut2 = FluxInteractionsAPI.<>o__18.<>p__1.Target(FluxInteractionsAPI.<>o__18.<>p__1, wshShell2.CreateShortcut("autoexec.lnk"));
				wshShortcut2.TargetPath = text4;
				wshShortcut2.Save();
			}
		}

		static string get_file_sha384(string path)
		{
			string result;
			using (FileStream fileStream = File.OpenRead(path))
			{
				result = BitConverter.ToString(new SHA384Managed().ComputeHash(fileStream)).Replace("-", string.Empty).ToLower();
			}
			return result;
		}

		static bool GetCorrectDLL()
		{
			Process[] processesByName = Process.GetProcessesByName("Windows10Universal");
			if (processesByName.Length == 0)
			{
				System.Windows.MessageBox.Show("The game was not found!\nDue to the release of Byfron, Vega X currently only supports the Windows Store version of the game!\nPlease install and join using the Windows Store version and inject again!", "Vega X");
				HandleSettings.SaveBool("Currently_Installing_DLL", false);
				return false;
			}
			if (processesByName.Length != 0)
			{
				string text = FluxInteractionsAPI.get_file_sha384(Process.GetProcessById(processesByName[0].Id).MainModule.FileName);
				if (HandleSettings.ReadSString("game_hash_uwp") != text || !File.Exists(DLLFileSystem.DLLPath))
				{
					HandleSettings.SaveBool("Currently_Installing_DLL", true);
					if (File.Exists(DLLFileSystem.DLLPath))
					{
						File.Delete(DLLFileSystem.DLLPath);
					}
					using (WebClient webClient = new WebClient())
					{
						string text2 = webClient.DownloadString("https://vegax.gg/windows/get_dll_hash.php?hash=" + text);
						if (text2 == "no")
						{
							System.Windows.MessageBox.Show("You are running a version of the game Vega X is not updated for yet!\nPlease wait a few hours for an update and try again!", "Vega X", MessageBoxButton.OK, MessageBoxImage.Hand);
							HandleSettings.SaveBool("Currently_Installing_DLL", false);
							return false;
						}
						Uri address = new Uri(text2);
						try
						{
							webClient.DownloadFile(address, DLLFileSystem.DLLPath);
						}
						catch (Exception ex)
						{
							System.Windows.MessageBox.Show("DLL Installation System Failed!\n" + ex.ToString(), "Vega X");
							HandleSettings.SaveBool("Currently_Installing_DLL", false);
						}
					}
					HandleSettings.SaveBool("Currently_Installing_DLL", false);
					HandleSettings.SaveString("game_hash_uwp", text);
					return true;
				}
			}
			HandleSettings.SaveBool("Currently_Installing_DLL", false);
			return true;
		}

		public static string dll_path;

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
