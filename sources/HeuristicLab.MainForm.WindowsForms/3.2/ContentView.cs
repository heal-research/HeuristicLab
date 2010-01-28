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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class ContentView : View {
    public ContentView()
      : base() {
      InitializeComponent();
    }

    public ContentView(object content)
      : this() {
      this.content = content;
    }

    private object content;
    public object Content {
      get { return content; }
      protected set {
        if ((value != null) && (!MainFormManager.ViewCanViewObject(this, value)))
          throw new ArgumentException(string.Format("View \"{0}\" cannot view object \"{1}\".", this.GetType().Name, value.GetType().Name));
        if (InvokeRequired) {
          Invoke(new Action<object>(delegate(object o) { this.Content = o; }), value);
        } else {
          if (this.content != value) {
            if (this.content != null)
              this.DeregisterObjectEvents();
            this.content = value;
            this.OnContentChanged(new EventArgs());
            if (this.content != null)
              this.RegisterObjectEvents();
          }
        }
      }
    }

    /// <summary>
    /// Adds eventhandlers to the current instance.
    /// </summary>
    protected virtual void RegisterObjectEvents() {
    }

    /// <summary>
    /// Removes the eventhandlers from the current instance.
    /// </summary>
    protected virtual void DeregisterObjectEvents() {
    }

    /// <summary>
    /// Is called when the content property changes.
    /// </summary>
    protected virtual void OnContentChanged(EventArgs e) {
    }

  }
}
