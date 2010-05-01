using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AopAlliance.Intercept;
using System.Threading;
using HeuristicLab.Hive.Server.LINQDataAccess;
using System.Transactions;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Tracing;

namespace HeuristicLab.Hive.Server {
  internal class ServiceCallInterception : IMethodInterceptor {

    private const bool UseTransactions = true;

    public object Invoke(IMethodInvocation invocation) {
      Logger.Info("Entering Method: " + invocation.Method.Name);

      if(ContextFactory.Context != null) {
        Logger.Info("Not null context found - why wasn't this disposed?");
        try {
          Logger.Info("Trying to dispose");
          ContextFactory.Context.Dispose();
        } catch (Exception e) {
          Logger.Error(e);
        }
        ContextFactory.Context = null;
      }

      Object obj = null;

      if (invocation.Method.Name.Equals("SendStreamedJob") || invocation.Method.Name.Equals("StoreFinishedJobResultStreamed")) {        
        ContextFactory.Context.Connection.Open();
        if(UseTransactions) {
          Logger.Debug("Opening Transaction");
          ContextFactory.Context.Transaction = ContextFactory.Context.Connection.BeginTransaction(ApplicationConstants.ISOLATION_LEVEL);
        } else {
          Logger.Debug("Not using a Transaction");
        }
        try {
          obj = invocation.Proceed();
          Logger.Info("leaving context open for streaming");
        }
        catch (Exception e) {
          Logger.Error("Exception occured during method invocation", e);              
          ContextFactory.Context.Dispose();
          ContextFactory.Context = null;
        }        
      } else {
        if(UseTransactions) {
          using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = ApplicationConstants.ISOLATION_LEVEL_SCOPE })) {
            try {
              Logger.Debug("Current Transaction Isolation level is: " + Transaction.Current.IsolationLevel);
              obj = invocation.Proceed();
              scope.Complete();
            }
            catch (Exception e) {
              Logger.Error("Exception occured during method invocation", e);              
            }
            finally {
              ContextFactory.Context.Dispose();
              Logger.Debug("Disposed Context");
              ContextFactory.Context = null;
              Logger.Debug("Set Context Null");
            }
          }
        } else {
          try {
            obj = invocation.Proceed();            
          }
          catch (Exception e) {
            Logger.Error("Exception occured during method invocation", e);                          
          }
          finally {
            ContextFactory.Context.Dispose();
            Logger.Debug("Disposed Context");
            ContextFactory.Context = null;
            Logger.Debug("Set Context Null");
          }  
        }
      }
      Logger.Info("Leaving Method: " + invocation.Method.Name);

      return obj;
    }
  }
}
     
  