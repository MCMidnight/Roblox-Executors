using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IWshRuntimeLibrary
{
	[CompilerGenerated, DefaultMember("FullName"), Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B"), TypeIdentifier, ComImport]
	public interface IWshShortcut
	{
		[DispId(0), IndexerName("FullName")]
		string FullName { [DispId(0), MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

		void _VtblGap1_9();

		[DispId(1005)]
		string TargetPath { [DispId(1005), MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1005), MethodImpl(MethodImplOptions.InternalCall)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

		void _VtblGap2_5();

		[DispId(2001), MethodImpl(MethodImplOptions.InternalCall)]
		void Save();
	}
}
