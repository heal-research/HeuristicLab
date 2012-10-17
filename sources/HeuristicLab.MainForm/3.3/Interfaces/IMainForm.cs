#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.MainForm {
  public interface IMainForm {
    string Title { get; set; }
    IEnumerable<ICommandLineArgument> Arguments { get; set; }

    IView ActiveView { get; }
    IEnumerable<IView> Views { get; }

    event EventHandler ActiveViewChanged;
    event EventHandler Changed;

    event EventHandler<ViewEventArgs> ViewClosed;
    event EventHandler<ViewShownEventArgs> ViewShown;
    event EventHandler<ViewEventArgs> ViewHidden;

    IContentView ShowContent(IContent content);
    IContentView ShowContent<T>(T content, bool reuseExistingView, IEqualityComparer<T> comparer = null) where T : class,IContent;
    IContentView ShowContent(IContent content, Type viewType);

    Type UserInterfaceItemType { get; }
    void CloseAllViews();
    void Close();
  }
}
