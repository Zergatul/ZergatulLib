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

        public BioStreamWrapper(Stream stream, int bufferSize = 0x10000)
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

        static BioStreamWrapper()
        {
            int type = OpenSsl.BIO_get_new_index();
            if (type == -1)
                throw new OpenSslException();

            _bioMethod = OpenSsl.BIO_meth_new(type, nameof(BioStreamWrapper));
            if (_bioMethod == IntPtr.Zero)
                throw new OpenSslException();

            if (OpenSsl.BIO_meth_set_read(_bioMethod, ReadDelegate) != 1)
                throw new OpenSslException();

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

        private static int ReadDelegate(IntPtr bio, IntPtr buffer, int count)
        {
            int id = (int)OpenSsl.BIO_get_data(bio);
            BioStreamWrapper wrapper;
            lock (_syncRoot)
                wrapper = _wrappers[id];

            count = System.Math.Min(count, wrapper._readBuffer.Length);
            count = wrapper._stream.Read(wrapper._readBuffer, 0, count);
            Marshal.Copy(wrapper._readBuffer, 0, buffer, count);
            return count;
        }

        private static int WriteDelegate(IntPtr bio, IntPtr buffer, int count)
        {
            int id = (int)OpenSsl.BIO_get_data(bio);
            BioStreamWrapper wrapper;
            lock (_syncRoot)
                wrapper = _wrappers[id];

            count = System.Math.Min(count, wrapper._writeBuffer.Length);
            Marshal.Copy(buffer, wrapper._writeBuffer, 0, count);
            wrapper._stream.Write(wrapper._writeBuffer, 0, count);
            return count;
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