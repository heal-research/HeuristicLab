#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public class Content : DeepCloneable, IContent {
    public Content() {
      this.readOnlyView = false;
    }
    public Content(bool readOnlyView)
      : this() {
      this.ReadOnlyView = readOnlyView;
    }

    private bool readOnlyView;
    public virtual bool ReadOnlyView {
      get { return readOnlyView; }
      set {
        if (readOnlyView != value) {
          readOnlyView = value;
          OnReadOnlyViewChanged();
        }
      }
    }
    public event EventHandler ReadOnlyViewChanged;
    protected virtual void OnReadOnlyViewChanged() {
      EventHandler handler = ReadOnlyViewChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
