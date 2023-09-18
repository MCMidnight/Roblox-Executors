using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using IWshRuntimeLibrary;
using Microsoft.CSharp.RuntimeBinder;

namespace Delta
{
	public class oxygenu_api
	{
		[DllImport("kernel32.dll")]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		public static void ShowConsole()
		{
			oxygenu_api.ShowWindow(oxygenu_api.GetConsoleWindow(), 5);
		}

		public static void HideConsole()
		{
			IntPtr consoleWindow = oxygenu_api.GetConsoleWindow();
			if (consoleWindow != IntPtr.Zero)
			{
				oxygenu_api.ShowWindow(consoleWindow, 0);
			}
		}

		void ipc_onchange(object source, FileSystemEventArgs e)
		{
			try
			{
				if (oxygenu_api.GetConsoleWindow() == IntPtr.Zero)
				{
					oxygenu_api.AllocConsole();
				}
				oxygenu_api.HideConsole();
				if (!this.file_debounce[e.Name])
				{
					this.file_debounce[e.Name] = true;
				}
				else
				{
					this.file_debounce[e.Name] = false;
					string data = File.ReadAllText(e.FullPath);
					string name = e.Name;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num <= 2714656340U)
					{
						if (num <= 1344450834U)
						{
							if (num != 554485536U)
							{
								if (num != 741618301U)
								{
									if (num == 1344450834U)
									{
										if (name == "i.txt")
										{
											oxygenu_api.HideConsole();
										}
									}
								}
								else if (name == "clip.txt")
								{
									Thread thread = new Thread(delegate()
									{
										Clipboard.SetText(data);
									});
									thread.SetApartmentState(ApartmentState.STA);
									thread.Start();
									thread.Join();
								}
							}
							else if (name == "g.txt")
							{
								Console.Title = data;
							}
						}
						else if (num != 1592897548U)
						{
							if (num != 2117095829U)
							{
								if (num == 2714656340U)
								{
									if (name == "c.txt")
									{
										oxygenu_api.ShowConsole();
										Console.Write("[ERROR]: " + data);
									}
								}
							}
							else if (name == "d.txt")
							{
								oxygenu_api.ShowConsole();
								Console.Write("[INFORMATION]: " + data);
							}
						}
						else if (name == "k.txt")
						{
							oxygenu_api.HideConsole();
							Console.Title = "Delta - Console";
						}
					}
					else if (num <= 3416295761U)
					{
						if (num != 2780947583U)
						{
							if (num != 2956830970U)
							{
								if (num == 3416295761U)
								{
									if (name == "h.txt")
									{
										oxygenu_api.ShowConsole();
									}
								}
							}
							else if (name == "dbg.txt")
							{
								oxygenu_api.ShowConsole();
								Console.WriteLine(data);
							}
						}
						else if (!(name == "f.txt"))
						{
						}
					}
					else if (num <= 3802720310U)
					{
						if (num != 3667405499U)
						{
							if (num == 3802720310U)
							{
								if (name == "e.txt")
								{
									oxygenu_api.ShowConsole();
									Console.Clear();
								}
							}
						}
						else if (name == "j.txt")
						{
							oxygenu_api.HideConsole();
							Console.Title = "Delta - Console";
						}
					}
					else if (num != 3820973562U)
					{
						if (num == 3833732323U)
						{
							if (name == "b.txt")
							{
								oxygenu_api.ShowConsole();
								Console.Write("[WARNING]: " + data);
							}
						}
					}
					else if (name == "a.txt")
					{
						oxygenu_api.ShowConsole();
						Console.Write(data);
					}
				}
			}
			catch
			{
			}
		}

		public void setup_ipc()
		{
			if (this.rbxpath != null)
			{
				FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
				fileSystemWatcher.Path = Path.Combine(this.rbxpath, "ipc");
				fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
				fileSystemWatcher.Filter = "*.*";
				fileSystemWatcher.Changed += this.ipc_onchange;
				fileSystemWatcher.EnableRaisingEvents = true;
			}
		}

		public void create_links()
		{
			foreach (string text in Directory.GetDirectories(Environment.GetEnvironmentVariable("LocalAppData") + "\\Packages"))
			{
				if (text.Contains("OBLOXCORPORATION"))
				{
					if (Directory.GetDirectories(text + "\\AC").Any((string dir) => dir.Contains("Temp")))
					{
						this.rbxpath = text + "\\AC";
					}
				}
			}
			if (this.rbxpath == null)
			{
				Console.WriteLine("Could not find Roblox path.");
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
			string text2 = Path.Combine(this.rbxpath, "workspace");
			string text3 = Path.Combine(this.rbxpath, "autoexec");
			string path = Path.Combine(this.rbxpath, "ipc");
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			if (!Directory.Exists(text3))
			{
				Directory.CreateDirectory(text3);
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (!File.Exists("workspace.lnk"))
			{
				WshShell wshShell = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
				if (oxygenu_api.<>o__12.<>p__0 == null)
				{
					oxygenu_api.<>o__12.<>p__0 = CallSite<Func<CallSite, object, IWshShortcut>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IWshShortcut), typeof(oxygenu_api)));
				}
				IWshShortcut wshShortcut = oxygenu_api.<>o__12.<>p__0.Target(oxygenu_api.<>o__12.<>p__0, wshShell.CreateShortcut("workspace.lnk"));
				wshShortcut.TargetPath = text2;
				wshShortcut.Save();
			}
			if (!File.Exists("autoexec.lnk"))
			{
				WshShell wshShell2 = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
				if (oxygenu_api.<>o__12.<>p__1 == null)
				{
					oxygenu_api.<>o__12.<>p__1 = CallSite<Func<CallSite, object, IWshShortcut>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IWshShortcut), typeof(oxygenu_api)));
				}
				IWshShortcut wshShortcut2 = oxygenu_api.<>o__12.<>p__1.Target(oxygenu_api.<>o__12.<>p__1, wshShell2.CreateShortcut("autoexec.lnk"));
				wshShortcut2.TargetPath = text3;
				wshShortcut2.Save();
			}
		}

		public string rbxpath;

		public Dictionary<string, bool> file_debounce = new Dictionary<string, bool>
		{
			{
				"clip.txt",
				false
			},
			{
				"a.txt",
				false
			},
			{
				"b.txt",
				false
			},
			{
				"c.txt",
				false
			},
			{
				"d.txt",
				false
			},
			{
				"e.txt",
				false
			},
			{
				"f.txt",
				false
			},
			{
				"g.txt",
				false
			},
			{
				"h.txt",
				false
			},
			{
				"i.txt",
				false
			},
			{
				"j.txt",
				false
			},
			{
				"k.txt",
				false
			},
			{
				"dbg.txt",
				false
			}
		};

		const int SW_SHOW = 5;

		const int SW_HIDE = 0;
	}
}
