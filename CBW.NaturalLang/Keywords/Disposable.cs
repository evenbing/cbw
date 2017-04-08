using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.NaturalLang
{
    public abstract class Disposable : IDisposable
    {
        ~Disposable()
        {
            this.Dispose(false);
        }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected bool IsDisposed
        {
            get;
            private set;
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeNativeResources()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    DisposeManagedResources();
                }

                DisposeNativeResources();

                IsDisposed = true;
            }
        }

    }
}
