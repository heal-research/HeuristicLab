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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprocessingContext", "PreprocessingContext")]
  [StorableClass]
  public class PreprocessingContext : NamedItem, IStorableContent {
    public string Filename { get; set; }

    [Storable]
    public IFilteredPreprocessingData Data { get; private set; }

    [Storable]
    private IItem Source { get; set; }

    public event EventHandler Reset;

    #region Constructors
    public PreprocessingContext(IDataAnalysisProblemData problemData, IItem source = null)
      : base("Data Preprocessing") {
      if (problemData == null) throw new ArgumentNullException("problemData");
      Import(problemData, source);
    }

    [StorableConstructor]
    protected PreprocessingContext(bool deserializing)
      : base(deserializing) { }
    protected PreprocessingContext(PreprocessingContext original, Cloner cloner)
      : base(original, cloner) {
      Source = cloner.Clone(original.Source);
      Data = cloner.Clone(original.Data);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingContext(this, cloner);
    }
    #endregion

    #region Import
    public void Import(IDataAnalysisProblemData problemData, IItem source = null) {
      if (problemData == null) throw new ArgumentNullException("problemData");
      if (source != null && ExtractProblemData(source) != problemData)
        source = null; // ignore the source if the source's problem data is different
      Source = source ?? problemData;
      var namedSource = Source as INamedItem;
      if (namedSource != null)
        Name = "Preprocessing " + namedSource.Name;
      Data = new FilteredPreprocessingData(new PreprocessingData(problemData));
      OnReset();
      // Reset GUI:
      // - OnContentChanged for PreprocessingView!
      // event? task(async import)?
    }
    private IDataAnalysisProblemData ExtractProblemData(IItem source) {
      var algorithm = source as Algorithm;
      var problem = algorithm != null ? algorithm.Problem as IDataAnalysisProblem : source as IDataAnalysisProblem;
      var problemData = problem != null ? problem.ProblemData : source as IDataAnalysisProblemData;
      return problemData;
    }
    #endregion

    #region Export
    public bool CanExport {
      get { return Source is IAlgorithm || Source is IDataAnalysisProblem || Source is IDataAnalysisProblemData; }
    }
    public IEnumerable<KeyValuePair<string, Func<IItem>>> GetSourceExportOptions() {
      var algorithm = Source as IAlgorithm;
      if (algorithm != null)
        yield return new KeyValuePair<string, Func<IItem>>(
          algorithm.GetType().GetPrettyName(),
          () => ExportAlgorithm(algorithm));

      var problem = algorithm != null ? algorithm.Problem as IDataAnalysisProblem : Source as IDataAnalysisProblem;
      if (problem != null)
        yield return new KeyValuePair<string, Func<IItem>>(
          problem.GetType().GetPrettyName(),
          () => ExportProblem(problem));

      var problemData = problem != null ? problem.ProblemData : Source as IDataAnalysisProblemData;
      if (problemData != null)
        yield return new KeyValuePair<string, Func<IItem>>(
          problemData.GetType().GetPrettyName(),
          () => ExportProblemData(problemData));
    }

    private IAlgorithm ExportAlgorithm(IAlgorithm source) {
      var preprocessedAlgorithm = (IAlgorithm)source.Clone();
      preprocessedAlgorithm.Name = preprocessedAlgorithm.Name + "(Preprocessed)";
      preprocessedAlgorithm.Runs.Clear();
      var problem = (IDataAnalysisProblem)preprocessedAlgorithm.Problem;
      problem.ProblemDataParameter.ActualValue = CreateNewProblemData();
      return preprocessedAlgorithm;
    }
    private IDataAnalysisProblem ExportProblem(IDataAnalysisProblem source) {
      var preprocessedProblem = (IDataAnalysisProblem)source.Clone();
      preprocessedProblem.ProblemDataParameter.ActualValue = CreateNewProblemData();
      return preprocessedProblem;
    }
    private IDataAnalysisProblemData ExportProblemData(IDataAnalysisProblemData source) {
      return CreateNewProblemData();
    }

    public IDataAnalysisProblemData CreateNewProblemData() {
      var creator = new ProblemDataCreator(this);
      var oldProblemData = ExtractProblemData(Source);
      var newProblemData = creator.CreateProblemData(oldProblemData);
      newProblemData.Name = "Preprocessed " + oldProblemData.Name;
      return newProblemData;
    }
    #endregion

    protected virtual void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
    }
  }
}
