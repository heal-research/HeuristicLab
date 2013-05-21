#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Abstract base class for data analysis solutions
  /// </summary>
  [StorableClass]
  public abstract class DataAnalysisSolution : ResultCollection, IDataAnalysisSolution {
    private const string ModelResultName = "Model";
    private const string ProblemDataResultName = "ProblemData";

    public string Filename { get; set; }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }

    #region properties
    public IDataAnalysisModel Model {
      get { return (IDataAnalysisModel)this[ModelResultName].Value; }
      protected set {
        if (this[ModelResultName].Value != value) {
          if (value != null) {
            this[ModelResultName].Value = value;
            OnModelChanged();
          }
        }
      }
    }

    public IDataAnalysisProblemData ProblemData {
      get { return (IDataAnalysisProblemData)this[ProblemDataResultName].Value; }
      set {
        if (this[ProblemDataResultName].Value != value) {
          if (value != null) {
            ProblemData.Changed -= new EventHandler(ProblemData_Changed);
            this[ProblemDataResultName].Value = value;
            ProblemData.Changed += new EventHandler(ProblemData_Changed);
            OnProblemDataChanged();
          }
        }
      }
    }
    #endregion

    [StorableConstructor]
    protected DataAnalysisSolution(bool deserializing) : base(deserializing) { }
    protected DataAnalysisSolution(DataAnalysisSolution original, Cloner cloner)
      : base(original, cloner) {
      name = original.Name;
      description = original.Description;
    }
    public DataAnalysisSolution(IDataAnalysisModel model, IDataAnalysisProblemData problemData)
      : base() {
      name = ItemName;
      description = ItemDescription;
      Add(new Result(ModelResultName, "The data analysis model.", model));
      Add(new Result(ProblemDataResultName, "The data analysis problem data.", problemData));

      problemData.Changed += new EventHandler(ProblemData_Changed);
    }

    protected abstract void RecalculateResults();

    private void ProblemData_Changed(object sender, EventArgs e) {
      OnProblemDataChanged();
    }

    public event EventHandler ModelChanged;
    protected virtual void OnModelChanged() {
      RecalculateResults();
      var listeners = ModelChanged;
      if (listeners != null) listeners(this, EventArgs.Empty);
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged() {
      RecalculateResults();
      var listeners = ProblemDataChanged;
      if (listeners != null) listeners(this, EventArgs.Empty);
    }

    #region INamedItem Members
    [Storable]
    protected string name;
    public string Name {
      get { return name; }
      set {
        if (!CanChangeName) throw new NotSupportedException("Name cannot be changed.");
        if (!(name.Equals(value) || (value == null) && (name == string.Empty))) {
          CancelEventArgs<string> e = value == null ? new CancelEventArgs<string>(string.Empty) : new CancelEventArgs<string>(value);
          OnNameChanging(e);
          if (!e.Cancel) {
            name = value == null ? string.Empty : value;
            OnNameChanged();
          }
        }
      }
    }
    public virtual bool CanChangeName {
      get { return true; }
    }
    [Storable]
    protected string description;
    public string Description {
      get { return description; }
      set {
        if (!CanChangeDescription) throw new NotSupportedException("Description cannot be changed.");
        if (!(description.Equals(value) || (value == null) && (description == string.Empty))) {
          description = value == null ? string.Empty : value;
          OnDescriptionChanged();
        }
      }
    }
    public virtual bool CanChangeDescription {
      get { return true; }
    }

    public override string ToString() {
      return Name;
    }

    public event EventHandler<CancelEventArgs<string>> NameChanging;
    protected virtual void OnNameChanging(CancelEventArgs<string> e) {
      var handler = NameChanging;
      if (handler != null) handler(this, e);
    }

    public event EventHandler NameChanged;
    protected virtual void OnNameChanged() {
      var handler = NameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnToStringChanged();
    }

    public event EventHandler DescriptionChanged;
    protected virtual void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
