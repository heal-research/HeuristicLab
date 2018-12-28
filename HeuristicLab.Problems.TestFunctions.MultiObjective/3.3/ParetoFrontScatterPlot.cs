#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("3BF7AD0E-8D55-4033-974A-01DB16F9E41A")]
  [Item("Pareto Front Scatter Plot", "The optimal front, current front and its associated Points in the searchspace")]
  public class ParetoFrontScatterPlot : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    [Storable]
    private int objectives;
    public int Objectives {
      get { return objectives; }
    }

    [Storable]
    private int problemSize;
    public int ProblemSize {
      get { return problemSize; }
    }

    [Storable]
    private double[][] qualities;
    public double[][] Qualities {
      get { return qualities; }
    }

    [Storable]
    private double[][] solutions;
    public double[][] Solutions {
      get { return solutions; }
    }

    [Storable]
    private double[][] paretoFront;
    public double[][] ParetoFront {
      get { return paretoFront; }
    }

    #region Constructor, Cloning & Persistance
    public ParetoFrontScatterPlot(double[][] qualities, double[][] solutions, double[][] paretoFront, int objectives, int problemSize) {
      this.qualities = qualities;
      this.solutions = solutions;
      this.paretoFront = paretoFront;
      this.objectives = objectives;
      this.problemSize = problemSize;
    }
    public ParetoFrontScatterPlot() { }

    protected ParetoFrontScatterPlot(ParetoFrontScatterPlot original, Cloner cloner)
      : base(original, cloner) {
      if (original.qualities != null) qualities = original.qualities.Select(s => s.ToArray()).ToArray();
      if (original.solutions != null) solutions = original.solutions.Select(s => s.ToArray()).ToArray();
      if (original.paretoFront != null) paretoFront = original.paretoFront.Select(s => s.ToArray()).ToArray();
      objectives = original.objectives;
      problemSize = original.problemSize;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ParetoFrontScatterPlot(this, cloner);
    }

    [StorableConstructor]
    protected ParetoFrontScatterPlot(StorableConstructorFlag _) : base(_) { }
    #endregion
  }
}
