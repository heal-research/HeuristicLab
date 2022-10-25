using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HEAL.Hive.SwaggerClient
{
  public partial class swaggerClient {

    // prevent endless recursion
    private static bool loginInProgress = false;
    private static LoginResponseDTO loginResponse;
    private Func<string> username = () => "";
    private Func<string> password = () => "";

    public void SetCredentials(Func<string> username, Func<string> password) {
      this.username = username;
      this.password = password;
    }

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url) {
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.Token);
      if (loginInProgress) return;
      if (loginResponse != null && DateTime.UtcNow < DateTime.Parse(loginResponse.Expiration)) return;

      loginInProgress = true;
      try {
        loginResponse = LoginAsync(new LoginDTO { Username = username(), Password = password() }).Result;
      } catch (AggregateException) {
        loginResponse = null;
      } finally {
        loginInProgress = false;
      }

      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.Token);
    }

  }
}
