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

namespace HeuristicLab.SupportVectorMachines {
  public partial class PredictorView : SVMModelView {
    private Predictor predictor;

    public PredictorView()
      : base() {
      InitializeComponent();
    }

    public PredictorView(Predictor predictor)
      : base(predictor.Model) {
      this.predictor = predictor;
      InitializeComponent();
      lowerLimitTextbox.DataBindings.Add(new Binding("Text", predictor, "LowerPredictionLimit"));
      upperLimitTextbox.DataBindings.Add(new Binding("Text", predictor, "UpperPredictionLimit"));
      maxTimeOffsetTextBox.DataBindings.Add(new Binding("Text", predictor, "MaxTimeOffset"));
      minTimeOffsetTextBox.DataBindings.Add(new Binding("Text", predictor, "MinTimeOffset"));
      UpdateControls();
    }

    protected override string GetModelString() {
      StringBuilder builder = new StringBuilder();
      builder.Append("LowerPredictionLimit: ").AppendLine(predictor.LowerPredictionLimit.ToString());
      builder.Append("UpperPredictionLimit: ").AppendLine(predictor.UpperPredictionLimit.ToString());
      builder.Append("MaxTimeOffset: ").AppendLine(predictor.MaxTimeOffset.ToString());
      builder.Append("MinTimeOffset: ").AppendLine(predictor.MinTimeOffset.ToString());
      builder.Append("InputVariables :");
      builder.Append(predictor.GetInputVariables().First());
      foreach (string variable in predictor.GetInputVariables().Skip(1)) {
        builder.Append("; ").Append(variable);
      }
      builder.AppendLine();
      builder.Append(base.GetModelString());
      return builder.ToString();
    }
  }
}
