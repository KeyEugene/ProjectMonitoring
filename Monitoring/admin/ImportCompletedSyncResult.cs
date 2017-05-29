using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace Monitoring
{
    public class ImportCompletedSyncResult : IAsyncResult
    {

        private Exception operationException;
        public Exception OperationException
        {
            get { return operationException; }
            set { operationException = value; }
        }

        private object state;
        object IAsyncResult.AsyncState
        {
            get { return state; }
        }

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get { return null; }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return true; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return true; }
        }

        public ImportCompletedSyncResult(Exception operationException,
            AsyncCallback asyncCallback, object asyncState)
        {
            state = asyncState;
            OperationException = operationException;
            if (asyncCallback != null)
                asyncCallback(this);
        }


    }
}