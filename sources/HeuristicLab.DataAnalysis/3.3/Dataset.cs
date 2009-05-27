#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Globalization;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.DataAnalysis {
  public sealed class Dataset : ItemBase {

    [Storable]
    private string name;

    [Storable]
    private int rows;

    [Storable]
    private int columns;

    [Storable]
    private string[] variableNames;

    [Storable]
    private double[] scalingFactor;

    [Storable]
    private double[] scalingOffset;

    [Storable]
    private double[] samples;

    private Dictionary<int, Dictionary<int, double>>[] cachedMeans;
    private Dictionary<int, Dictionary<int, double>>[] cachedRanges;

    [Storable]
    private object CreateDictionaries_Persistence {
      get { return null; }
      set { CreateDictionaries(); }
    }

    public string Name {
      get { return name; }
      set { name = value; }
    }

    public int Rows {
      get { return rows; }
      set { rows = value; }
    }

    public int Columns {
      get { return columns; }
      set {
        columns = value;
        if (variableNames == null || variableNames.Length != columns) {
          variableNames = new string[columns];
        }
      }
    }

    public double[] ScalingFactor {
      get { return scalingFactor; }
    }
    public double[] ScalingOffset {
      get { return scalingOffset; }
    }

    public double GetValue(int i, int j) {
      return samples[columns * i + j];
    }

    public void SetValue(int i, int j, double v) {
      if (v != samples[columns * i + j]) {
        samples[columns * i + j] = v;
        CreateDictionaries();
        FireChanged();
      }
    }

    public double[] Samples {
      get { return samples; }
      set {
        scalingFactor = new double[columns];
        scalingOffset = new double[columns];
        for (int i = 0; i < scalingFactor.Length; i++) {
          scalingFactor[i] = 1.0;
          scalingOffset[i] = 0.0;
        }
        samples = value;
        CreateDictionaries();
        FireChanged();
      }
    }

    public Dataset() {
      Name = "-";
      variableNames = new string[] { "Var0" };
      Columns = 1;
      Rows = 1;
      Samples = new double[1];
      scalingOffset = new double[] { 0.0 };
      scalingFactor = new double[] { 1.0 };
    }

    private void CreateDictionaries() {
      // keep a means and ranges dictionary for each column (possible target variable) of the dataset.
      cachedMeans = new Dictionary<int, Dictionary<int, double>>[columns];
      cachedRanges = new Dictionary<int, Dictionary<int, double>>[columns];
      for (int i = 0; i < columns; i++) {
        cachedMeans[i] = new Dictionary<int, Dictionary<int, double>>();
        cachedRanges[i] = new Dictionary<int, Dictionary<int, double>>();
      }
    }

    public string GetVariableName(int variableIndex) {
      return variableNames[variableIndex];
    }

    public void SetVariableName(int variableIndex, string name) {
      variableNames[variableIndex] = name;
    }


    public override IView CreateView() {
      return new DatasetView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Dataset clone = new Dataset();
      clonedObjects.Add(Guid, clone);
      double[] cloneSamples = new double[rows * columns];
      Array.Copy(samples, cloneSamples, samples.Length);
      clone.rows = rows;
      clone.columns = columns;
      clone.Samples = cloneSamples;
      clone.Name = Name;
      clone.variableNames = new string[variableNames.Length];
      Array.Copy(variableNames, clone.variableNames, variableNames.Length);
      Array.Copy(scalingFactor, clone.scalingFactor, columns);
      Array.Copy(scalingOffset, clone.scalingOffset, columns);
      return clone;
    }

    public override string ToString() {
      return ToString(CultureInfo.CurrentCulture.NumberFormat);
    }

    private string ToString(NumberFormatInfo format) {
      StringBuilder builder = new StringBuilder();
      for (int row = 0; row < rows; row++) {
        for (int column = 0; column < columns; column++) {
          builder.Append(";");
          builder.Append(samples[row * columns + column].ToString("r", format));
        }
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }

    public double GetMean(int column) {
      return GetMean(column, 0, Rows);
    }

    public double GetMean(int column, int from, int to) {
      if (!cachedMeans[column].ContainsKey(from) || !cachedMeans[column][from].ContainsKey(to)) {
        double[] values = new double[to - from];
        for (int sample = from; sample < to; sample++) {
          values[sample - from] = GetValue(sample, column);
        }
        double mean = Statistics.Mean(values);
        if (!cachedMeans[column].ContainsKey(from)) cachedMeans[column][from] = new Dictionary<int, double>();
        cachedMeans[column][from][to] = mean;
        return mean;
      } else {
        return cachedMeans[column][from][to];
      }
    }

    public double GetRange(int column) {
      return GetRange(column, 0, Rows);
    }

    public double GetRange(int column, int from, int to) {
      if (!cachedRanges[column].ContainsKey(from) || !cachedRanges[column][from].ContainsKey(to)) {
        double[] values = new double[to - from];
        for (int sample = from; sample < to; sample++) {
          values[sample - from] = GetValue(sample, column);
        }
        double range = Statistics.Range(values);
        if (!cachedRanges[column].ContainsKey(from)) cachedRanges[column][from] = new Dictionary<int, double>();
        cachedRanges[column][from][to] = range;
        return range;
      } else {
        return cachedRanges[column][from][to];
      }
    }

    public double GetMaximum(int column) {
      double max = Double.NegativeInfinity;
      for (int i = 0; i < Rows; i++) {
        double val = GetValue(i, column);
        if (!double.IsNaN(val) && val > max) max = val;
      }
      return max;
    }

    public double GetMinimum(int column) {
      double min = Double.PositiveInfinity;
      for (int i = 0; i < Rows; i++) {
        double val = GetValue(i, column);
        if (!double.IsNaN(val) && val < min) min = val;
      }
      return min;
    }

    internal void ScaleVariable(int column) {
      if (scalingFactor[column] == 1.0 && scalingOffset[column] == 0.0) {
        double min = GetMinimum(column);
        double max = GetMaximum(column);
        double range = max - min;
        if (range == 0) ScaleVariable(column, 1.0, -min);
        else ScaleVariable(column, 1.0 / range, -min);
      }
      CreateDictionaries();
      FireChanged();
    }

    internal void ScaleVariable(int column, double factor, double offset) {
      scalingFactor[column] = factor;
      scalingOffset[column] = offset;
      for (int i = 0; i < Rows; i++) {
        double origValue = samples[i * columns + column];
        samples[i * columns + column] = (origValue + offset) * factor;
      }
      CreateDictionaries();
      FireChanged();
    }

    internal void UnscaleVariable(int column) {
      if (scalingFactor[column] != 1.0 || scalingOffset[column] != 0.0) {
        for (int i = 0; i < rows; i++) {
          double scaledValue = samples[i * columns + column];
          samples[i * columns + column] = scaledValue / scalingFactor[column] - scalingOffset[column];
        }
        scalingFactor[column] = 1.0;
        scalingOffset[column] = 0.0;
      }
    }
  }
}
