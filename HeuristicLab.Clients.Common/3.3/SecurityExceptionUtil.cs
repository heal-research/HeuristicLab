using System;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Common {
  public class SecurityExceptionUtil {
    public static void TryAndReportSecurityExceptions(Action action, Action finallyCallback = null) {
      try {
        action();
      } catch (MessageSecurityException ex) {
        ErrorHandlingUI.ShowErrorDialog("There is something wrong with the applied security. Are your credentials correct?", ex);
      } catch (SecurityAccessDeniedException ex) {
        ErrorHandlingUI.ShowErrorDialog("The security authorization request failed. Are you a Hive administrator?", ex);
      } finally {
        if (finallyCallback != null) finallyCallback();
      }
    }

    public static async Task TryAsyncAndReportSecurityExceptions(Action action, Action finallyCallback = null) {
      try {
        await Task.Run(action);
      } catch (MessageSecurityException ex) {
        ErrorHandlingUI.ShowErrorDialog("There is something wrong with the applied security. Are your credentials correct?", ex);
      } catch (SecurityAccessDeniedException ex) {
        ErrorHandlingUI.ShowErrorDialog("The security authorization request failed. Are you a Hive administrator?", ex);
      } finally {
        if (finallyCallback != null) finallyCallback();
      }
    }
  }
}
