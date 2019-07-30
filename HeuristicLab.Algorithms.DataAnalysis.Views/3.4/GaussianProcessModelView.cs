#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Gaussian Process Model")]
  [Content(typeof(IGaussianProcessModel), true)]
  public partial class GaussianProcessModelView : AsynchronousContentView {

    public new IGaussianProcessModel Content {
      get { return (IGaussianProcessModel)base.Content; }
      set {
        base.Content = value;
      }
    }

    public GaussianProcessModelView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        // clear
        resultCollectionView.Content = null;
      } else {
        resultCollectionView.Content = CreateResultCollection(Content);
      }
    }

    private ResultCollection CreateResultCollection(IGaussianProcessModel gaussianProcessModel) {
      var res = new ResultCollection();
      res.Add(new Result("Mean Function", gaussianProcessModel.MeanFunction));
      res.Add(new Result("Covariance Function", gaussianProcessModel.CovarianceFunction));
      res.Add(new Result("Noise sigma", new DoubleValue(gaussianProcessModel.SigmaNoise)));
      return res;
    }
  }
}
