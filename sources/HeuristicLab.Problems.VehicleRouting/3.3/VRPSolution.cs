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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// Represents a VRP solution which can be visualized in the GUI.
  /// </summary>
  [Item("VRPSolution", "Represents a VRP solution which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class VRPSolution : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
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
    private IVRPEncoding solution;
    public IVRPEncoding Solution {
      get { return solution; }
      set {
        if (solution != value) {
          if (solution != null) DeregisterSolutionEvents();
          solution = value;
          if (solution != null) RegisterSolutionEvents();
          OnSolutionChanged();
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
    [Storable]
    private DoubleValue overload;
    public DoubleValue Overload {
      get { return overload; }
      set {
        if (overload != value) {
          if (overload != null) DeregisterOverloadEvents();
          overload = value;
          if (overload != null) RegisterOverloadEvents();
          OnOverloadChanged();
        }
      }
    }
    [Storable]
    private DoubleValue tardiness;
    public DoubleValue Tardiness {
      get { return tardiness; }
      set {
        if (tardiness != value) {
          if (tardiness != null) DeregisterTardinessEvents();
          tardiness = value;
          if (tardiness != null) RegisterTardinessEvents();
          OnTardinessChanged();
        }
      }
    }
    [Storable]
    private DoubleValue travelTime;
    public DoubleValue TravelTime {
      get { return travelTime; }
      set {
        if (travelTime != value) {
          if (travelTime != null) DeregisterTravelTimeEvents();
          travelTime = value;
          if (travelTime != null) RegisterTravelTimeEvents();
          OnTravelTimeChanged();
        }
      }
    }
    [Storable]
    private DoubleValue vehicleUtilization;
    public DoubleValue VehicleUtilization {
      get { return vehicleUtilization; }
      set {
        if (vehicleUtilization != value) {
          if (vehicleUtilization != null) DeregisterVehicleUtilizationEvents();
          vehicleUtilization = value;
          if (vehicleUtilization != null) RegisterVehicleUtilizationEvents();
          OnVehicleUtilizationChanged();
        }
      }
    }
    [Storable]
    private DoubleMatrix distanceMatrix;
    public DoubleMatrix DistanceMatrix {
      get { return distanceMatrix; }
      set {
        if (distanceMatrix != value) {
          distanceMatrix = value;
        }
      }
    }
    [Storable]
    private BoolValue useDistanceMatrix;
    public BoolValue UseDistanceMatrix {
      get { return useDistanceMatrix; }
      set {
        if (useDistanceMatrix != value) {
          useDistanceMatrix = value;
        }
      }
    }
    [Storable]
    private DoubleArray readyTime;
    public DoubleArray ReadyTime {
      get { return readyTime; }
      set {
        if (readyTime != value) {
          readyTime = value;
        }
      }
    }
    [Storable]
    private DoubleArray dueTime;
    public DoubleArray DueTime {
      get { return dueTime; }
      set {
        if (dueTime != value) {
          dueTime = value;
        }
      }
    }
    [Storable]
    private DoubleArray serviceTime;
    public DoubleArray ServiceTime {
      get { return serviceTime; }
      set {
        if (serviceTime != value) {
          serviceTime = value;
        }
      }
    }

    public VRPSolution() : base() { }

    public VRPSolution(DoubleMatrix coordinates)
      : base() {
      this.coordinates = coordinates;
    }

    public VRPSolution(DoubleMatrix coordinates, IVRPEncoding solution, DoubleValue quality,
      DoubleValue distance, DoubleValue overload, DoubleValue tardiness, DoubleValue travelTime,
      DoubleValue vehicleUtilization, DoubleMatrix distanceMatrix, BoolValue useDistanceMatrix,
      DoubleArray readyTime, DoubleArray dueTime, DoubleArray serviceTime)
      : base() {
      this.coordinates = coordinates;
      this.solution = solution;
      this.quality = quality;
      this.distance = distance;
      this.overload = overload;
      this.tardiness = tardiness;
      this.travelTime = travelTime;
      this.vehicleUtilization = vehicleUtilization;
      this.distanceMatrix = distanceMatrix;
      this.useDistanceMatrix = useDistanceMatrix;
      this.readyTime = readyTime;
      this.dueTime = dueTime;
      this.serviceTime = serviceTime;
      Initialize();
    }
    [StorableConstructor]
    private VRPSolution(bool deserializing) : base(deserializing) { }

    private VRPSolution(VRPSolution original, Cloner cloner)
      : base(original, cloner) {
      coordinates = cloner.Clone(original.coordinates);
      solution = cloner.Clone(original.solution);
      quality = cloner.Clone(original.quality);
      distance = cloner.Clone(original.distance);
      overload = cloner.Clone(original.overload);
      tardiness = cloner.Clone(original.tardiness);
      travelTime = cloner.Clone(original.travelTime);
      vehicleUtilization = cloner.Clone(original.vehicleUtilization);
      distanceMatrix = cloner.Clone(original.distanceMatrix);
      useDistanceMatrix = cloner.Clone(original.useDistanceMatrix);
      readyTime = cloner.Clone(original.readyTime);
      dueTime = cloner.Clone(original.dueTime);
      serviceTime = cloner.Clone(original.serviceTime);
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private void Initialize() {
      if (coordinates != null) RegisterCoordinatesEvents();
      if (solution != null) RegisterSolutionEvents();
      if (quality != null) RegisterQualityEvents();
      if (distance != null) RegisterDistanceEvents();
      if (overload != null) RegisterOverloadEvents();
      if (tardiness != null) RegisterTardinessEvents();
      if (travelTime != null) RegisterTravelTimeEvents();
      if (vehicleUtilization != null) RegisterVehicleUtilizationEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPSolution(this, cloner);
    }

    #region Events
    public event EventHandler CoordinatesChanged;
    private void OnCoordinatesChanged() {
      var changed = CoordinatesChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler SolutionChanged;
    private void OnSolutionChanged() {
      var changed = SolutionChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler DistanceChanged;
    private void OnDistanceChanged() {
      var changed = DistanceChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler OverloadChanged;
    private void OnOverloadChanged() {
      var changed = OverloadChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler TardinessChanged;
    private void OnTardinessChanged() {
      var changed = TardinessChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler TravelTimeChanged;
    private void OnTravelTimeChanged() {
      var changed = TravelTimeChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler VehicleUtilizationChanged;
    private void OnVehicleUtilizationChanged() {
      var changed = VehicleUtilizationChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterCoordinatesEvents() {
      Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset += new EventHandler(Coordinates_Reset);
    }
    private void DeregisterCoordinatesEvents() {
      Coordinates.ItemChanged -= new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
      Coordinates.Reset -= new EventHandler(Coordinates_Reset);
    }
    private void RegisterSolutionEvents() {
      Solution.ToStringChanged += new EventHandler(Solution_ToStringChanged);
    }
    private void DeregisterSolutionEvents() {
      Solution.ToStringChanged -= new EventHandler(Solution_ToStringChanged);
    }
    private void RegisterQualityEvents() {
      Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    }
    private void RegisterDistanceEvents() {
      Distance.ValueChanged += new EventHandler(Distance_ValueChanged);
    }
    private void DeregisterDistanceEvents() {
      Distance.ValueChanged -= new EventHandler(Distance_ValueChanged);
    }
    private void RegisterOverloadEvents() {
      Overload.ValueChanged += new EventHandler(Overload_ValueChanged);
    }
    private void DeregisterOverloadEvents() {
      Overload.ValueChanged -= new EventHandler(Overload_ValueChanged);
    }
    private void RegisterTardinessEvents() {
      Tardiness.ValueChanged += new EventHandler(Tardiness_ValueChanged);
    }
    private void DeregisterTardinessEvents() {
      Tardiness.ValueChanged -= new EventHandler(Tardiness_ValueChanged);
    }
    private void RegisterTravelTimeEvents() {
      TravelTime.ValueChanged += new EventHandler(TravelTime_ValueChanged);
    }
    private void DeregisterTravelTimeEvents() {
      TravelTime.ValueChanged -= new EventHandler(TravelTime_ValueChanged);
    }
    private void RegisterVehicleUtilizationEvents() {
      VehicleUtilization.ValueChanged += new EventHandler(VehicleUtilization_ValueChanged);
    }
    private void DeregisterVehicleUtilizationEvents() {
      VehicleUtilization.ValueChanged -= new EventHandler(VehicleUtilization_ValueChanged);
    }

    private void Coordinates_ItemChanged(object sender, EventArgs<int, int> e) {
      OnCoordinatesChanged();
    }
    private void Coordinates_Reset(object sender, EventArgs e) {
      OnCoordinatesChanged();
    }
    private void Solution_ToStringChanged(object sender, EventArgs e) {
      OnSolutionChanged();
    }
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }
    private void Distance_ValueChanged(object sender, EventArgs e) {
      OnDistanceChanged();
    }
    private void Overload_ValueChanged(object sender, EventArgs e) {
      OnOverloadChanged();
    }
    private void Tardiness_ValueChanged(object sender, EventArgs e) {
      OnTardinessChanged();
    }
    private void TravelTime_ValueChanged(object sender, EventArgs e) {
      OnTravelTimeChanged();
    }
    private void VehicleUtilization_ValueChanged(object sender, EventArgs e) {
      OnVehicleUtilizationChanged();
    }
    #endregion
  }
}
