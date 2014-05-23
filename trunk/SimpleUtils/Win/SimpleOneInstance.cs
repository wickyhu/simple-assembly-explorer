using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.IO;

namespace SimpleUtils.Win
{
    public class SimpleOneInstance : IDisposable
    {
        private Mutex _mutex;
        private bool _owned = false;
        private string _identifier = String.Empty;

        public SimpleOneInstance()
        {
            _identifier = Assembly.GetEntryAssembly().GetName().Name;
            _mutex = new Mutex(
                true, // desire intial ownership
                _identifier,
                out _owned);
        }

        public SimpleOneInstance(string identifier)
        {
            _identifier = Assembly.GetEntryAssembly().GetName().Name + identifier;
            _mutex = new Mutex(
                true, // desire intial ownership
                _identifier,
                out _owned);
        }

        ~SimpleOneInstance()
        {
            Release();
        }

        public bool IsFirstInstance
        {
            get { return _owned; }
        }

        private void Release()
        {
            if (_owned)
            {
                _mutex.ReleaseMutex();
                _owned = false;
            }
        }

        #region Implementation of IDisposable
        public void Dispose()
        {
            //release mutex (if necessary) and notify 
            // the garbage collector to ignore the destructor
            Release();
            GC.SuppressFinalize(this);
        }
        #endregion

    }//end of class
}