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

namespace HeuristicLab.Common {
  public class StorableContent : DeepCloneable, IStorableContent {
    private string filename;
    public string Filename {
      get { return filename; }
      set {
        if (!filename.Equals(value)) {
          filename = value;
          OnFilenameChanged();
        }
      }
    }

    public StorableContent() {
      filename = string.Empty;
    }

    public event EventHandler FilenameChanged;
    protected virtual void OnFilenameChanged() {
      EventHandler handler = FilenameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
