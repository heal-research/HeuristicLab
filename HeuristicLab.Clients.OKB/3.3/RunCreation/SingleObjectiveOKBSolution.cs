#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.Xml;
using System;
using System.IO;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("OKB Solution (single-objective)", "")]
  [StorableClass]
  public sealed class SingleObjectiveOKBSolution : OKBSolution {
    [Storable]
    private double quality;
    public double Quality {
      get { return quality; }
      set {
        if (quality == value) return;
        quality = value;
        OnQualityChanged();
      }
    }

    [StorableConstructor]
    private SingleObjectiveOKBSolution(bool deserializing) : base(deserializing) { }
    private SingleObjectiveOKBSolution(SingleObjectiveOKBSolution original, Cloner cloner)
      : base(original, cloner) {
      quality = original.quality;
    }
    public SingleObjectiveOKBSolution(long problemId) : base(problemId) {
      quality = double.NaN;
    }
    public SingleObjectiveOKBSolution(Solution solution) : base(solution) {
      var soSolution = solution as SingleObjectiveSolution;
      if (soSolution == null) throw new ArgumentException("Solution is not of type SingleObjectiveSolution", "solution");
      Quality = soSolution.Quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveOKBSolution(this, cloner);
    }

    public override void Download(long solutionId) {
      if (SolutionId == solutionId) return;
      byte[] solutionData = RunCreationClient.Instance.GetSolutionData(solutionId);
      using (var stream = new MemoryStream(solutionData)) {
        Solution = XmlParser.Deserialize<IItem>(stream);
        SolutionId = solutionId;
      }
      var sol = RunCreationClient.Instance.GetSolution(solutionId);
      ProblemId = sol.ProblemId;
      var soSol = sol as SingleObjectiveSolution;
      if (soSol == null) throw new InvalidOperationException(string.Format("Solution with id {0} is not a single-objective solution.", solutionId));
      Quality = soSol.Quality;
    }

    public override void DownloadData() {
      using (var stream = new MemoryStream(RunCreationClient.Instance.GetSolutionData(SolutionId))) {
        Solution = XmlParser.Deserialize<IItem>(stream);
      }
    }

    public override void Upload() {
      if (SolutionId != -1) throw new InvalidOperationException("Solution exists already.");
      using (var stream = new MemoryStream()) {
        if (Solution != null) XmlGenerator.Serialize(Solution, stream);
        SolutionId = RunCreationClient.Instance.AddSolution(
          new SingleObjectiveSolution() {
            ProblemId = ProblemId,
            Quality = Quality,
            DataType = Solution != null ? new DataType() {
              Name = Solution.GetType().Name,
              TypeName = Solution.GetType().FullName
            } : null
          }, stream.ToArray());
      }
    }

    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var handler = QualityChanged;
      if  (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
