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
using System.IO;

namespace HeuristicLab.SupportVectorMachines {
  public partial class SVMModelView : ViewBase {
    private SVMModel model;
    public SVMModelView()
      : base() {
      InitializeComponent();
    }
    public SVMModelView(SVMModel model)
      : base() {
      InitializeComponent();
      this.model = model;
      model.Changed += (sender, args) => Refresh();

      numberOfSupportVectors.DataBindings.Add(new Binding("Text", model.Model, "SupportVectorCount"));
      rho.DataBindings.Add(new Binding("Text", model.Model, "Rho"));
      svmType.DataBindings.Add(new Binding("Text", model.Model.Parameter, "SvmType"));
      kernelType.DataBindings.Add(new Binding("Text", model.Model.Parameter, "KernelType"));
      gamma.DataBindings.Add(new Binding("Text", model.Model.Parameter, "Gamma"));
      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      textBox.Text = GetModelString();
    }

    private string GetModelString() {
      using (MemoryStream s = new MemoryStream()) {
        SVMModel.Export(model, s);
        s.Flush();
        s.Seek(0, System.IO.SeekOrigin.Begin);
        StreamReader reader = new StreamReader(s);
        return reader.ReadToEnd();
      }
    }
  }
}
