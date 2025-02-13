/***************************************************************************
 *                             FileOperations.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: FileOperations.cs 4 2006-06-15 04:28:39Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/


using System;
using System.IO;
#if !MONO
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
#endif

namespace Server {
	public static class FileOperations {
		public const int KB = 1024;
		public const int MB = 1024 * KB;

#if !MONO
		private const FileOptions NoBuffering = ( FileOptions ) 0x20000000;

		[DllImport( "Kernel32", CharSet = CharSet.Auto, SetLastError = true )]
		private static extern SafeFileHandle CreateFile( string lpFileName, int dwDesiredAccess, FileShare dwShareMode, IntPtr securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile );
#endif

		private static int bufferSize = 1 * MB;
		private static int concurrency = 1;

		private static bool unbuffered = true;

		public static int BufferSize {
			get => bufferSize;
            set => bufferSize = value;
        }

		public static int Concurrency {
			get => concurrency;
            set => concurrency = value;
        }

		public static bool Unbuffered {
			get => unbuffered;
            set => unbuffered = value;
        }

		public static bool AreSynchronous => ( concurrency < 1 );

        public static bool AreAsynchronous => ( concurrency > 0 );

        public static FileStream OpenSequentialStream( string path, FileMode mode, FileAccess access, FileShare share ) {
			FileOptions options = FileOptions.SequentialScan;

			if ( concurrency > 0 ) {
				options |= FileOptions.Asynchronous;
			}

#if MONO
			return new FileStream( path, mode, access, share, bufferSize, options );
#else
			if ( unbuffered ) {
				options |= NoBuffering;
			} else {
				return new FileStream( path, mode, access, share, bufferSize, options );
			}

			SafeFileHandle fileHandle = CreateFile( path, (int) access, share, IntPtr.Zero, mode, (int) options, IntPtr.Zero );

			if ( fileHandle.IsInvalid ) {
				throw new IOException();
			}

			return new UnbufferedFileStream( fileHandle, access, bufferSize, ( concurrency > 0 ) );
#endif
		}

#if !MONO
		private class UnbufferedFileStream : FileStream {
			private SafeFileHandle fileHandle;

			public UnbufferedFileStream( SafeFileHandle fileHandle, FileAccess access, int bufferSize, bool isAsync )
			 : base( fileHandle, access, bufferSize, isAsync ) {
				this.fileHandle = fileHandle;
			}

			public override void Write( byte[] array, int offset, int count ) {
				base.Write( array, offset, bufferSize );
			}

			public override IAsyncResult BeginWrite( byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject ) {
				return base.BeginWrite( array, offset, bufferSize, userCallback, stateObject );
			}

			protected override void Dispose( bool disposing ) {
				if ( !fileHandle.IsClosed ) {
					fileHandle.Close();
				}

				base.Dispose( disposing );
			}
		}
#endif
	}
}
