#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.Optimization.Views;

namespace HeuristicLab.Algorithms.Benchmarks.Views {
  [View("Benchmark View")]
  [Content(typeof(Whetstone), true)]
  [Content(typeof(Dhrystone), true)]
  [Content(typeof(Linpack), true)]
  public partial class BenchmarkView : AlgorithmView {
    public BenchmarkView() {
      InitializeComponent();
      tabControl.TabPages.Remove(this.problemTabPage);
      tabControl.TabPages.Remove(this.parametersTabPage);
    }
  }
}
