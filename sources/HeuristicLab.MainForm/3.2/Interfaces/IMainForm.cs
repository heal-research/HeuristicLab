#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace HeuristicLab.MainForm {
  public interface IMainForm {
    string Title { get; set; }

    IView ActiveView { get; }
    IEnumerable<IView> Views { get; }

    event EventHandler ActiveViewChanged;
    event EventHandler Changed;

    event EventHandler<ViewEventArgs> ViewClosed;
    event EventHandler<ViewShownEventArgs> ViewShown;
    event EventHandler<ViewEventArgs> ViewHidden;

    Type UserInterfaceItemType { get; }
    void CloseAllViews();
    void Close();
  }
}
