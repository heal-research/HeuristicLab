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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Orienteering {
  [Item("OrienteeringSolution", "Represents a Orienteering solution which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class OrienteeringSolution : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable]
    private IntegerVector integerVector;
    public IntegerVector IntegerVector {
      get { return integerVector; }
      set {
        if (integerVector != value) {
          if (integerVector != null) DeregisterIntegerVectorEvents();
          integerVector = value;
          if (integerVector != null) RegisterIntegerVectorEvents();
          OnIntegerVectorChanged();
        }
      }
    }
    [Storable]
    private DoubleMatrix coordinates;
    public DoubleMatrix Coordinates {
      get { return coordinates; }
      set {
        if (coordinates != value) {
          if (coordinates != null) DeregisterCoordinatesEvents();
          coordinates = value;
          if (coordinates != null) RegisterCoordinatesEvents();
          OnCoordinatesChanged();
        }
      }
    }
    [Storable]
    private IntValue startingPoint;
    public IntValue StartingPoint {
      get { return startingPoint; }
      set {
        if (startingPoint != value) {
          if (startingPoint != null) DeregisterStartingPointEvents();
          startingPoint = value;
          if (startingPoint != null) RegisterStartingPointEvents();
          OnStartingPointChanged();
        }
      }
    }
    [Storable]
    private IntValue terminalPoint;
    public IntValue TerminalPoint {
      get { return terminalPoint; }
      set {
        if (terminalPoint != value) {
          if (terminalPoint != null) DeregisterTerminalPointEvents();
          terminalPoint = value;
          if (terminalPoint != null) RegisterTerminalPointEvents();
          OnTerminalPointChanged();
        }
      }
    }
    [Storable]
    private DoubleArray scores;
    public DoubleArray Scores {
      get { return scores; }
      set {
        if (scores != value) {
          if (scores != null) DeregisterScoresEvents();
          scores = value;
          if (scores != null) RegisterScoresEvents();
          OnScoresChanged();
        }
      }
    }
    [Storable]
    private DoubleValue quality;
    public DoubleValue Quality {
      get { return quality; }
      set {
        if (quality != value) {
          if (quality != null) DeregisterQualityEvents();
          quality = value;
          if (quality != null) RegisterQualityEvents();
          OnQualityChanged();
        }
      }
    }
    [Storable]
    private DoubleValue penalty;
    public DoubleValue Penalty {
      get { return penalty; }
      set {
        if (penalty != value) {
          if (penalty != null) DeregisterPenaltyEvents();
          penalty = value;
          if (penalty != null) RegisterPenaltyEvents();
          OnPenaltyChanged();
        }
      }
    }
    [Storable]
    private DoubleValue distance;
    public DoubleValue Distance {
      get { return distance; }
      set {
        if (distance != value) {
          if (distance != null) DeregisterDistanceEvents();
          distance = value;
          if (distance != null) RegisterDistanceEvents();
          OnDistanceChanged();
        }
      }
    }

    [StorableConstructor]
    private OrienteeringSolution(bool deserializing)
      : base(deserializing) { }
    private OrienteeringSolution(OrienteeringSolution original, Cloner cloner)
      : base(original, cloner) {
      this.integerVector = cloner.Clone(original.integerVector);
      this.coordinates = cloner.Clone(original.coordinates);
      this.quality = cloner.Clone(original.quality);
      this.penalty = cloner.Clone(original.penalty);
      Initialize();
    }
    public OrienteeringSolution(IntegerVector integerVector, DoubleMatrix coordinates, IntValue startingPoint, IntValue terminalPoint,
      DoubleArray scores, DoubleValue quality = null, DoubleValue penalty = null, DoubleValue distance = null)
      : base() {
      this.integerVector = integerVector;
      this.coordinates = coordinates;
      this.startingPoint = startingPoint;
      this.terminalPoint = terminalPoint;
      this.scores = scores;
      this.quality = quality;
      this.penalty = penalty;
      this.distance = distance;
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringSolution(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      if (integerVector != null) RegisterIntegerVectorEvents();
      if (coordinates != null) RegisterCoordinatesEvents();
      if (startingPoint != null) RegisterStartingPointEvents();
      if (terminalPoint != null) RegisterTerminalPointEvents();
      if (scores != null) RegisterScoresEvents();
      if (quality != null) RegisterQualityEvents();
      if (penalty != null) RegisterPenaltyEvents();
      if (distance != null) RegisterDistanceEvents();
    }

    #region Events
    public event EventHandler IntegerVectorChanged;
    private void OnIntegerVectorChanged() {
      var changed = IntegerVectorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler CoordinatesChanged;
    private void OnCoordinatesChanged() {
      var changed = CoordinatesChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler StartingPointChanged;
    private void OnStartingPointChanged() {
      var changed = StartingPointChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler TerminalPointChanged;
    private void OnTerminalPointChanged() {
      var changed = TerminalPointChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler ScoresChanged;
    private void OnScoresChanged() {
      var changed = ScoresChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler PenaltyChanged;
    private void OnPenaltyChanged() {
      var changed = PenaltyChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler DistanceChanged;
    private void OnDistanceChanged() {
      var changed = DistanceChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterIntegerVectorEvents() {
      IntegerVector.ItemChanged += new EventHandler<EventArgs<int>>(IntegerVector_ItemChanged);
      IntegerVector.Reset += new EventHandler(IntegerVector_Reset);
    }
    private void DeregisterIntegerVectorEvents() {
      IntegerVector.ItemChanged -= new EventHandler<EventArgs<int>>(IntegerVector_ItemChanged);
      IntegerVector.Reset -= new EventHandler(IntegerVector_Reset);
    }
    private void RegisterCoordinatesEvents() {
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
    }
    private void DeregisterCoordinatesEvents() {
      Coordinates.ItemChanged -= new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset -= new EventHandler(Coordinates_Reset);
    }
    private void RegisterStartingPointEvents() {
      StartingPoint.ValueChanged += new EventHandler(StartingPoint_ValueChanged);
    }
    private void DeregisterStartingPointEvents() {
      StartingPoint.ValueChanged -= new EventHandler(StartingPoint_ValueChanged);
    }
    private void RegisterTerminalPointEvents() {
      TerminalPoint.ValueChanged += new EventHandler(TerminalPoint_ValueChanged);
    }
    private void DeregisterTerminalPointEvents() {
      TerminalPoint.ValueChanged -= new EventHandler(TerminalPoint_ValueChanged);
    }
    private void RegisterScoresEvents() {
      Scores.ItemChanged += new EventHandler<EventArgs<int>>(Scores_ItemChanged);
      Scores.Reset += new EventHandler(Scores_Reset);
    }
    private void DeregisterScoresEvents() {
      Scores.ItemChanged -= new EventHandler<EventArgs<int>>(Scores_ItemChanged);
      Scores.Reset -= new EventHandler(Scores_Reset);
    }
    private void RegisterQualityEvents() {
      Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    }
    private void RegisterPenaltyEvents() {
      Penalty.ValueChanged += new EventHandler(Penalty_ValueChanged);
    }
    private void DeregisterPenaltyEvents() {
      Penalty.ValueChanged -= new EventHandler(Penalty_ValueChanged);
    }
    private void RegisterDistanceEvents() {
      Distance.ValueChanged += new EventHandler(Distance_ValueChanged);
    }
    private void DeregisterDistanceEvents() {
      Distance.ValueChanged -= new EventHandler(Distance_ValueChanged);
    }

    private void IntegerVector_ItemChanged(object sender, EventArgs<int> e) {
      OnIntegerVectorChanged();
    }
    private void IntegerVector_Reset(object sender, EventArgs e) {
      OnIntegerVectorChanged();
    }
    private void Coordinates_ItemChanged(object sender, EventArgs<int, int> e) {
      OnCoordinatesChanged();
    }
    private void Coordinates_Reset(object sender, EventArgs e) {
      OnCoordinatesChanged();
    }
    private void StartingPoint_ValueChanged(object sender, EventArgs e) {
      OnStartingPointChanged();
    }
    private void TerminalPoint_ValueChanged(object sender, EventArgs e) {
      OnTerminalPointChanged();
    }
    private void Scores_ItemChanged(object sender, EventArgs<int> e) {
      OnCoordinatesChanged();
    }
    private void Scores_Reset(object sender, EventArgs e) {
      OnCoordinatesChanged();
    }
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }
    private void Penalty_ValueChanged(object sender, EventArgs e) {
      OnPenaltyChanged();
    }
    private void Distance_ValueChanged(object sender, EventArgs e) {
      OnDistanceChanged();
    }
    #endregion
  }
}