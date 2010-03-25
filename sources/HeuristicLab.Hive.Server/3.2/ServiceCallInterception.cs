using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AopAlliance.Intercept;
using System.Threading;
using HeuristicLab.Hive.Server.LINQDataAccess;
using System.Transactions;

namespace HeuristicLab.Hive.Server {
  internal class ServiceCallInterception : IMethodInterceptor {

    private bool useTransactions = false;

    public object Invoke(IMethodInvocation invocation) {
      Console.WriteLine(DateTime.Now + " - " + Thread.CurrentThread.ManagedThreadId + " - Entering Method " +
                        invocation.Method.Name);

      if(ContextFactory.Context != null) {
        Console.WriteLine("Error - Not null context found - why wasn't this disposed?");
        ContextFactory.Context = null;
      }

      Object obj = null;

      if (invocation.Method.Name.Equals("SendStreamedJob") || invocation.Method.Name.Equals("StoreFinishedJobResultStreamed")) {        
        ContextFactory.Context.Connection.Open();
        if(useTransactions)
          ContextFactory.Context.Transaction = ContextFactory.Context.Connection.BeginTransaction();
        try {
          obj = invocation.Proceed();
          Console.WriteLine("leaving context open for Streaming");
        }
        catch (Exception e) {          
          Console.WriteLine(e);
          ContextFactory.Context.Dispose();
          ContextFactory.Context = null;
        }        
      } else {
        if(useTransactions) {
          using (TransactionScope scope = new TransactionScope()) {
            try {
              obj = invocation.Proceed();
              scope.Complete();
            }
            catch (Exception e) {
              Console.WriteLine("Exception Occured");
              Console.WriteLine(e);
            }
            finally {
              ContextFactory.Context.Dispose();
              Console.WriteLine("setting old context null");
              ContextFactory.Context = null;
              Console.WriteLine("Disposing old Context");
            }
          }
        } else {
          try {
            obj = invocation.Proceed();            
          }
          catch (Exception e) {
            Console.WriteLine("Exception Occured");
            Console.WriteLine(e);
          }
          finally {
            ContextFactory.Context.Dispose();
            Console.WriteLine("setting old context null");
            ContextFactory.Context = null;
            Console.WriteLine("Disposing old Context");
          }  
        }
      }
      Console.WriteLine(DateTime.Now + " - " + Thread.CurrentThread.ManagedThreadId + " - Leaving Method " +
                          invocation.Method.Name);

      return obj;
    }
  }
}
     
  