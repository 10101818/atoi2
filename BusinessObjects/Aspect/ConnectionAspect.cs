using BusinessObjects.Util;
using PostSharp.Aspects;
using System;

namespace BusinessObjects.Aspect
{
    [Serializable]
    public class ConnectionAspect : OnMethodBoundaryAspect
    {
        [NonSerialized]
        private ConnectionUtil connectoinUtil = null;

        public override void OnEntry(MethodExecutionArgs args)
        {
            this.connectoinUtil = new ConnectionUtil();
        }
        public override void OnExit(MethodExecutionArgs args)
        {
            this.connectoinUtil.Dispose();
        }
    }
}

