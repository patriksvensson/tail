using System;
using System.IO;
using System.Threading;
using Tail.Extensibility;

namespace Tail.Providers
{
	internal sealed class FileStreamListener : TailStreamListener<FileStreamContext>
	{
		public override void Listen(FileStreamContext context, ITailCallback callback, WaitHandle abortSignal)
		{
			// Wait for a file to be loaded.
			var file = new FileInfo(context.Path);
			if (!file.Exists)
			{
				return;
			}

			using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
			{
				long lastOffset = reader.BaseStream.Length;
				if (reader.BaseStream.Length > 0)
				{
					// Send the last 10 kb of text to the reader.
					lastOffset = Math.Max(0, reader.BaseStream.Length - (1024 * 10));
				}

				while (!abortSignal.WaitOne(100))
				{
					// Idle if file hasn't changed.
					if (reader.BaseStream.Length <= lastOffset)
					{
						if (reader.BaseStream.Length < lastOffset)
						{
							lastOffset = reader.BaseStream.Length;
						}
						continue;
					}

					// Read the data.
					reader.BaseStream.Seek(lastOffset, SeekOrigin.Begin);
					var delta = reader.BaseStream.Length - lastOffset;
					var buffer = new char[delta];
					reader.ReadBlock(buffer, 0, buffer.Length);

					// Publish the data.
					callback.Publish(new string(buffer));

					// Update the offset.
					lastOffset = reader.BaseStream.Position;
				}
			}
		}
	}
}
