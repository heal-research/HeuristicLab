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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Persistence.Default.Xml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("OKB Problem", "A base class for problems which are stored in the OKB.")]
  [StorableType("041DC8A8-2987-4045-B24A-CBA7EAD47316")]
  public abstract class OKBProblem : Item, IHeuristicOptimizationProblem {
    public virtual Type ProblemType {
      get { return typeof(IHeuristicOptimizationProblem); }
    }
    private long problemId;
    public long ProblemId {
      get { return problemId; }
    }
    private IHeuristicOptimizationProblem problem;
    protected IHeuristicOptimizationProblem Problem {
      get { return problem; }
      private set {
        if (value == null) throw new ArgumentNullException("Problem", "Problem cannot be null.");
        if (value != problem) {
          CancelEventArgs<string> e = new CancelEventArgs<string>(value.Name);
          OnNameChanging(e);
          if (!e.Cancel) {
            DeregisterProblemEvents();
            problem = value;
            RegisterProblemEvents();

            OnToStringChanged();
            OnItemImageChanged();
            OnNameChanged();
            OnDescriptionChanged();
            OnProblemChanged();
            OnSolutionCreatorChanged();
            OnEvaluatorChanged();
            OnOperatorsChanged();
            OnReset();

            solutions.Clear();
          }
        }
      }
    }

    [Storable]
    private ItemList<OKBSolution> solutions; 
    public ItemList<OKBSolution> Solutions {
      get { return solutions; }
    }

    public override Image ItemImage {
      get { return Problem.ItemImage; }
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    public string Name {
      get { return Problem.Name; }
      set { throw new NotSupportedException("Name cannot be changed."); }
    }
    public string Description {
      get { return Problem.Description; }
      set { throw new NotSupportedException("Description cannot be changed."); }
    }
    public bool CanChangeName {
      get { return false; }
    }
    public bool CanChangeDescription {
      get { return false; }
    }

    public IKeyedItemCollection<string, IParameter> Parameters {
      get { return Problem.Parameters; }
    }

    public IParameter SolutionCreatorParameter {
      get { return Problem.SolutionCreatorParameter; }
    }
    public ISolutionCreator SolutionCreator {
      get { return Problem.SolutionCreator; }
    }
    public IParameter EvaluatorParameter {
      get { return Problem.EvaluatorParameter; }
    }
    public IEvaluator Evaluator {
      get { return Problem.Evaluator; }
    }
    public IEnumerable<IItem> Operators {
      get { return Problem.Operators; }
    }

    public IEnumerable<IParameterizedItem> ExecutionContextItems { get { return new[] { this }; } }

    #region Persistence Properties
    [Storable(Name = "ProblemId")]
    private long StorableProblemId {
      get { return problemId; }
      set { problemId = value; }
    }
    [Storable(Name = "Problem")]
    private IHeuristicOptimizationProblem StorableProblem {
      get { return problem; }
      set {
        problem = value;
        RegisterProblemEvents();
      }
    }
    #endregion

    [StorableConstructor]
    protected OKBProblem(StorableConstructorFlag _) : base(_) { }
    protected OKBProblem(OKBProblem original, Cloner cloner)
      : base(original, cloner) {
      problemId = original.problemId;
      problem = cloner.Clone(original.problem);
      solutions = cloner.Clone(original.solutions);
      RegisterProblemEvents();
    }
    protected OKBProblem(IHeuristicOptimizationProblem initialProblem)
      : base() {
      if (initialProblem == null) throw new ArgumentNullException("initialProblem", "Initial problem cannot be null.");
      problemId = -1;
      problem = initialProblem;
      solutions = new ItemList<OKBSolution>();
      RegisterProblemEvents();
    }

    public void Load(long problemId) {
      if (this.problemId != problemId) {
        IHeuristicOptimizationProblem problem;
        byte[] problemData = RunCreationClient.Instance.GetProblemData(problemId);
        using (MemoryStream stream = new MemoryStream(problemData)) {
          problem = XmlParser.Deserialize<IHeuristicOptimizationProblem>(stream);
        }
        if (ProblemType.IsAssignableFrom(problem.GetType())) {
          this.problemId = problemId;
          Problem = problem;
        }
      }
    }

    public void RefreshSolutions() {
      if (ProblemId != -1) {
        var sols = RunCreationClient.Instance.GetSolutions(ProblemId).Select(OKBSolution.Convert).ToList();
        foreach (var sol in sols) sol.DownloadData();
        Solutions.Replace(sols);
      } else Solutions.Clear();
    }

    public IProblem CloneProblem() {
      return (IProblem)Problem.Clone();
    }

    public void CollectParameterValues(IDictionary<string, IItem> values) {
      Problem.CollectParameterValues(values);
    }

    #region Events
    public event EventHandler ProblemChanged;
    protected virtual void OnProblemChanged() {
      EventHandler handler = ProblemChanged;
      if (handler != null) handler(this, EventArgs.Empty);
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
    }
    public event EventHandler DescriptionChanged;
    protected virtual void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler SolutionCreatorChanged;
    protected virtual void OnSolutionCreatorChanged() {
      var handler = SolutionCreatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    protected virtual void OnEvaluatorChanged() {
      var handler = EvaluatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OperatorsChanged;
    protected virtual void OnOperatorsChanged() {
      var handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Reset;
    protected virtual void OnReset() {
      var handler = Reset;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    protected virtual void RegisterProblemEvents() {
      if (Problem != null) {
        Problem.ToStringChanged += new EventHandler(Problem_ToStringChanged);
        Problem.ItemImageChanged += new EventHandler(Problem_ItemImageChanged);
        Problem.NameChanging += new EventHandler<CancelEventArgs<string>>(Problem_NameChanging);
        Problem.NameChanged += new EventHandler(Problem_NameChanged);
        Problem.DescriptionChanged += new EventHandler(Problem_DescriptionChanged);
        Problem.SolutionCreatorChanged += new EventHandler(Problem_SolutionCreatorChanged);
        Problem.EvaluatorChanged += new EventHandler(Problem_EvaluatorChanged);
        Problem.OperatorsChanged += new EventHandler(Problem_OperatorsChanged);
        Problem.Reset += new EventHandler(Problem_Reset);
      }
    }
    protected virtual void DeregisterProblemEvents() {
      if (Problem != null) {
        Problem.ToStringChanged -= new EventHandler(Problem_ToStringChanged);
        Problem.ItemImageChanged -= new EventHandler(Problem_ItemImageChanged);
        Problem.NameChanging -= new EventHandler<CancelEventArgs<string>>(Problem_NameChanging);
        Problem.NameChanged -= new EventHandler(Problem_NameChanged);
        Problem.DescriptionChanged -= new EventHandler(Problem_DescriptionChanged);
        Problem.SolutionCreatorChanged -= new EventHandler(Problem_SolutionCreatorChanged);
        Problem.EvaluatorChanged -= new EventHandler(Problem_EvaluatorChanged);
        Problem.OperatorsChanged -= new EventHandler(Problem_OperatorsChanged);
        Problem.Reset -= new EventHandler(Problem_Reset);
      }
    }

    private void Problem_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
    private void Problem_ItemImageChanged(object sender, EventArgs e) {
      OnItemImageChanged();
    }
    private void Problem_NameChanging(object sender, CancelEventArgs<string> e) {
      OnNameChanging(e);
    }
    private void Problem_NameChanged(object sender, EventArgs e) {
      OnNameChanged();
    }
    private void Problem_DescriptionChanged(object sender, EventArgs e) {
      OnDescriptionChanged();
    }
    private void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged();
    }
    private void Problem_EvaluatorChanged(object sender, EventArgs e) {
      OnEvaluatorChanged();
    }
    private void Problem_OperatorsChanged(object sender, EventArgs e) {
      OnOperatorsChanged();
    }
    private void Problem_Reset(object sender, EventArgs e) {
      OnReset();
    }
    #endregion
  }
}
