using BusinessObjects.Util;
using PostSharp.Aspects;
using System;

namespace BusinessObjects.Aspect
{
    [Serializable]
    public class TransactionAspect : OnMethodBoundaryAspect
    {
        [NonSerialized]
        private ConnectionUtil connectionUtil = null;

        public override void OnEntry(MethodExecutionArgs args)
        {
            this.connectionUtil = new ConnectionUtil();

            this.connectionUtil.BeginTransaction();
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            this.connectionUtil.CommitTransaction();
        }

        public override void OnException(MethodExecutionArgs args)
        {
            this.connectionUtil.RollbackTransaction();
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            this.connectionUtil.Dispose();
        }
    }
}
