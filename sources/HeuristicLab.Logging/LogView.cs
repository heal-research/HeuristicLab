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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Logging {
  public partial class LogView : ViewBase {
    public Log Log {
      get { return (Log)base.Item; }
      set { base.Item = value; }
    }

    public LogView() {
      InitializeComponent();
      Caption = "Log View";
    }
    public LogView(Log log)
      : this() {
      Log = log;
    }

    protected override void RemoveItemEvents() {
      if (Log != null) {
        Log.Items.ItemAdded -= new EventHandler<ItemIndexEventArgs>(Items_ItemAdded);
        Log.Items.ItemRemoved -= new EventHandler<ItemIndexEventArgs>(Items_ItemRemoved);
      }
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      if (Log != null) {
        Log.Items.ItemAdded += new EventHandler<ItemIndexEventArgs>(Items_ItemAdded);
        Log.Items.ItemRemoved += new EventHandler<ItemIndexEventArgs>(Items_ItemRemoved);
      }
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      qualityLogTextBox.Clear();
      if (Log == null) {
        qualityLogTextBox.Enabled = false;
      } else {
        string[] lines = new string[Log.Items.Count];
        for (int i = 0; i < Log.Items.Count; i++) {
          lines[i] = Log.Items[i].ToString().Replace(';','\t');
        }
        qualityLogTextBox.Lines = lines;
        qualityLogTextBox.Enabled = true;
      }
    }

    #region ItemList Events
    private delegate void IndexDelegate(int index);
    private void Items_ItemRemoved(object sender, ItemIndexEventArgs e) {
      RemoveItem(e.Index);
    }
    private void RemoveItem(int index) {
      if (InvokeRequired)
        Invoke(new IndexDelegate(RemoveItem), index);
      else {
        string[] lines = new string[qualityLogTextBox.Lines.Length - 1];
        Array.Copy(qualityLogTextBox.Lines, 0, lines, 0, index);
        Array.Copy(qualityLogTextBox.Lines, index + 1, lines, index, lines.Length - index);
        qualityLogTextBox.Lines = lines;
      }
    }
    private void Items_ItemAdded(object sender, ItemIndexEventArgs e) {
      AddItem(e.Index);
    }
    private void AddItem(int index) {
      if (InvokeRequired)
        Invoke(new IndexDelegate(AddItem), index);
      else {
        string[] lines = new string[qualityLogTextBox.Lines.Length + 1];
        Array.Copy(qualityLogTextBox.Lines, 0, lines, 0, index);
        Array.Copy(qualityLogTextBox.Lines, index, lines, index + 1, qualityLogTextBox.Lines.Length - index);
        lines[index] = Log.Items[index].ToString().Replace(';', '\t');
        qualityLogTextBox.Lines = lines;
      }
    }
    #endregion
  }
}
