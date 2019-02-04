using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl
{
    internal class BioStreamWrapper : IDisposable
    {
        private static readonly object _syncRoot = new object();
        private static readonly IntPtr _bioMethod;
        private static readonly Dictionary<int, BioStreamWrapper> _wrappers;
        private static readonly StaticFinalizer _finalizer;
        private static int _lastId;

        public IntPtr Bio { get; }

        private readonly Stream _stream;
        private readonly int _id;
        private byte[] _readBuffer;
        private byte[] _writeBuffer;
        private int _writeBufferPos;

        public BioStreamWrapper(Stream stream, int bufferSize = 0x8000)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _id = GetNextId(this);

            Bio = OpenSsl.BIO_new(_bioMethod);
            if (Bio == IntPtr.Zero)
                throw new OpenSslException();

            OpenSsl.BIO_set_init(Bio, 1);
            OpenSsl.BIO_set_data(Bio, (IntPtr)_id);

            _readBuffer = new byte[bufferSize];
            _writeBuffer = new byte[bufferSize];
            _writeBufferPos = 0;
        }

        public void Flush()
        {
            if (_writeBufferPos > 0)
            {
                _stream.Write(_writeBuffer, 0, _writeBufferPos);
                _stream.Flush();
                _writeBufferPos = 0;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            lock (_syncRoot)
                _wrappers.Remove(_id);

            if (Bio != IntPtr.Zero)
                OpenSsl.BIO_free(Bio);
        }

        ~BioStreamWrapper()
        {
            Dispose(false);
        }

        #endregion

        #region Static methods

        // to prevent garbage collector to collect delegate instances
        // we store them in static fields
        private static readonly OpenSsl.BIOCtrlDelegate CtrlDelegate;
        private static readonly OpenSsl.BIOReadDelegate ReadDelegate;
        private static readonly OpenSsl.BIOWriteDelegate WriteDelegate;

        static BioStreamWrapper()
        {
            int type = OpenSsl.BIO_get_new_index();
            if (type == -1)
                throw new OpenSslException();

            _bioMethod = OpenSsl.BIO_meth_new(type, nameof(BioStreamWrapper));
            if (_bioMethod == IntPtr.Zero)
                throw new OpenSslException();

            CtrlDelegate = BIOCtrl;
            if (OpenSsl.BIO_meth_set_ctrl(_bioMethod, CtrlDelegate) != 1)
                throw new OpenSslException();

            ReadDelegate = BIORead;
            if (OpenSsl.BIO_meth_set_read(_bioMethod, ReadDelegate) != 1)
                throw new OpenSslException();

            WriteDelegate = BIOWrite;
            if (OpenSsl.BIO_meth_set_write(_bioMethod, WriteDelegate) != 1)
                throw new OpenSslException();

            _wrappers = new Dictionary<int, BioStreamWrapper>();
            _lastId = 1;

            _finalizer = new StaticFinalizer();
        }

        private static int GetNextId(BioStreamWrapper wrapper)
        {
            lock (_syncRoot)
            {
                while (_wrappers.ContainsKey(_lastId))
                    _lastId++;
                _wrappers.Add(_lastId, wrapper);
                return _lastId;
            }
        }

        private static long BIOCtrl(IntPtr bio, int cmd, long larg, IntPtr parg)
        {
            switch (cmd)
            {
                case OpenSsl.BIO_CTRL_PUSH:
                case OpenSsl.BIO_CTRL_POP:
                    return 0;

                case OpenSsl.BIO_CTRL_FLUSH:
                    var wrapper = GetWrapper(bio);
                    if (wrapper._writeBufferPos > 0)
                    {
                        wrapper._stream.Write(wrapper._writeBuffer, 0, wrapper._writeBufferPos);
                        wrapper._stream.Flush();
                        wrapper._writeBufferPos = 0;
                    }
                    return 1;

                default:
                    break;
            }
            return 0;
        }

        private static int BIORead(IntPtr bio, IntPtr buffer, int count)
        {
            var wrapper = GetWrapper(bio);
            count = System.Math.Min(count, wrapper._readBuffer.Length);
            count = wrapper._stream.Read(wrapper._readBuffer, 0, count);
            Marshal.Copy(wrapper._readBuffer, 0, buffer, count);
            return count;
        }

        private static int BIOWrite(IntPtr bio, IntPtr buffer, int count)
        {
            var wrapper = GetWrapper(bio);
            int total = 0;
            while (count > 0)
            {
                int write = System.Math.Min(count, wrapper._writeBuffer.Length - wrapper._writeBufferPos);
                Marshal.Copy(buffer + total, wrapper._writeBuffer, wrapper._writeBufferPos, write);
                wrapper._writeBufferPos += write;
                total += write;
                count -= write;
                if (wrapper._writeBufferPos == wrapper._writeBuffer.Length)
                {
                    wrapper._stream.Write(wrapper._writeBuffer, 0, wrapper._writeBuffer.Length);
                    wrapper._writeBufferPos = 0;
                }
            }

            return total;
        }

        private static BioStreamWrapper GetWrapper(IntPtr bio)
        {
            int id = (int)OpenSsl.BIO_get_data(bio);
            lock (_syncRoot)
                return _wrappers[id];
        }

        #endregion

        #region Nested Classes

        private class StaticFinalizer
        {
            ~StaticFinalizer()
            {
                OpenSsl.BIO_meth_free(_bioMethod);
            }
        }

        #endregion
    }
}