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

namespace HeuristicLab.DataAnalysis {
  public sealed class Dataset : ItemBase {

    private string name;
    private double[] samples;
    private int rows;
    private int columns;
    private Dictionary<int, Dictionary<int, double>>[] cachedMeans;
    private Dictionary<int, Dictionary<int, double>>[] cachedRanges;
    private double[] scalingFactor;
    private double[] scalingOffset;

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
      set { columns = value; }
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
      if(v != samples[columns * i + j]) {
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
        for(int i = 0; i < scalingFactor.Length; i++) {
          scalingFactor[i] = 1.0;
          scalingOffset[i] = 0.0;
        }
        samples = value;
        CreateDictionaries();
        FireChanged();
      }
    }

    private string[] variableNames;
    public string[] VariableNames {
      get { return variableNames; }
      set { variableNames = value; }
    }

    public Dataset() {
      Name = "-";
      VariableNames = new string[] { "Var0" };
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
      for(int i = 0; i < columns; i++) {
        cachedMeans[i] = new Dictionary<int, Dictionary<int, double>>();
        cachedRanges[i] = new Dictionary<int, Dictionary<int, double>>();
      }
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
      clone.VariableNames = new string[VariableNames.Length];
      Array.Copy(VariableNames, clone.VariableNames, VariableNames.Length);
      Array.Copy(scalingFactor, clone.scalingFactor, columns);
      Array.Copy(scalingOffset, clone.scalingOffset, columns);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute problemName = document.CreateAttribute("Name");
      problemName.Value = Name;
      node.Attributes.Append(problemName);
      XmlAttribute dim1 = document.CreateAttribute("Dimension1");
      dim1.Value = rows.ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim1);
      XmlAttribute dim2 = document.CreateAttribute("Dimension2");
      dim2.Value = columns.ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim2);
      XmlAttribute variableNames = document.CreateAttribute("VariableNames");
      variableNames.Value = GetVariableNamesString();
      node.Attributes.Append(variableNames);
      XmlAttribute scalingFactorsAttribute = document.CreateAttribute("ScalingFactors");
      scalingFactorsAttribute.Value = GetString(scalingFactor);
      node.Attributes.Append(scalingFactorsAttribute);
      XmlAttribute scalingOffsetsAttribute = document.CreateAttribute("ScalingOffsets");
      scalingOffsetsAttribute.Value = GetString(scalingOffset);
      node.Attributes.Append(scalingOffsetsAttribute);
      node.InnerText = ToString(CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Name = node.Attributes["Name"].Value;
      rows = int.Parse(node.Attributes["Dimension1"].Value, CultureInfo.InvariantCulture.NumberFormat);
      columns = int.Parse(node.Attributes["Dimension2"].Value, CultureInfo.InvariantCulture.NumberFormat);

      VariableNames = ParseVariableNamesString(node.Attributes["VariableNames"].Value);
      if(node.Attributes["ScalingFactors"] != null)
        scalingFactor = ParseDoubleString(node.Attributes["ScalingFactors"].Value);
      else {
        scalingFactor = new double[columns]; // compatibility with old serialization format
        for(int i = 0; i < scalingFactor.Length; i++) scalingFactor[i] = 1.0;
      }
      if(node.Attributes["ScalingOffsets"] != null)
        scalingOffset = ParseDoubleString(node.Attributes["ScalingOffsets"].Value);
      else {
        scalingOffset = new double[columns]; // compatibility with old serialization format
        for(int i = 0; i < scalingOffset.Length; i++) scalingOffset[i] = 0.0;
      }

      string[] tokens = node.InnerText.Split(';');
      if(tokens.Length != rows * columns) throw new FormatException();
      samples = new double[rows * columns];
      for(int row = 0; row < rows; row++) {
        for(int column = 0; column < columns; column++) {
          if(double.TryParse(tokens[row * columns + column], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out samples[row * columns + column]) == false) {
            throw new FormatException("Can't parse " + tokens[row * columns + column] + " as double value.");
          }
        }
      }
      CreateDictionaries();
    }

    public override string ToString() {
      return ToString(CultureInfo.CurrentCulture.NumberFormat);
    }

    private string ToString(NumberFormatInfo format) {
      StringBuilder builder = new StringBuilder();
      for(int row = 0; row < rows; row++) {
        for(int column = 0; column < columns; column++) {
          builder.Append(";");
          builder.Append(samples[row * columns + column].ToString("r", format));
        }
      }
      if(builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }

    private string GetVariableNamesString() {
      string s = "";
      for(int i = 0; i < variableNames.Length; i++) {
        s += variableNames[i] + "; ";
      }

      if(variableNames.Length > 0) {
        s = s.TrimEnd(';', ' ');
      }
      return s;
    }
    private string GetString(double[] xs) {
      string s = "";
      for(int i = 0; i < xs.Length; i++) {
        s += xs[i].ToString("r", CultureInfo.InvariantCulture) + "; ";
      }

      if(xs.Length > 0) {
        s = s.TrimEnd(';', ' ');
      }
      return s;
    }

    private string[] ParseVariableNamesString(string p) {
      p = p.Trim();
      string[] tokens = p.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
      return tokens;
    }
    private double[] ParseDoubleString(string s) {
      s = s.Trim();
      string[] ss = s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
      double[] xs = new double[ss.Length];
      for(int i = 0; i < xs.Length; i++) {
        xs[i] = double.Parse(ss[i], CultureInfo.InvariantCulture);
      }
      return xs;
    }

    public double GetMean(int column) {
      return GetMean(column, 0, Rows - 1);
    }

    public double GetMean(int column, int from, int to) {
      if(!cachedMeans[column].ContainsKey(from) || !cachedMeans[column][from].ContainsKey(to)) {
        double[] values = new double[to - from + 1];
        for(int sample = from; sample <= to; sample++) {
          values[sample - from] = GetValue(sample, column);
        }
        double mean = Statistics.Mean(values);
        if(!cachedMeans[column].ContainsKey(from)) cachedMeans[column][from] = new Dictionary<int, double>();
        cachedMeans[column][from][to] = mean;
        return mean;
      } else {
        return cachedMeans[column][from][to];
      }
    }

    public double GetRange(int column) {
      return GetRange(column, 0, Rows - 1);
    }

    public double GetRange(int column, int from, int to) {
      if(!cachedRanges[column].ContainsKey(from) || !cachedRanges[column][from].ContainsKey(to)) {
        double[] values = new double[to - from + 1];
        for(int sample = from; sample <= to; sample++) {
          values[sample - from] = GetValue(sample, column);
        }
        double range = Statistics.Range(values);
        if(!cachedRanges[column].ContainsKey(from)) cachedRanges[column][from] = new Dictionary<int, double>();
        cachedRanges[column][from][to] = range;
        return range;
      } else {
        return cachedRanges[column][from][to];
      }
    }

    public double GetMaximum(int column) {
      double max = Double.NegativeInfinity;
      for(int i = 0; i < Rows; i++) {
        double val = GetValue(i, column);
        if(val > max) max = val;
      }
      return max;
    }

    public double GetMinimum(int column) {
      double min = Double.PositiveInfinity;
      for(int i = 0; i < Rows; i++) {
        double val = GetValue(i, column);
        if(val < min) min = val;
      }
      return min;
    }

    internal void ScaleVariable(int column) {
      if(scalingFactor[column] == 1.0 && scalingOffset[column] == 0.0) {
        double min = GetMinimum(column);
        double max = GetMaximum(column);
        double range = max - min;
        if(range == 0) ScaleVariable(column, 1.0, -min);
        else ScaleVariable(column, 1.0 / range, -min);
      }
      CreateDictionaries();
      FireChanged();
    }

    internal void ScaleVariable(int column, double factor, double offset) {
      scalingFactor[column] = factor;
      scalingOffset[column] = offset;
      for(int i = 0; i < Rows; i++) {
        double origValue = samples[i * columns + column];
        samples[i * columns + column] = (origValue + offset) * factor;
      }
      CreateDictionaries();
      FireChanged();
    }

    internal void UnscaleVariable(int column) {
      if(scalingFactor[column] != 1.0 || scalingOffset[column]!=0.0) {
        for(int i = 0; i < rows; i++) {
          double scaledValue = samples[i * columns + column];
          samples[i * columns + column] = scaledValue / scalingFactor[column] - scalingOffset[column];
        }
        scalingFactor[column] = 1.0;
        scalingOffset[column] = 0.0;
      }
    }
  }
}
