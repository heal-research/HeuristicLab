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
using System.Xml;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class ViewBase : UserControl, IView {   
    public ViewBase() {
      InitializeComponent();
    }

    private string myCaption;
    public string Caption {
      get { return myCaption; }
      set {
        if (value != myCaption) {
          myCaption = value;
          OnCaptionChanged();
        }
      }
    }

    public event EventHandler CaptionChanged;
    protected virtual void OnCaptionChanged() {
      if (CaptionChanged != null)
        CaptionChanged(this, new EventArgs());      
    }

    public event EventHandler Changed;
    protected virtual void OnChanged() {
      if (Changed != null)
        Changed(this, new EventArgs());    
    }

    public virtual void OnClosing(object sender, CancelEventArgs e) {
    }

    public virtual void OnClosed(object sender, EventArgs e) {
    }
  }

  public class ViewBase<T> : ViewBase, IView<T> {
    private T item;
    public virtual T Item {
      get { return this.item; }
      protected set { this.item = value; }
    }

    public virtual void View(T item) {
    }
  }
}
