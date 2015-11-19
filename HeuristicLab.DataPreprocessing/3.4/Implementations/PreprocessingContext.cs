#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.DataPreprocessing.Implementations;
using HeuristicLab.DataPreprocessing.Interfaces;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprocessingContext", "PreprocessingContext")]
  public class PreprocessingContext
    : Item, IPreprocessingContext {


    public IFilteredPreprocessingData Data { get; private set; }

    public IAlgorithm Algorithm { get; private set; }
    public IDataAnalysisProblem Problem { get; private set; }
    public IDataAnalysisProblemData ProblemData { get; private set; }

    private readonly ProblemDataCreator creator;

    public PreprocessingContext(IDataAnalysisProblemData dataAnalysisProblemData, IAlgorithm algorithm, IDataAnalysisProblem problem) {
      var transactionalPreprocessingData = new TransactionalPreprocessingData(dataAnalysisProblemData);
      Data = new FilteredPreprocessingData(transactionalPreprocessingData);

      ProblemData = dataAnalysisProblemData;
      Algorithm = algorithm;
      Problem = problem;

      creator = new ProblemDataCreator(this);
    }

    protected PreprocessingContext(PreprocessingContext original, Cloner cloner)
      : base(original, cloner) {
      Data = cloner.Clone(original.Data);
      Algorithm = original.Algorithm;
      Problem = original.Problem;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingContext(this, cloner);
    }

    public IItem Export() {
      if (Algorithm != null)
        return ExportAlgorithm();
      else if (Problem != null)
        return ExportProblem();
      return ExportProblemData();
    }
    public IAlgorithm ExportAlgorithm() {
      var preprocessedAlgorithm = (IAlgorithm)Algorithm.Clone(new Cloner());
      preprocessedAlgorithm.Name = preprocessedAlgorithm.Name + "(Preprocessed)";
      Algorithm.Runs.Clear();
      var problem = (IDataAnalysisProblem)preprocessedAlgorithm.Problem;
      SetNewProblemData(problem);
      return preprocessedAlgorithm;
    }
    public IDataAnalysisProblem ExportProblem() {
      var preprocessedProblem = (IDataAnalysisProblem)Problem.Clone(new Cloner());
      SetNewProblemData(preprocessedProblem);
      return preprocessedProblem;
    }

    public IDataAnalysisProblemData ExportProblemData() {
      return creator.CreateProblemData();
    }

    private void SetNewProblemData(IDataAnalysisProblem problem) {
      var data = creator.CreateProblemData();
      problem.ProblemDataParameter.ActualValue = data;
      problem.Name = "Preprocessed " + problem.Name;
    }
  }
}
