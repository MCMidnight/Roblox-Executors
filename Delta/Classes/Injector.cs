using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Delta.Classes
{
	public class Injector
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr OpenProcess(uint access, bool inhert_handle, int pid);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool WaitNamedPipe(string pipe, int timeout = 10);

		public bool Exists(string Name)
		{
			return Injector.WaitNamedPipe("\\\\.\\pipe\\" + Name, 10);
		}

		public bool is_ghost_proc(ProcessModuleCollection a1)
		{
			foreach (object obj in a1)
			{
				string text = ((ProcessModule)obj).FileName.ToString();
				if (text.Contains("cryptnet") || text.Contains("mswsock") || text.Contains("urlmon") || text.Contains("XInput1_4") || text.Contains("CoreUIComponents"))
				{
					return false;
				}
			}
			return true;
		}

		Injector.Result r_inject(string dll_path, bool ghostcheck)
		{
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			uint num = 0U;
			while ((ulong)num < (ulong)((long)processesByName.Length))
			{
				Process process = processesByName[(int)num];
				string name = "DeltaPipe";
				bool flag = ghostcheck;
				if (flag)
				{
					flag = !this.is_ghost_proc(process.Modules);
				}
				if (!this.Exists(name) && flag)
				{
					IntPtr intPtr = Injector.OpenProcess(1082U, false, process.Id);
					if (intPtr == Injector.NULL)
					{
						return Injector.Result.Unknown;
					}
					IntPtr procAddress = Injector.GetProcAddress(Injector.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
					if (procAddress == Injector.NULL)
					{
						return Injector.Result.Unknown;
					}
					IntPtr intPtr2 = Injector.VirtualAllocEx(intPtr, (IntPtr)null, (IntPtr)dll_path.Length, 12288U, 64U);
					if (intPtr2 == Injector.NULL)
					{
						return Injector.Result.Unknown;
					}
					byte[] bytes = Encoding.ASCII.GetBytes(dll_path);
					if (Injector.WriteProcessMemory(intPtr, intPtr2, bytes, bytes.Length, 0) == 0)
					{
						return Injector.Result.Unknown;
					}
					if (Injector.CreateRemoteThread(intPtr, (IntPtr)null, Injector.NULL, procAddress, intPtr2, 0U, (IntPtr)null) == Injector.NULL)
					{
						return Injector.Result.Unknown;
					}
					Injector.CloseHandle(intPtr);
					return Injector.Result.Success;
				}
				else
				{
					num += 1U;
				}
			}
			return Injector.Result.Success;
		}

		public Injector.Result inject(string path, bool ghostcheck)
		{
			try
			{
				if (!File.Exists(path))
				{
					return Injector.Result.DLLNotFound;
				}
				return this.r_inject(path, ghostcheck);
			}
			catch (Exception ex)
			{
				if (ex.ToString().Contains("(0x80004005)"))
				{
					LinkOpener.openlink("https://delta-documentation.gitbook.io/delta-error-fixes/error-fixes/access-is-denied");
				}
				MessageBox.Show("Injection Error\n" + ex.ToString(), "Injection", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			return Injector.Result.Unknown;
		}

		static readonly IntPtr NULL = (IntPtr)0;

		public enum Result : uint
		{
			Success,
			DLLNotFound,
			OpenProcFail,
			AllocFail,
			LoadLibFail,
			RobloxNotOpen,
			Unknown
		}
	}
}
