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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using HeuristicLab.Clients.Access;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Fossil;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("OKB Run", "The parameters and results of an algorithm run which are stored in the OKB.")]
  [StorableType("357B418B-4384-48EC-AAB3-F3008A7CD961")]
  public sealed class OKBRun : NamedItemWrapper<IRun>, IRun, IStorableContent {
    public string Filename { get; set; }

    public override Image ItemImage {
      get { return Stored ? HeuristicLab.Common.Resources.VSImageLibrary.Database : HeuristicLab.Common.Resources.VSImageLibrary.DatabaseModified; }
    }

    private long algorithmId;
    private long problemId;
    private DateTime createdDate;

    private bool stored;
    public bool Stored {
      get { return stored; }
      private set {
        if (value != stored) {
          stored = value;
          OnPropertyChanged("Stored");
          OnItemImageChanged();
        }
      }
    }

    public IAlgorithm Algorithm {
      get { return WrappedItem.Algorithm; }
    }
    public IObservableDictionary<string, IItem> Parameters {
      get { return WrappedItem.Parameters; }
    }
    public IObservableDictionary<string, IItem> Results {
      get { return WrappedItem.Results; }
    }
    public IRun WrappedRun {
      get { return WrappedItem; }
    }

    public Color Color {
      get { return WrappedItem.Color; }
      set { WrappedItem.Color = value; }
    }
    public bool Visible {
      get { return WrappedItem.Visible; }
      set { WrappedItem.Visible = value; }
    }

    #region Persistence Properties
    [Storable]
    private Guid UserId;

    [Storable]
    private Guid ClientId;

    [Storable(Name = "Stored")]
    private bool StorableStored {
      get { return stored; }
      set { stored = value; }
    }
    [Storable(Name = "AlgorithmId")]
    private long StorableAlgorithmId {
      get { return algorithmId; }
      set { algorithmId = value; }
    }
    [Storable(Name = "ProblemId")]
    private long StorableProblemId {
      get { return problemId; }
      set { problemId = value; }
    }
    [Storable(Name = "CreatedDate")]
    private DateTime StorableCreatedDate {
      get { return createdDate; }
      set { createdDate = value; }
    }
    #endregion

    [StorableConstructor]
    private OKBRun(StorableConstructorFlag _) : base(_) { }
    private OKBRun(OKBRun original, Cloner cloner)
      : base(original, cloner) {
      algorithmId = original.algorithmId;
      problemId = original.problemId;
      createdDate = original.createdDate;
      stored = original.stored;
      UserId = original.UserId;
      ClientId = original.ClientId;
    }
    public OKBRun(long algorithmId, long problemId, IRun run, Guid userId)
      : base(run) {
      this.algorithmId = algorithmId;
      this.problemId = problemId;
      this.createdDate = DateTime.Now;
      this.stored = false;
      this.UserId = userId;
      if (ClientInformation.Instance.ClientExists) {
        this.ClientId = ClientInformation.Instance.ClientInfo.Id;
      } else {
        this.ClientId = Guid.Empty;
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OKBRun(this, cloner);
    }

    public void Store() {
      if (Stored) throw new InvalidOperationException("Cannot store already stored run.");
      if (!ClientInformation.Instance.ClientExists) {
        throw new MissingClientRegistrationException();
      }

      //if user has now registered his client...
      if (ClientId == Guid.Empty) {
        ClientId = ClientInformation.Instance.ClientInfo.Id;
      }

      Run run = new Run();
      run.AlgorithmId = algorithmId;
      run.ProblemId = problemId;
      run.UserId = UserId;
      run.ClientId = ClientId;
      run.CreatedDate = createdDate;
      run.ParameterValues = ConvertToValues(Parameters);
      run.ResultValues = ConvertToValues(Results);
      RunCreationClient.Instance.AddRun(run);

      Stored = true;
    }

    #region Events
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(property));
    }

    protected override void RegisterWrappedItemEvents() {
      base.RegisterWrappedItemEvents();
      WrappedItem.PropertyChanged += WrappedItem_PropertyChanged;
    }
    protected override void DeregisterWrappedItemEvents() {
      WrappedItem.PropertyChanged -= WrappedItem_PropertyChanged;
      base.DeregisterWrappedItemEvents();
    }

    private void WrappedItem_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      OnPropertyChanged(e.PropertyName);
    }
    #endregion

    #region Helpers
    List<Value> ConvertToValues(IDictionary<string, IItem> items) {
      List<Value> values = new List<Value>();
      bool add = true;
      foreach (var item in items) {
        Value value;
        if (item.Value is Data.BoolValue) {
          BoolValue v = new BoolValue();
          v.Value = ((Data.BoolValue)item.Value).Value;
          value = v;
        } else if (item.Value is Data.IntValue) {
          IntValue v = new IntValue();
          v.Value = ((Data.IntValue)item.Value).Value;
          value = v;
        } else if (item.Value is Data.TimeSpanValue) {
          TimeSpanValue v = new TimeSpanValue();
          v.Value = (long)((Data.TimeSpanValue)item.Value).Value.TotalSeconds;
          value = v;
        } else if (item.Value is Data.PercentValue) {
          PercentValue v = new PercentValue();
          v.Value = ((Data.PercentValue)item.Value).Value;
          value = v;
          if (double.IsNaN(v.Value)) {
            add = false;
          }
        } else if (item.Value is Data.DoubleValue) {
          DoubleValue v = new DoubleValue();
          v.Value = ((Data.DoubleValue)item.Value).Value;
          value = v;
          if (double.IsNaN(v.Value)) {
            add = false;
          }
        } else if (item.Value is Data.StringValue) {
          StringValue v = new StringValue();
          v.Value = ((Data.StringValue)item.Value).Value;
          value = v;
        } else {
          BinaryValue v = new BinaryValue();
          using (MemoryStream stream = new MemoryStream()) {
            XmlGenerator.Serialize(item.Value, stream);
            stream.Close();
            v.Value = stream.ToArray();
          }
          value = v;
        }
        if (add) {
          value.Name = item.Key;
          value.DataType = new DataType();
          value.DataType.Name = item.Value.GetType().Name;
          value.DataType.TypeName = item.Value.GetType().AssemblyQualifiedName;
          values.Add(value);
        } else {
          add = true;
        }
      }
      return values;
    }
    #endregion
  }
}
