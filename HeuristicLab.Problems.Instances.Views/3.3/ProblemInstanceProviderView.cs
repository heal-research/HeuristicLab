#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.Instances.Views {
  [View("ProblemInstanceProviderView")]
  [Content(typeof(IProblemInstanceProvider), IsDefaultView = false)]
  public abstract partial class ProblemInstanceProviderView : AsynchronousContentView {

    public new IProblemInstanceProvider Content {
      get { return (IProblemInstanceProvider)base.Content; }
      set { base.Content = value; }
    }

    public abstract IProblemInstanceConsumer Consumer { get; set; }
    public abstract IProblemInstanceExporter Exporter { get; set; }

    protected ProblemInstanceProviderView() {
      InitializeComponent();
    }

    protected string GetProblemType() {
      var item = Consumer as IItem;
      return item != null ? item.ItemName : "problem";
    }

    protected string GetProviderFormatInfo() {
      return Content != null ? Content.Name : string.Empty;
    }

    protected abstract void SetTooltip();
  }
}
