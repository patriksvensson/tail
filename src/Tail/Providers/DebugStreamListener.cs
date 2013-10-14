using BlackBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Tail.Extensibility;
using Tail.Providers.Utilities;

namespace Tail.Providers
{
	internal sealed class DebugStreamListener : TailStreamListener<DebugStreamContext>, IDisposable
	{
		private readonly ILogger _logger;
		private readonly object _lock;
		private IntPtr _bufferReadyEvent = IntPtr.Zero;
		private IntPtr _readyEvent = IntPtr.Zero;
		private IntPtr _sharedFile = IntPtr.Zero;
		private IntPtr _sharedMemory = IntPtr.Zero;
		private string _scope;
		private bool _hasBeenShutdown;

		public DebugStreamListener(ILogger logger)
		{
			_logger = logger;
			_lock = new object();
		}

		~DebugStreamListener()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.Shutdown();
		}

		public override void Initialize(DebugStreamContext context)
		{
			_scope = context.GlobalScope ? @"Global\" : @"Local\";

			_logger.Information("Initializing debug listener...");

			// Initialize the security descriptor.
			_logger.Verbose("Creating new security descriptor...");
			SecurityDescriptor sd = new SecurityDescriptor();
			if (!Win32Native.InitializeSecurityDescriptor(ref sd, Win32Native.SECURITY_DESCRIPTOR_REVISION))
			{
				_logger.Error("Failed to initializes the security descriptor.");
				throw new InvalidOperationException("Failed to initializes the security descriptor.");
			}
			// Set information in a discretionary access control list
			_logger.Verbose("Creating new security descriptor dacl...");
			if (!Win32Native.SetSecurityDescriptorDacl(ref sd, true, IntPtr.Zero, false))
			{
				_logger.Error("Failed to initializes the security descriptor.");
				throw new InvalidOperationException("Failed to initializes the security descriptor.");
			}

			_logger.Verbose("Creating security attributes...");
			SecurityAttributes sa = new SecurityAttributes();
			sa.nLength = Marshal.SizeOf(sa);
			sa.lpSecurityDescriptor = Marshal.AllocHGlobal(Marshal.SizeOf(sd));
			Marshal.StructureToPtr(sd, sa.lpSecurityDescriptor, false);

			// Create the event for slot 'DBWIN_BUFFER_READY'
			_logger.Verbose("Creating event DBWIN_BUFFER_READY...");
			_bufferReadyEvent = Win32Native.CreateEvent(ref sa, false, false, _scope + "DBWIN_BUFFER_READY");
			if (_bufferReadyEvent == IntPtr.Zero)
			{
				_logger.Error("Failed to create event DBWIN_BUFFER_READY.");
				throw new InvalidOperationException("Failed to create event 'DBWIN_BUFFER_READY'");
			}

			// Create the event for slot 'DBWIN_DATA_READY'
			_logger.Verbose("Creating event DBWIN_DATA_READY...");
			_readyEvent = Win32Native.CreateEvent(ref sa, false, false, _scope + "DBWIN_DATA_READY");
			if (_readyEvent == IntPtr.Zero)
			{
				_logger.Error("Failed to create event DBWIN_DATA_READY.");
				throw new InvalidOperationException("Failed to create event 'DBWIN_DATA_READY'");
			}

			// Get a handle to the readable shared memory at slot 'DBWIN_BUFFER'.
			_logger.Verbose("Creating file mapping to slot DBWIN_BUFFER...");
			_sharedFile = Win32Native.CreateFileMapping(new IntPtr(-1), ref sa, PageProtection.ReadWrite, 0, 4096, _scope + "DBWIN_BUFFER");
			if (_sharedFile == IntPtr.Zero)
			{
				_logger.Error("Failed to create a file mapping to slot DBWIN_BUFFER.");
				throw new InvalidOperationException("Failed to create a file mapping to slot 'DBWIN_BUFFER'");
			}

			// Create a view for this file mapping so we can access it
			_logger.Verbose("Creating mapping view to slot DBWIN_BUFFER...");
			_sharedMemory = Win32Native.MapViewOfFile(_sharedFile, Win32Native.SECTION_MAP_READ, 0, 0, 512);
			if (_sharedMemory == IntPtr.Zero)
			{
				_logger.Error("Failed to create a mapping view to slot DBWIN_BUFFER.");
				throw new InvalidOperationException("Failed to create a mapping view for slot 'DBWIN_BUFFER'");
			}

			_logger.Information("Debug listener has been initialized.");
		}

		public override void Listen(DebugStreamContext context, ITailCallback callback, WaitHandle abortSignal)
		{
			// Everything after the first DWORD is our debugging text.
			bool isWow64 = Win32Native.IsWow64Process();
			IntPtr pString = isWow64
				? new IntPtr(_sharedMemory.ToInt32() + Marshal.SizeOf(typeof(int)))
				: new IntPtr(_sharedMemory.ToInt64() + Marshal.SizeOf(typeof(int)));

			_logger.Information(isWow64 ? "Running in WOW64." : "Not running in WOW64.");

			while (!abortSignal.WaitOne(0))
			{
				// We're ready to receive new buffer data.
				Win32Native.SetEvent(_bufferReadyEvent);

				int ret = Win32Native.WaitForSingleObject(_readyEvent, 500);
				if (ret == Win32Native.WAIT_OBJECT_0)
				{
					// Get the process ID and the message.
					var pid = Marshal.ReadInt32(_sharedMemory);
					var message = Marshal.PtrToStringAnsi(pString);

					// Publish the message.
					callback.Publish(message);
				}
			}
		}

		public override void Shutdown()
		{
			lock (_lock)
			{
				if (_hasBeenShutdown)
				{
					return;
				}

				_logger.Information("Shutting down debug listener...");

				if (_bufferReadyEvent != IntPtr.Zero)
				{
					_logger.Verbose("Closing handle to event DBWIN_BUFFER_READY...");
					Win32Native.CloseHandle(_bufferReadyEvent);
					_bufferReadyEvent = IntPtr.Zero;
				}
				if (_readyEvent != IntPtr.Zero)
				{
					_logger.Verbose("Closing handle to event DBWIN_DATA_READY...");
					Win32Native.CloseHandle(_readyEvent);
					_readyEvent = IntPtr.Zero;
				}
				if (_sharedFile != IntPtr.Zero)
				{
					_logger.Verbose("Closing handle to file mapping DBWIN_BUFFER...");
					Win32Native.CloseHandle(_sharedFile);
					_sharedFile = IntPtr.Zero;
				}
				if (_sharedMemory != IntPtr.Zero)
				{
					_logger.Verbose("Closing handle to mapping view DBWIN_BUFFER...");
					Win32Native.UnmapViewOfFile(_sharedMemory);
					_sharedMemory = IntPtr.Zero;
				}

				_logger.Information("Debug listener has been shut down.");
				_hasBeenShutdown = true;
			}
		}
	}
}
