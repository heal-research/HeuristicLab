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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Core {
  public partial class EditorBase : ViewBase, IEditor {
    private string myFilename;
    public string Filename {
      get { return myFilename; }
      set {
        if (value != myFilename) {
          myFilename = value;
          OnFilenameChanged();
        }
      }
    }

    public EditorBase() {
      InitializeComponent();
      Caption = "Editor";
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (Item == null)
        Caption = "Editor";
      else
        Caption = "Editor (" + Item.GetType().Name + ")";
    }

    public event EventHandler FilenameChanged;
    protected virtual void OnFilenameChanged() {
      if (FilenameChanged != null)
        FilenameChanged(this, new EventArgs());
    }
  }
}
