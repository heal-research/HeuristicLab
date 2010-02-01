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
using HeuristicLab.Core;


namespace HeuristicLab.GP {
  public partial class FunctionLibraryInjectorView : OperatorBaseView {
    private FunctionLibraryInjectorBase functionLibraryInjector;
    private OpenFileDialog openFileDialog;
    private SaveFileDialog saveFileDialog;

    public FunctionLibraryInjectorView()
      : base() {
      InitializeComponent();
      CreateFileDialogs();
    }

    public FunctionLibraryInjectorView(FunctionLibraryInjectorBase functionLibraryInjector)
      : base(functionLibraryInjector) {
      InitializeComponent();
      this.functionLibraryInjector = functionLibraryInjector;
      functionLibraryEditor.FunctionLibrary = functionLibraryInjector.FunctionLibrary;

      CreateFileDialogs();
    }

    private void CreateFileDialogs() {
      openFileDialog = new OpenFileDialog();
      openFileDialog.AddExtension = true;
      openFileDialog.DefaultExt = ".hl";
      openFileDialog.Filter = "HeuristicLab Dateien (*.hl) |*.hl| Alle Dateien (*.*) |*.*";
      openFileDialog.Multiselect = false;
      saveFileDialog = new SaveFileDialog();
      saveFileDialog.AddExtension = true;
      saveFileDialog.DefaultExt = ".hl";
      saveFileDialog.Filter = "HeuristicLab Dateien (*.hl) |*.hl| Alle Dateien (*.*) |*.*";
    }

    private void loadButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        functionLibraryInjector.FunctionLibrary = (FunctionLibrary)PersistenceManager.Load(openFileDialog.FileName);
        functionLibraryEditor.FunctionLibrary = functionLibraryInjector.FunctionLibrary;
      }
    }

    private void saveButton_Click(object sender, EventArgs e) {
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        PersistenceManager.Save(functionLibraryInjector.FunctionLibrary, saveFileDialog.FileName);
      }
    }
  }
}
