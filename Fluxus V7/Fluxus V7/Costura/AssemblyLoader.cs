using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Costura
{
	[CompilerGenerated]
	static class AssemblyLoader
	{
		static string CultureToString(CultureInfo culture)
		{
			if (culture == null)
			{
				return "";
			}
			return culture.Name;
		}

		static Assembly ReadExistingAssembly(AssemblyName name)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			Assembly[] assemblies = currentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				AssemblyName name2 = assembly.GetName();
				if (string.Equals(name2.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(AssemblyLoader.CultureToString(name2.CultureInfo), AssemblyLoader.CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
				{
					return assembly;
				}
			}
			return null;
		}

		static void CopyTo(Stream source, Stream destination)
		{
			byte[] array = new byte[81920];
			int count;
			while ((count = source.Read(array, 0, array.Length)) != 0)
			{
				destination.Write(array, 0, count);
			}
		}

		static Stream LoadStream(string fullName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if (fullName.EndsWith(".compressed"))
			{
				using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(fullName))
				{
					using (DeflateStream deflateStream = new DeflateStream(manifestResourceStream, CompressionMode.Decompress))
					{
						MemoryStream memoryStream = new MemoryStream();
						AssemblyLoader.CopyTo(deflateStream, memoryStream);
						memoryStream.Position = 0L;
						return memoryStream;
					}
				}
			}
			return executingAssembly.GetManifestResourceStream(fullName);
		}

		static Stream LoadStream(Dictionary<string, string> resourceNames, string name)
		{
			string fullName;
			if (resourceNames.TryGetValue(name, out fullName))
			{
				return AssemblyLoader.LoadStream(fullName);
			}
			return null;
		}

		static byte[] ReadStream(Stream stream)
		{
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			return array;
		}

		static Assembly ReadFromEmbeddedResources(Dictionary<string, string> assemblyNames, Dictionary<string, string> symbolNames, AssemblyName requestedAssemblyName)
		{
			string text = requestedAssemblyName.Name.ToLowerInvariant();
			if (requestedAssemblyName.CultureInfo != null && !string.IsNullOrEmpty(requestedAssemblyName.CultureInfo.Name))
			{
				text = requestedAssemblyName.CultureInfo.Name + "." + text;
			}
			byte[] rawAssembly;
			using (Stream stream = AssemblyLoader.LoadStream(assemblyNames, text))
			{
				if (stream == null)
				{
					return null;
				}
				rawAssembly = AssemblyLoader.ReadStream(stream);
			}
			using (Stream stream2 = AssemblyLoader.LoadStream(symbolNames, text))
			{
				if (stream2 != null)
				{
					byte[] rawSymbolStore = AssemblyLoader.ReadStream(stream2);
					return Assembly.Load(rawAssembly, rawSymbolStore);
				}
			}
			return Assembly.Load(rawAssembly);
		}

		public static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
		{
			object obj = AssemblyLoader.nullCacheLock;
			lock (obj)
			{
				if (AssemblyLoader.nullCache.ContainsKey(e.Name))
				{
					return null;
				}
			}
			AssemblyName assemblyName = new AssemblyName(e.Name);
			Assembly assembly = AssemblyLoader.ReadExistingAssembly(assemblyName);
			if (assembly != null)
			{
				return assembly;
			}
			assembly = AssemblyLoader.ReadFromEmbeddedResources(AssemblyLoader.assemblyNames, AssemblyLoader.symbolNames, assemblyName);
			if (assembly == null)
			{
				object obj2 = AssemblyLoader.nullCacheLock;
				lock (obj2)
				{
					AssemblyLoader.nullCache[e.Name] = true;
				}
				if ((assemblyName.Flags & AssemblyNameFlags.Retargetable) != AssemblyNameFlags.None)
				{
					assembly = Assembly.Load(assemblyName);
				}
			}
			return assembly;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AssemblyLoader()
		{
			AssemblyLoader.assemblyNames.Add("cake.powershell", "costura.cake.powershell.dll.compressed");
			AssemblyLoader.symbolNames.Add("cake.powershell", "costura.cake.powershell.pdb.compressed");
			AssemblyLoader.assemblyNames.Add("costura", "costura.costura.dll.compressed");
			AssemblyLoader.symbolNames.Add("costura", "costura.costura.pdb.compressed");
			AssemblyLoader.assemblyNames.Add("microsoft.win32.primitives", "costura.microsoft.win32.primitives.dll.compressed");
			AssemblyLoader.assemblyNames.Add("microsoft.windowsapicodepack", "costura.microsoft.windowsapicodepack.dll.compressed");
			AssemblyLoader.assemblyNames.Add("microsoft.windowsapicodepack.shell", "costura.microsoft.windowsapicodepack.shell.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.appcontext", "costura.system.appcontext.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.buffers", "costura.system.buffers.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.console", "costura.system.console.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.diagnostics.diagnosticsource", "costura.system.diagnostics.diagnosticsource.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.diagnostics.tracing", "costura.system.diagnostics.tracing.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.globalization.calendars", "costura.system.globalization.calendars.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.io.compression", "costura.system.io.compression.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.io.compression.zipfile", "costura.system.io.compression.zipfile.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.io", "costura.system.io.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.io.filesystem", "costura.system.io.filesystem.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.io.filesystem.primitives", "costura.system.io.filesystem.primitives.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.linq", "costura.system.linq.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.linq.expressions", "costura.system.linq.expressions.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.management.automation", "costura.system.management.automation.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.memory", "costura.system.memory.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.net.http", "costura.system.net.http.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.net.sockets", "costura.system.net.sockets.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.numerics.vectors", "costura.system.numerics.vectors.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.reflection", "costura.system.reflection.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.runtime.compilerservices.unsafe", "costura.system.runtime.compilerservices.unsafe.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.runtime", "costura.system.runtime.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.runtime.extensions", "costura.system.runtime.extensions.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.runtime.interopservices", "costura.system.runtime.interopservices.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.runtime.interopservices.runtimeinformation", "costura.system.runtime.interopservices.runtimeinformation.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.security.cryptography.algorithms", "costura.system.security.cryptography.algorithms.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.security.cryptography.encoding", "costura.system.security.cryptography.encoding.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.security.cryptography.primitives", "costura.system.security.cryptography.primitives.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.security.cryptography.x509certificates", "costura.system.security.cryptography.x509certificates.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.text.regularexpressions", "costura.system.text.regularexpressions.dll.compressed");
			AssemblyLoader.assemblyNames.Add("system.xml.readerwriter", "costura.system.xml.readerwriter.dll.compressed");
		}

		public static void Attach()
		{
			if (Interlocked.Exchange(ref AssemblyLoader.isAttached, 1) == 1)
			{
				return;
			}
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += AssemblyLoader.ResolveAssembly;
		}

		static object nullCacheLock = new object();

		static Dictionary<string, bool> nullCache = new Dictionary<string, bool>();

		static Dictionary<string, string> assemblyNames = new Dictionary<string, string>();

		static Dictionary<string, string> symbolNames = new Dictionary<string, string>();

		static int isAttached;
	}
}
