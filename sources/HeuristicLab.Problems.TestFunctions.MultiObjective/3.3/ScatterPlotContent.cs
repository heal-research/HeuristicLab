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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableClass]
  [Item("ScatterPlot", "The optimal front, current front and its associated Points in the searchspace")]
  public class ScatterPlotContent : Item {

    [Storable]
    private double[][] qualities;
    public double[][] Qualities {
      get {
        return qualities;
      }

      private set {
        qualities = value;
      }
    }

    [Storable]
    private int objectives;
    public int Objectives {
      get {
        return objectives;
      }

      private set {
        objectives = value;
      }
    }

    [Storable]
    private double[][] solutions;
    public double[][] Solutions {
      get {
        return solutions;
      }

      private set {
        solutions = value;
      }
    }

    [Storable]
    private double[][] paretoFront;
    public double[][] ParetoFront {
      get {
        return paretoFront;
      }

      private set {
        paretoFront = value;
      }
    }

    [StorableConstructor]
    protected ScatterPlotContent(bool deserializing) : base() { }

    protected ScatterPlotContent(ScatterPlotContent original, Cloner cloner)
      : this() {
      this.qualities = original.qualities.Select(s => s.ToArray()).ToArray();
      this.solutions = original.solutions.Select(s => s.ToArray()).ToArray();
      this.paretoFront = original.paretoFront.Select(s => s.ToArray()).ToArray();
      this.objectives = original.objectives;
    }
    protected ScatterPlotContent() : base() { }
    public ScatterPlotContent(double[][] qualities, double[][] solutions, double[][] paretoFront, int objectives) {
      this.qualities = qualities;
      this.solutions = solutions;
      this.paretoFront = paretoFront;
      this.objectives = objectives;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotContent(this, cloner);
    }
  }
}
