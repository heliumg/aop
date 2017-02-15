using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using log4net;
using Newtonsoft.Json;

namespace UnityCastleAutofacDiff.Interceptors
{
    public class LoggingInterceptorCastle : IInterceptor
    {
        private readonly ILog _logger = LogManager.GetLogger("AOPLoggerCastle");
        private System.Diagnostics.Stopwatch _watch;

        public void Intercept(IInvocation invocation)
        {
            BeforeMethod(invocation);
            invocation.Proceed();
            AfterMethod(invocation);
        }

        private void BeforeMethod(IInvocation invocation)
        {
            _watch = System.Diagnostics.Stopwatch.StartNew();

            var methodArguments = new List<string>();
            foreach (var argument in invocation.Arguments)
            {
                methodArguments.Add(JsonConvert.SerializeObject(argument));
            }

            var loggingInfo = $"Enter Method {invocation.TargetType.FullName}.{invocation.Method.Name}({string.Join(", ", methodArguments)})";
            _logger.Info(loggingInfo);
        }
        private void AfterMethod(IInvocation invocation)
        {
            string loggingInfo;
            if (invocation.Method.ReturnType == typeof(void))
            {
                loggingInfo = $"Exit Method {invocation.TargetType.FullName}.{invocation.Method.Name}.";
                _logger.Info(loggingInfo);
            }
            else if (invocation.Method.ReturnType == typeof(Task))
            {
                loggingInfo = $"Exit Method {invocation.TargetType.FullName}.{invocation.Method.Name} with returning value: task.";
                _logger.Info(loggingInfo);
            }
            else if (invocation.Method.ReturnType.IsGenericType && invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var task = (Task)invocation.ReturnValue;
                task.ContinueWith((antecedent) =>
                {
                    var result = antecedent.GetType().GetProperty("Result").GetValue(antecedent, null);
                    loggingInfo = $"Exit Method {invocation.TargetType.FullName}.{invocation.Method.Name} with returning value: {JsonConvert.SerializeObject(result)}.";
                    _logger.Info(loggingInfo);
                });
            }
            else
            {
                loggingInfo = $"Exit Method {invocation.TargetType.FullName}.{invocation.Method.Name} with returning value: {JsonConvert.SerializeObject(invocation.ReturnValue)}.";
                _logger.Info(loggingInfo);
            }
            _watch.Stop();
            var elapsedMs = _watch.ElapsedMilliseconds;
            _logger.Info($"Duration: {elapsedMs}, GUID: {invocation.TargetType.GUID}");
        }
    }
}
