using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AopAlliance.Intercept;
using System.Threading;

namespace HeuristicLab.Hive.Server {
  class ServiceCallInterception: IMethodInterceptor {
    #region IMethodInterceptor Members

    public object Invoke(IMethodInvocation invocation) {
      Console.WriteLine(DateTime.Now + " - " + Thread.CurrentThread.ManagedThreadId + " - Entering Method " + invocation.Method.Name);
      var obj = invocation.Proceed();
      Console.WriteLine(DateTime.Now + " - " + Thread.CurrentThread.ManagedThreadId + " - Leaving Method " + invocation.Method.Name);
      return obj;
    }

    #endregion
  }
}
