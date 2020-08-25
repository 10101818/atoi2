using NLog;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using System;
using System.Reflection;
using System.Text;

namespace BusinessObjects.Aspect
{
    [Serializable]
    [MulticastAttributeUsage(TargetMemberAttributes = MulticastAttributes.Instance)]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        [NonSerialized]
        static readonly Logger log = LogManager.GetCurrentClassLogger();

        public override void OnException(MethodExecutionArgs args)
        {
            log.Error(args.Exception, "Exception: '{0}' happened in {1}", args.Exception.Message, DisplayObjectInfo(args));
            args.FlowBehavior = FlowBehavior.RethrowException;
        }

        private string DisplayObjectInfo(MethodExecutionArgs args)
        {
            StringBuilder sb = new StringBuilder();
            Type type = args.Arguments.GetType();
            sb.Append(string.Format("Class '{0}'", args.Instance.GetType().Name));
            sb.Append(string.Format(", Method '{0}'", args.Method.Name));
            sb.Append(", Arguments:");
            if (args.Arguments.Count > 0)
            {
                bool first = true;
                foreach (FieldInfo f in type.GetFields())
                {
                    if (first)
                    {
                        sb.Append(" ");
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    sb.Append(f.FieldType.Name + " = ");
                    if (f.GetValue(args.Arguments) == null)
                    {
                        sb.Append("NULL");
                    }
                    else
                    {
                        sb.Append(f.GetValue(args.Arguments).ToString());
                    }
                }
            }
            else
            {
                sb.Append(" None");
            }

            return sb.ToString();
        }
    }
}
