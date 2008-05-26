using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Parrano.Api
{
    /*
     * Substantial portions of this code were copied with only minor modifications
     * from the source code for Mono. The relevant file was Mono.Unix/StdioFileStream.cs 
     * by Jonathan Pryor (jonpryor@vt.edu). I don't know if i need to metnion this or not, 
     * but in case I do, here's the honourable mention. It made writing this *a lot* faster.
     * 
     * Thanks,
     * Troy Howard (thoward37@gmail.com)
     *
     */


    /// <summary>
    /// 
    /// </summary>
    public class NativeFileStream : Stream, IDisposable
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
        public NativeFileStream (IntPtr fileStream)
			: this (fileStream, true) {}

		public NativeFileStream (IntPtr fileStream, bool ownsHandle)
		{
		    InitStream (fileStream, ownsHandle);
		}

        public NativeFileStream (string path)
        {
            InitStream (Fopen (path, "rb"), true);
        }

        public NativeFileStream (string path, string mode)
        {
            InitStream (Fopen (path, mode), true);
        }

        public NativeFileStream (string path, Native.FOpenMode mode)
        {
            InitStream (Fopen (path, Native.GetFOpenModeDescription(mode)), true);
        }

        private bool canSeek;
        private bool canRead;
        private bool canWrite;
        private bool owner;
        private IntPtr file = IntPtr.Zero;
                
        public IntPtr FileHandle
        {
            get { return file; }
            set {  file = value; }
        }

        private void InitStream(IntPtr fileStream, bool ownsHandle)
        {
            if (fileStream == IntPtr.Zero)
                throw new ArgumentException("Invalid file stream", "fileStream");

            file = fileStream;
            owner = ownsHandle;

            try
            {
                long offset = Native.fseek(file, 0, (int)SeekOrigin.Current);
                
                if (offset != -1)
                    canSeek = true;

                Native.fread(IntPtr.Zero, 0, 0, file);
                
                if (Native.ferror(file) == 0)
                    canRead = true;
                
                Native.fwrite(IntPtr.Zero, 0, 0, file);
                
                if (Native.ferror(file) == 0)
                    canWrite = true;

                Native.clearerr(file);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid file stream", "fileStream", e);
            }

            GC.KeepAlive(this);
        }

        private static IntPtr Fopen(string path, string mode)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("path");
            if (mode == null)
                throw new ArgumentNullException("mode");

            IntPtr f = Native.fopen(path, mode);

            if (f == IntPtr.Zero)
                throw new DirectoryNotFoundException("path",
                    new Win32Exception(Marshal.GetLastWin32Error()));
                    
            return f;
        }

        private void AssertNotDisposed()
        {
            if (file == IntPtr.Zero)
                throw new ObjectDisposedException("Invalid File Stream");
            GC.KeepAlive(this);
        }

        private static void AssertValidBuffer(ICollection<byte> buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "< 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "< 0");
            if (offset > buffer.Count)
                throw new ArgumentException("destination offset is beyond array size");
            if (offset > (buffer.Count - count))
                throw new ArgumentException("would overrun buffer");
        }

        #region Stream Members

        public override bool  CanRead
        {
            get { return canRead; }
        }

        public override bool  CanSeek
        {
            get { return canSeek; }
        }

        public override bool  CanWrite
        {
            get { return canWrite; }
        }
        
        public override void  Flush()
        {
            AssertNotDisposed();
            int r = Native.fflush(file);
            if (r != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            GC.KeepAlive(this);
        }

        public override long Length
        {
            get
            {
                AssertNotDisposed();
                if (!CanSeek)
                    throw new NotSupportedException("File Stream doesn't support seeking");
                long curPos = Native.ftell(file);
                if (curPos == -1)
                    throw new NotSupportedException("Unable to obtain current file position");
                
                Native.fseek(file, 0, (int)SeekOrigin.End);

                long endPos = Native.ftell(file);

                if (endPos == -1)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                Native.fseek(file, curPos, (int)SeekOrigin.Begin);                

                GC.KeepAlive(this);
                return endPos;
            }
        }

        public override long Position
        {
            get
            {
                AssertNotDisposed();
                if (!CanSeek)
                    throw new NotSupportedException("The stream does not support seeking");
                long pos = Native.ftell(file);
                if (pos == -1)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                GC.KeepAlive(this);
                return pos;
            }
            set
            {
                AssertNotDisposed();
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override int  Read(byte[] buffer, int offset, int count)
        {
            AssertNotDisposed();
            AssertValidBuffer(buffer, offset, count);
            if (!CanRead)
                throw new NotSupportedException("Stream does not support reading");

            ulong r;
            unsafe
            {
                fixed (byte* buf = &buffer[offset])
                {
                    r = Native.fread((IntPtr)buf, 1, count, file);
                }
            }
            if (r != (ulong)count)
            {
                if (Native.ferror(file) != 0)
                    throw new IOException();
            }
            GC.KeepAlive(this);
            return (int)r;
        }

        public override long  Seek(long offset, SeekOrigin origin)
        {
            AssertNotDisposed();
            if (!CanSeek)
                throw new NotSupportedException("The File Stream does not support seeking");

            int r = Native.fseek(file, offset, (int)origin);

            if (r != 0)
                throw new IOException("Unable to seek", 
                    new Win32Exception(Marshal.GetLastWin32Error()));

            long pos = Native.ftell(file);
            if (pos == -1)
                throw new IOException("Unable to get current file position",
                        new Win32Exception(Marshal.GetLastWin32Error()));

            GC.KeepAlive(this);
            return pos;
        }

        public override void  SetLength(long value)
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public override void  Write(byte[] buffer, int offset, int count)
        {
            //AssertNotDisposed();
            //AssertValidBuffer(buffer, offset, count);

            if (!CanWrite)
                throw new NotSupportedException("File Stream does not support writing");

            ulong r;
            unsafe
            {
                fixed (byte* buf = &buffer[offset])
                {
                    r = Native.fwrite((IntPtr)buf, 1, count, file);
                }
            }
            if (r != (ulong)count)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            GC.KeepAlive(this);
        }

        public override void Close()
        {
            if (file == IntPtr.Zero)
                return;

            if (owner)
            {
                int r = Native.fclose(file);
                if (r != 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
                Flush();

            file = IntPtr.Zero;
            canRead = false;
            canSeek = false;
            canWrite = false;

            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        ~NativeFileStream ()
		{
			Close ();
		}
    }
}