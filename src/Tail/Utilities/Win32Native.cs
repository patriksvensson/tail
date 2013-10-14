using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32;

namespace Tail.Providers.Utilities
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct SecurityAttributes
	{
		public int nLength;
		public IntPtr lpSecurityDescriptor;
		public int bInheritHandle;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct SecurityDescriptor
	{
		public byte revision;
		public byte size;
		public short control;
		public IntPtr owner;
		public IntPtr group;
		public IntPtr sacl;
		public IntPtr dacl;
	}

	[Flags]
	internal enum PageProtection : uint
	{
		NoAccess = 0x01,
		Readonly = 0x02,
		ReadWrite = 0x04,
		WriteCopy = 0x08,
		Execute = 0x10,
		ExecuteRead = 0x20,
		ExecuteReadWrite = 0x40,
		ExecuteWriteCopy = 0x80,
		Guard = 0x100,
		NoCache = 0x200,
		WriteCombine = 0x400,
	}

	internal enum TokenInformationClass
	{
		TokenUser = 1,
		TokenGroups,
		TokenPrivileges,
		TokenOwner,
		TokenPrimaryGroup,
		TokenDefaultDacl,
		TokenSource,
		TokenType,
		TokenImpersonationLevel,
		TokenStatistics,
		TokenRestrictedSids,
		TokenSessionId,
		TokenGroupsAndPrivileges,
		TokenSessionReference,
		TokenSandBoxInert,
		TokenAuditPolicy,
		TokenOrigin,
		TokenElevationType,
		TokenLinkedToken,
		TokenElevation,
		TokenHasRestrictions,
		TokenAccessInformation,
		TokenVirtualizationAllowed,
		TokenVirtualizationEnabled,
		TokenIntegrityLevel,
		TokenUIAccess,
		TokenMandatoryPolicy,
		TokenLogonSid,
		MaxTokenInfoClass
	}

	internal enum TokenElevationType
	{
		TokenElevationTypeDefault = 1,
		TokenElevationTypeFull,
		TokenElevationTypeLimited
	}

	internal static class Win32Native
	{
		public const int WAIT_OBJECT_0 = 0;
		public const uint INFINITE = 0xFFFFFFFF;
		public const int ERROR_ALREADY_EXISTS = 183;
		public const uint SECURITY_DESCRIPTOR_REVISION = 1;
		public const uint SECTION_MAP_READ = 0x0004;
		private const uint STANDARD_RIGHTS_READ = 0x00020000;
		private const uint TOKEN_QUERY = 0x0008;
		private const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

		private const string uacRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
		private const string uacRegistryValue = "EnableLUA";

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint
			dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,
			uint dwNumberOfBytesToMap);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool InitializeSecurityDescriptor(ref SecurityDescriptor sd, uint dwRevision);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool SetSecurityDescriptorDacl(ref SecurityDescriptor sd, bool daclPresent, IntPtr dacl, bool daclDefaulted);

		[DllImport("kernel32.dll")]
		public static extern IntPtr CreateEvent(ref SecurityAttributes sa, bool bManualReset, bool bInitialState, string lpName);

		[DllImport("kernel32.dll")]
		public static extern bool PulseEvent(IntPtr hEvent);

		[DllImport("kernel32.dll")]
		public static extern bool SetEvent(IntPtr hEvent);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateFileMapping(IntPtr hFile,
			ref SecurityAttributes lpFileMappingAttributes, PageProtection flProtect, uint dwMaximumSizeHigh,
			uint dwMaximumSizeLow, string lpName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hHandle);

		[DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
		public static extern Int32 WaitForSingleObject(IntPtr handle, uint milliseconds);

		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool GetTokenInformation(IntPtr TokenHandle, TokenInformationClass TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

		public static bool IsUacEnabled()
		{
			RegistryKey uacKey = Registry.LocalMachine.OpenSubKey(uacRegistryKey, false);
			bool result = uacKey.GetValue(uacRegistryValue).Equals(1);
			return result;
		}

		public static bool IsProcessElevated()
		{
			if (!IsUacEnabled())
			{
				WindowsIdentity identity = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(identity);
				bool result = principal.IsInRole(WindowsBuiltInRole.Administrator);
				return result;
			}

			IntPtr tokenHandle;
			if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_READ, out tokenHandle))
			{
				return true;
			}

			TokenElevationType elevationResult = TokenElevationType.TokenElevationTypeDefault;

			int elevationResultSize = Marshal.SizeOf((int)elevationResult);
			uint returnedSize = 0;
			IntPtr elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);

			bool success = GetTokenInformation(tokenHandle, TokenInformationClass.TokenElevationType, elevationTypePtr, (uint)elevationResultSize, out returnedSize);
			if (success)
			{
				elevationResult = (TokenElevationType)Marshal.ReadInt32(elevationTypePtr);
				bool isProcessAdmin = elevationResult == TokenElevationType.TokenElevationTypeFull;
				return isProcessAdmin;
			}
			return true;
		}

		public static bool IsWow64Process()
		{
			bool retVal;
			IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
			return retVal;
		}
	}
}
