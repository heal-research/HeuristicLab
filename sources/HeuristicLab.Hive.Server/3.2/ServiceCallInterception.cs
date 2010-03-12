using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AopAlliance.Intercept;
using System.Threading;
using HeuristicLab.Hive.Server.LINQDataAccess;
using System.Transactions;

namespace HeuristicLab.Hive.Server {
  class ServiceCallInterception: IMethodInterceptor {
    #region IMethodInterceptor Members

    public object Invoke(IMethodInvocation invocation) {
      Console.WriteLine(DateTime.Now + " - " + Thread.CurrentThread.ManagedThreadId + " - Entering Method " + invocation.Method.Name);

      Object obj;
      
     // using (TransactionScope scope = new TransactionScope()) {
        try {
          obj = invocation.Proceed();
     //     scope.Complete();
        } finally {
          ContextFactory.Context.Dispose();
          Console.WriteLine("setting old context null");
          ContextFactory.Context = null;
          Console.WriteLine("Disposing old Context");      
        }
     // }     
      Console.WriteLine(DateTime.Now + " - " + Thread.CurrentThread.ManagedThreadId + " - Leaving Method " + invocation.Method.Name);            
      return obj;
    }

    #endregion
  }
}
