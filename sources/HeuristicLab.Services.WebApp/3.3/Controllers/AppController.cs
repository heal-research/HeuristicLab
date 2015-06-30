#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System.Web.Mvc;

namespace HeuristicLab.Services.WebApp.Controllers {

  public class AppController : Controller {

    public ActionResult Index() {
      if (!Request.Path.EndsWith("/")) {
        return RedirectPermanent(Request.Url + "/");
      }
      return View("~/WebApp/shared/layout/layout.cshtml");
    }

    public ActionResult Empty() {
      return View("~/WebApp/shared/restricted/empty.cshtml");
    }

    public ActionResult LoadSharedView(string directory, string view, string dateTime) {
      // dateTime is optional to avoid browser caching
      return View(string.Format("~/WebApp/shared/{0}/{1}", directory, view));
    }

    public ActionResult LoadPluginView(string plugin, string view, string dateTime) {
      // dateTime is optional to avoid browser caching
      return View(string.Format("~/WebApp/plugins/{0}/{1}", plugin, view));
    }

    public ActionResult RedirectUrl(string url) {
      return RedirectPermanent(url);
    }
  }
}