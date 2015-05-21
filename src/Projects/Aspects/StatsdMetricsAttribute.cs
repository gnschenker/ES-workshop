using System;
using PostSharp.Aspects;
using StatsdClient;

namespace Projects.Aspects
{
    [Serializable]
    public class StatsdMetricsAttribute : OnMethodBoundaryAspect
    {
        private string _methodKey = "";

        public override void OnEntry(MethodExecutionArgs args)
        {
            _methodKey = args.Instance.ToString() + "." + args.Method.Name;
            System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            w.Start();
            args.MethodExecutionTag = w;
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            System.Diagnostics.Stopwatch w = (System.Diagnostics.Stopwatch) args.MethodExecutionTag;
            w.Stop();

            Metrics.Timer(_methodKey, (int)w.ElapsedMilliseconds);
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Metrics.Counter(_methodKey);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Metrics.Counter(_methodKey + "." + args.Exception.GetType().Name);
        }
    }
}