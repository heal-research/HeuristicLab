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

using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Clients.Access;
using HeuristicLab.Collections;
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
  [Item("OKB Algorithm", "An algorithm which is stored in the OKB.")]
  [Creatable(CreatableAttribute.Categories.TestingAndAnalysisOKB, Priority = 100)]
  [StorableType("159620CA-BC90-48D9-8868-5D8940016BA0")]
  public sealed class OKBAlgorithm : Item, IAlgorithm, IStorableContent {
    public string Filename { get; set; }

    private long algorithmId;
    public long AlgorithmId {
      get { return algorithmId; }
    }
    private IAlgorithm algorithm;
    private IAlgorithm Algorithm {
      get { return algorithm; }
      set {
        if (value == null) throw new ArgumentNullException("Algorithm", "Algorithm cannot be null.");
        if (value != algorithm) {
          CancelEventArgs<string> e = new CancelEventArgs<string>(value.Name);
          OnNameChanging(e);
          if (!e.Cancel) {
            IProblem problem = algorithm.Problem;
            DeregisterAlgorithmEvents();
            algorithm = value;
            RegisterAlgorithmEvents();

            OnToStringChanged();
            OnItemImageChanged();
            OnNameChanged();
            OnDescriptionChanged();
            OnAlgorithmChanged();
            OnStoreAlgorithmInEachRunChanged();

            try {
              algorithm.Problem = problem;
            } catch (ArgumentException) {
              algorithm.Problem = null;
            }
            algorithm.Prepare(true);
          }
        }
      }
    }

    public IEnumerable<IOptimizer> NestedOptimizers {
      get {
        // inner algorithm cannot be accessed directly
        return Enumerable.Empty<IOptimizer>();
      }
    }

    public override Image ItemImage {
      get { return Algorithm.ItemImage; }
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Event; }
    }

    public string Name {
      get { return Algorithm.Name; }
      set { throw new NotSupportedException("Name cannot be changed."); }
    }
    public string Description {
      get { return Algorithm.Description; }
      set { throw new NotSupportedException("Description cannot be changed."); }
    }
    public bool CanChangeName {
      get { return false; }
    }
    public bool CanChangeDescription {
      get { return false; }
    }

    public IKeyedItemCollection<string, IParameter> Parameters {
      get { return Algorithm.Parameters; }
    }

    public ExecutionState ExecutionState {
      get { return Algorithm.ExecutionState; }
    }
    public TimeSpan ExecutionTime {
      get { return Algorithm.ExecutionTime; }
    }

    public Type ProblemType {
      get { return Algorithm.ProblemType; }
    }
    public IProblem Problem {
      get { return Algorithm.Problem; }
      set { Algorithm.Problem = value; }
    }

    public ResultCollection Results {
      get { return Algorithm.Results; }
    }

    private RunCollection runs;
    public RunCollection Runs {
      get { return runs; }
    }
    private bool storeRunsAutomatically;
    public bool StoreRunsAutomatically {
      get { return storeRunsAutomatically; }
      set {
        if (value != storeRunsAutomatically) {
          storeRunsAutomatically = value;
          OnStoreRunsAutomaticallyChanged();
        }
      }
    }
    public bool StoreAlgorithmInEachRun {
      get { return Algorithm.StoreAlgorithmInEachRun; }
      set { Algorithm.StoreAlgorithmInEachRun = value; }
    }

    #region Persistence Properties
    [Storable]
    private Guid UserId;

    [Storable(Name = "AlgorithmId")]
    private long StorableAlgorithmId {
      get { return algorithmId; }
      set { algorithmId = value; }
    }
    [Storable(Name = "Algorithm")]
    private IAlgorithm StorableAlgorithm {
      get { return algorithm; }
      set {
        algorithm = value;
        RegisterAlgorithmEvents();
      }
    }
    [Storable(Name = "Runs")]
    private RunCollection StorableRuns {
      get { return runs; }
      set {
        runs = value;
        RegisterRunsEvents();
      }
    }
    [Storable(Name = "StoreRunsAutomatically")]
    private bool StorableStoreRunsAutomatically {
      get { return storeRunsAutomatically; }
      set { storeRunsAutomatically = value; }
    }
    #endregion

    [StorableConstructor]
    private OKBAlgorithm(StorableConstructorFlag _) : base(_) { }
    private OKBAlgorithm(OKBAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      algorithmId = original.algorithmId;
      algorithm = cloner.Clone(original.algorithm);
      RegisterAlgorithmEvents();
      runs = cloner.Clone(original.runs);
      storeRunsAutomatically = original.storeRunsAutomatically;
      UserId = original.UserId;
      RegisterRunsEvents();
    }
    public OKBAlgorithm()
      : base() {
      algorithmId = -1;
      algorithm = new EmptyAlgorithm("No algorithm selected. Please choose an algorithm from the OKB.");
      RegisterAlgorithmEvents();
      runs = new RunCollection();
      storeRunsAutomatically = true;
      RegisterRunsEvents();
    }

    private void CheckUserPermissions() {
      if (UserInformation.Instance.UserExists) {
        if (UserInformation.Instance.User.Roles.Count(x => x.Name == OKBRoles.OKBUser || x.Name == OKBRoles.OKBAdministrator) > 0) {
          UserId = UserInformation.Instance.User.Id;
        } else {
          throw new Exception("You don't have the appropriate roles for executing OKB Algorithms.");
        }
      } else {
        throw new Exception("You need an user account for executing OKB Algorithms.");
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OKBAlgorithm(this, cloner);
    }

    public void Load(long algorithmId) {
      if (this.algorithmId != algorithmId) {
        IAlgorithm algorithm;
        byte[] algorithmData = RunCreationClient.Instance.GetAlgorithmData(algorithmId);
        using (MemoryStream stream = new MemoryStream(algorithmData)) {
          algorithm = XmlParser.Deserialize<IAlgorithm>(stream);
        }
        this.algorithmId = algorithmId;
        Algorithm = algorithm;
      }
    }

    public IAlgorithm CloneAlgorithm() {
      return (IAlgorithm)Algorithm.Clone();
    }

    public void CollectParameterValues(IDictionary<string, IItem> values) {
      Algorithm.CollectParameterValues(values);
    }
    public void CollectResultValues(IDictionary<string, IItem> values) {
      Algorithm.CollectResultValues(values);
    }

    public void Prepare() {
      Algorithm.Prepare();
    }
    public void Prepare(bool clearRuns) {
      if (clearRuns) runs.Clear();
      Algorithm.Prepare(clearRuns);
    }
    public void Start() {
      Start(CancellationToken.None);
    }
    public void Start(CancellationToken cancellationToken) {
      CheckUserPermissions();
      if (!ClientInformation.Instance.ClientExists && storeRunsAutomatically) {
        throw new MissingClientRegistrationException();
      }
      Algorithm.Start(cancellationToken);
    }
    public async Task StartAsync() { await StartAsync(CancellationToken.None); }
    public async Task StartAsync(CancellationToken cancellationToken) {
      await AsyncHelper.DoAsync(Start, cancellationToken);
    }
    public void Pause() {
      Algorithm.Pause();
    }
    public void Stop() {
      Algorithm.Stop();
    }

    #region Events
    public event EventHandler AlgorithmChanged;
    private void OnAlgorithmChanged() {
      EventHandler handler = AlgorithmChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<CancelEventArgs<string>> NameChanging;
    private void OnNameChanging(CancelEventArgs<string> e) {
      var handler = NameChanging;
      if (handler != null) handler(this, e);
    }
    public event EventHandler NameChanged;
    private void OnNameChanged() {
      var handler = NameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler DescriptionChanged;
    private void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionStateChanged;
    private void OnExecutionStateChanged() {
      var handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    private void OnExecutionTimeChanged() {
      var handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ProblemChanged;
    private void OnProblemChanged() {
      var handler = ProblemChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler StoreRunsAutomaticallyChanged;
    private void OnStoreRunsAutomaticallyChanged() {
      var handler = StoreRunsAutomaticallyChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler StoreAlgorithmInEachRunChanged;
    private void OnStoreAlgorithmInEachRunChanged() {
      var handler = StoreAlgorithmInEachRunChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    private void OnPrepared() {
      var handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      var handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    private void OnPaused() {
      var handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
      var handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccurred(Exception exception) {
      var handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    private void RegisterAlgorithmEvents() {
      if (Algorithm != null) {
        Algorithm.ToStringChanged += new EventHandler(Algorithm_ToStringChanged);
        Algorithm.ItemImageChanged += new EventHandler(Algorithm_ItemImageChanged);
        Algorithm.NameChanging += new EventHandler<CancelEventArgs<string>>(Algorithm_NameChanging);
        Algorithm.NameChanged += new EventHandler(Algorithm_NameChanged);
        Algorithm.DescriptionChanged += new EventHandler(Algorithm_DescriptionChanged);
        Algorithm.ExecutionStateChanged += new EventHandler(Algorithm_ExecutionStateChanged);
        Algorithm.ExecutionTimeChanged += new EventHandler(Algorithm_ExecutionTimeChanged);
        Algorithm.ProblemChanged += new EventHandler(Algorithm_ProblemChanged);
        Algorithm.Runs.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_ItemsAdded);
        Algorithm.StoreAlgorithmInEachRunChanged += new EventHandler(Algorithm_StoreAlgorithmInEachRunChanged);
        Algorithm.Prepared += new EventHandler(Algorithm_Prepared);
        Algorithm.Started += new EventHandler(Algorithm_Started);
        Algorithm.Paused += new EventHandler(Algorithm_Paused);
        Algorithm.Stopped += new EventHandler(Algorithm_Stopped);
        Algorithm.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Algorithm_ExceptionOccurred);
      }
    }
    private void DeregisterAlgorithmEvents() {
      if (Algorithm != null) {
        Algorithm.ToStringChanged -= new EventHandler(Algorithm_ToStringChanged);
        Algorithm.ItemImageChanged -= new EventHandler(Algorithm_ItemImageChanged);
        Algorithm.NameChanging -= new EventHandler<CancelEventArgs<string>>(Algorithm_NameChanging);
        Algorithm.NameChanged -= new EventHandler(Algorithm_NameChanged);
        Algorithm.DescriptionChanged -= new EventHandler(Algorithm_DescriptionChanged);
        Algorithm.ExecutionStateChanged -= new EventHandler(Algorithm_ExecutionStateChanged);
        Algorithm.ExecutionTimeChanged -= new EventHandler(Algorithm_ExecutionTimeChanged);
        Algorithm.ProblemChanged -= new EventHandler(Algorithm_ProblemChanged);
        Algorithm.Runs.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_ItemsAdded);
        Algorithm.StoreAlgorithmInEachRunChanged -= new EventHandler(Algorithm_StoreAlgorithmInEachRunChanged);
        Algorithm.Prepared -= new EventHandler(Algorithm_Prepared);
        Algorithm.Started -= new EventHandler(Algorithm_Started);
        Algorithm.Paused -= new EventHandler(Algorithm_Paused);
        Algorithm.Stopped -= new EventHandler(Algorithm_Stopped);
        Algorithm.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Algorithm_ExceptionOccurred);
      }
    }

    private void Algorithm_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }
    private void Algorithm_ItemImageChanged(object sender, EventArgs e) {
      OnItemImageChanged();
    }
    private void Algorithm_NameChanging(object sender, CancelEventArgs<string> e) {
      OnNameChanging(e);
    }
    private void Algorithm_NameChanged(object sender, EventArgs e) {
      OnNameChanged();
    }
    private void Algorithm_DescriptionChanged(object sender, EventArgs e) {
      OnDescriptionChanged();
    }
    private void Algorithm_ExecutionStateChanged(object sender, EventArgs e) {
      OnExecutionStateChanged();
    }
    private void Algorithm_ExecutionTimeChanged(object sender, EventArgs e) {
      OnExecutionTimeChanged();
    }
    private void Algorithm_ProblemChanged(object sender, EventArgs e) {
      OnProblemChanged();
    }
    private void Algorithm_Runs_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      OKBProblem problem = Problem as OKBProblem;
      foreach (IRun run in e.Items) {
        if (problem != null) {
          OKBRun okbRun = new OKBRun(AlgorithmId, problem.ProblemId, run, UserId);
          runs.Add(okbRun);
          if (StoreRunsAutomatically) {
            okbRun.Store();
          }
        } else {
          runs.Add(run);
        }
      }
    }
    private void Algorithm_StoreAlgorithmInEachRunChanged(object sender, EventArgs e) {
      OnStoreAlgorithmInEachRunChanged();
    }
    private void Algorithm_Prepared(object sender, EventArgs e) {
      OnPrepared();
    }
    private void Algorithm_Started(object sender, EventArgs e) {
      OnStarted();
    }
    private void Algorithm_Paused(object sender, EventArgs e) {
      OnPaused();
    }
    private void Algorithm_Stopped(object sender, EventArgs e) {
      OnStopped();
    }
    private void Algorithm_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }

    private void RegisterRunsEvents() {
      runs.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(runs_ItemsRemoved);
      runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
    }
    private void runs_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (IRun run in e.Items) {
        OKBRun okbRun = run as OKBRun;
        if (okbRun != null)
          Algorithm.Runs.Remove(okbRun.WrappedRun);
        else
          Algorithm.Runs.Remove(run);
      }
    }
    private void Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (IRun run in e.OldItems) {
        OKBRun okbRun = run as OKBRun;
        if (okbRun != null)
          Algorithm.Runs.Remove(okbRun.WrappedRun);
        else
          Algorithm.Runs.Remove(run);
      }
    }
    #endregion
  }
}
