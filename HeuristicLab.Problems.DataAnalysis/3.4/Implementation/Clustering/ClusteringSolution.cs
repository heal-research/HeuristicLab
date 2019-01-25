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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents a clustering data analysis solution
  /// </summary>
  [StorableType("51241A39-F5C6-4DC8-A27A-83875FFCB51B")]
  public class ClusteringSolution : DataAnalysisSolution, IClusteringSolution {

    [StorableConstructor]
    protected ClusteringSolution(StorableConstructorFlag _) : base(_) { }
    protected ClusteringSolution(ClusteringSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public ClusteringSolution(IClusteringModel model, IClusteringProblemData problemData)
      : base(model, problemData) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClusteringSolution(this, cloner);
    }

    protected override void RecalculateResults() {
    }

    #region IClusteringSolution Members

    public new IClusteringModel Model {
      get { return (IClusteringModel)base.Model; }
      set { base.Model = value; }
    }

    public new IClusteringProblemData ProblemData {
      get { return (IClusteringProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    public virtual IEnumerable<int> ClusterValues {
      get {
        return GetClusterValues(Enumerable.Range(0, ProblemData.Dataset.Rows));
      }
    }

    public virtual IEnumerable<int> TrainingClusterValues {
      get {
        return GetClusterValues(ProblemData.TrainingIndices);
      }
    }

    public virtual IEnumerable<int> TestClusterValues {
      get {
        return GetClusterValues(ProblemData.TestIndices);
      }
    }

    public virtual IEnumerable<int> GetClusterValues(IEnumerable<int> rows) {
      return Model.GetClusterValues(ProblemData.Dataset, rows);
    }
    #endregion
  }
}
