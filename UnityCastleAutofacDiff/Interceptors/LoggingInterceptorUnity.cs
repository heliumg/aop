using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using Microsoft.Practices.Unity.InterceptionExtension;
using Newtonsoft.Json;

namespace UnityCastleAutofacDiff.Interceptors
{
    public class LoggingInterceptorUnity : IInterceptionBehavior
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger("AOPLoggerUnity");
        private System.Diagnostics.Stopwatch _watch;

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            BeforeMethod(input);

            // Invoke the next behavior in the chain.
            var result = getNext()(input, getNext);

            AfterMethod(input, result);

            return result;
        }

        private void BeforeMethod(IMethodInvocation invocation)
        {
            _watch = System.Diagnostics.Stopwatch.StartNew();

            var methodArguments = new List<string>();
            foreach (var argument in invocation.Arguments)
            {
                methodArguments.Add(JsonConvert.SerializeObject(argument));
            }

            var loggingInfo = $"Enter Method {invocation.MethodBase.DeclaringType}.{invocation.MethodBase.Name}({string.Join(", ", methodArguments)})";
            _logger.Info(loggingInfo);
        }
        private void AfterMethod(IMethodInvocation invocation, IMethodReturn result)
        {
            string loggingInfo;
            var method = invocation.MethodBase as MethodInfo;
            if (method != null)
            {
                if (method.ReturnType == typeof(void))
                {
                    loggingInfo = $"Exit Method {invocation.MethodBase.DeclaringType}.{invocation.MethodBase.Name}.";
                    _logger.Info(loggingInfo);
                }
                else if (method.ReturnType == typeof(Task))
                {
                    loggingInfo =
                        $"Exit Method {invocation.MethodBase.DeclaringType}.{invocation.MethodBase.Name} with returning value: task.";
                    _logger.Info(loggingInfo);
                }
                else if (method.ReturnType.IsGenericType &&
                         method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var task = (Task) result.ReturnValue;
                    task.ContinueWith((antecedent) =>
                    {
                        var res = antecedent.GetType().GetProperty("Result").GetValue(antecedent, null);
                        loggingInfo =
                            $"Exit Method {invocation.MethodBase.DeclaringType}.{invocation.MethodBase.Name} with returning value: {JsonConvert.SerializeObject(res)}.";
                        _logger.Info(loggingInfo);
                    });
                }
                else
                {
                    loggingInfo =
                        $"Exit Method {invocation.MethodBase.DeclaringType}.{invocation.MethodBase.Name} with returning value: {JsonConvert.SerializeObject(result.ReturnValue)}.";
                    _logger.Info(loggingInfo);
                }
            }
            _watch.Stop();
            var elapsedMs = _watch.ElapsedMilliseconds;
            if (invocation.MethodBase.DeclaringType != null)
                _logger.Info($"Duration: {elapsedMs}, GUID: {invocation.MethodBase.DeclaringType.GUID}");
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
