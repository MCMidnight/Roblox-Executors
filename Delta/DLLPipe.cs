using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace Delta
{
	class DLLPipe
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool WaitNamedPipe(string pipe, int timeout = 10);

		public static bool NamedPipeExist()
		{
			return DLLPipe.WaitNamedPipe("\\\\.\\pipe\\" + DLLPipe.luapipename, 10);
		}

		public static void LuaPipe(string script)
		{
			if (DLLPipe.NamedPipeExist())
			{
				new Thread(delegate()
				{
					try
					{
						using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", DLLPipe.luapipename, PipeDirection.Out))
						{
							namedPipeClientStream.Connect();
							using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream, Encoding.Default, 999999))
							{
								streamWriter.Write(script);
								streamWriter.Dispose();
							}
							namedPipeClientStream.Dispose();
						}
					}
					catch (IOException)
					{
						MessageBox.Show("Can't Connect to Pipe, Connection Errot (Restart PC Or Kill Roblox)", "Delta");
					}
					catch (Exception)
					{
						MessageBox.Show("Unknown error", "Delta");
					}
				}).Start();
				return;
			}
			MessageBox.Show("Inject Before Executing", "Delta");
		}

		public static string luapipename = "DeltaWins";
	}
}
