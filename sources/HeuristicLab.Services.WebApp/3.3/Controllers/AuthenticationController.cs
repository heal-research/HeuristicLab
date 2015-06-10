using System.Web.Http;
using System.Web.Security;
using HeuristicLab.Services.WebApp.Controllers.DataTransfer;

namespace HeuristicLab.Services.WebApp.Controllers {

  [Authorize]
  public class AuthenticationController : ApiController {

    [AllowAnonymous]
    public bool Login(User user) {
      if (ModelState.IsValid && Membership.ValidateUser(user.Username, user.Password)) {
        FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
        return true;
      }
      FormsAuthentication.SignOut();
      return false;
    }

    [HttpGet, HttpPost]
    public bool Logout() {
      FormsAuthentication.SignOut();
      return true;
    }
  }
}