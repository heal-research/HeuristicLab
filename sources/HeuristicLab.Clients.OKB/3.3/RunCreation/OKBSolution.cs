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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("OKB Solution", "")]
  [StorableClass]
  public abstract class OKBSolution : Item {
    [Storable]
    private long problemId;
    public long ProblemId {
      get { return problemId; }
      protected set { problemId = value; }
    }

    [Storable]
    private long solutionId;
    public long SolutionId {
      get { return solutionId; }
      protected set { solutionId = value; }
    }
    
    [Storable]
    private IItem solution;
    public IItem Solution {
      get { return solution; }
      set {
        if (solution == value) return;
        solution = value;
        OnSolutionChanged();
      }
    }

    [StorableConstructor]
    protected OKBSolution(bool deserializing) : base(deserializing) { }
    protected OKBSolution(OKBSolution original, Cloner cloner)
      : base(original, cloner) {
      problemId = original.problemId;
      solutionId = original.solutionId;
      solution = cloner.Clone(original.solution);
    }
    protected OKBSolution(long problemId) {
      this.problemId = problemId;
      solutionId = -1;
      solution = null;
    }
    protected OKBSolution(Solution sol) {
      problemId = sol.ProblemId;
      solutionId = sol.Id;
    }

    public abstract void Download(long solutionId);
    public abstract void DownloadData();

    public abstract void Upload();

    public static OKBSolution Convert(Solution solution) {
      if (solution is SingleObjectiveSolution) return new SingleObjectiveOKBSolution(solution);
      throw new ArgumentException("Unknown solution type", "solution");
    }

    public event EventHandler SolutionChanged;
    private void OnSolutionChanged() {
      var handler = SolutionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
