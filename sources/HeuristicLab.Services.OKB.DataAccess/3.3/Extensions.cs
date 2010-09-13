#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.OKB.DataAccess {
  #region Value Management
  #region Interfaces
  public interface IParameterValue {
    Experiment Experiment { get; set; }
    object Value { get; set; }
  }
  public interface IAlgorithmParameterValue : IParameterValue {
    AlgorithmParameter AlgorithmParameter { get; set; }
  }
  public interface IProblemParameterValue : IParameterValue {
    ProblemParameter ProblemParameter { get; set; }
  }
  public interface IResultValue {
    Result Result { get; set; }
    Run Run { get; set; }
    object Value { get; set; }
  }
  public interface IProblemCharacteristicValue {
    Problem Problem { get; set; }
    ProblemCharacteristic ProblemCharacteristic { get; set; }
    object Value { get; set; }
  }
  #endregion

  #region Algorithm Parameters
  public partial class AlgorithmParameterBlobValue : IAlgorithmParameterValue {
    object IAlgorithmParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (Binary)value; }
    }
  }
  public partial class AlgorithmParameterBoolValue : IAlgorithmParameterValue {
    object IAlgorithmParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (bool)value; }
    }
  }
  public partial class AlgorithmParameterFloatValue : IAlgorithmParameterValue {
    object IAlgorithmParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (double)value; }
    }
  }
  public partial class AlgorithmParameterIntValue : IAlgorithmParameterValue {
    object IAlgorithmParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (int)value; }
    }
  }
  public partial class AlgorithmParameterStringValue : IAlgorithmParameterValue {
    object IAlgorithmParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (string)value; }
    }
  }
  #endregion

  #region Problem Parameters
  public partial class ProblemParameterBlobValue : IProblemParameterValue {
    object IProblemParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (Binary)value; }
    }
  }
  public partial class ProblemParameterBoolValue : IProblemParameterValue {
    object IProblemParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (bool)value; }
    }
  }
  public partial class ProblemParameterFloatValue : IProblemParameterValue {
    object IProblemParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (double)value; }
    }
  }
  public partial class ProblemParameterIntValue : IProblemParameterValue {
    object IProblemParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (int)value; }
    }
  }
  public partial class ProblemParameterStringValue : IProblemParameterValue {
    object IProblemParameterValue.Value {
      get { return this.Value; }
      set { this.Value = (string)value; }
    }
  }
  #endregion

  #region Results
  public partial class ResultBlobValue : IResultValue {
    object IResultValue.Value {
      get { return this.Value; }
      set { this.Value = (Binary)value; }
    }
  }
  public partial class ResultBoolValue : IResultValue {
    object IResultValue.Value {
      get { return this.Value; }
      set { this.Value = (bool)value; }
    }
  }
  public partial class ResultFloatValue : IResultValue {
    object IResultValue.Value {
      get { return this.Value; }
      set { this.Value = (double)value; }
    }
  }
  public partial class ResultIntValue : IResultValue {
    object IResultValue.Value {
      get { return this.Value; }
      set { this.Value = (int)value; }
    }
  }
  public partial class ResultStringValue : IResultValue {
    object IResultValue.Value {
      get { return this.Value; }
      set { this.Value = (string)value; }
    }
  }
  #endregion

  #region Problem Characteristics
  public partial class ProblemCharacteristicIntValue : IProblemCharacteristicValue {
    object IProblemCharacteristicValue.Value {
      get { return this.Value; }
      set { this.Value = (int)value; }
    }
  }
  public partial class ProblemCharacteristicFloatValue : IProblemCharacteristicValue {
    object IProblemCharacteristicValue.Value {
      get { return this.Value; }
      set { this.Value = (double)value; }
    }
  }
  public partial class ProblemCharacteristicStringValue : IProblemCharacteristicValue {
    object IProblemCharacteristicValue.Value {
      get { return this.Value; }
      set { this.Value = (string)value; }
    }
  }
  #endregion

  public partial class Experiment {
    public IQueryable<IAlgorithmParameterValue> AlgorithmParameterValues {
      get {
        return AlgorithmParameterBlobValues
          .AsQueryable()
          .Cast<IAlgorithmParameterValue>()
          .Concat(AlgorithmParameterBoolValues.Cast<IAlgorithmParameterValue>())
          .Concat(AlgorithmParameterFloatValues.Cast<IAlgorithmParameterValue>())
          .Concat(AlgorithmParameterIntValues.Cast<IAlgorithmParameterValue>())
          .Concat(AlgorithmParameterStringValues.Cast<IAlgorithmParameterValue>());
      }
      set {


        if (value == null) return;
        foreach (IParameterValue pv in value) {
          if (pv == null) continue;
          if (pv is IntParameterValue) {
            IntParameterValues.Add(new IntParameterValue() { ParameterId = pv.ParameterId, Value = (int)pv.Value });
          } else if (pv is FloatParameterValue) {
            FloatParameterValues.Add(new FloatParameterValue() { ParameterId = pv.ParameterId, Value = (double)pv.Value });
          } else if (pv is CharParameterValue) {
            CharParameterValues.Add(new CharParameterValue() { ParameterId = pv.ParameterId, Value = (string)pv.Value });
          } else if (pv is OperatorParameterValue) {
            OperatorParameterValues.Add(new OperatorParameterValue() {
              ParameterId = pv.ParameterId,
              Value = (Binary)pv.Value,
              DataTypeId = ((OperatorParameterValue)pv).DataTypeId,
            });
          } else {
            throw new ArgumentException("Invalid Parameter type" + pv.GetType());
          }
        }
      }
    }
  }

  public partial class Run {
    public IQueryable<IResultValue> ResultValues {
      get {
        return IntResultValues
          .AsQueryable()
          .Cast<IResultValue>()
          .Concat(FloatResultValues.Cast<IResultValue>())
          .Concat(CharResultValues.Cast<IResultValue>())
          .Concat(BlobResultValues.Cast<IResultValue>());
      }
      set {
        foreach (IResultValue rv in value) {
          if (rv == null) continue;
          if (rv is IntResultValue) {
            IntResultValues.Add(new IntResultValue() { ResultId = rv.ResultId, Value = (int)rv.Value });
          } else if (rv is FloatResultValue) {
            FloatResultValues.Add(new FloatResultValue() { ResultId = rv.ResultId, Value = (double)rv.Value });
          } else if (rv is CharResultValue) {
            CharResultValues.Add(new CharResultValue() { ResultId = rv.ResultId, Value = (string)rv.Value });
          } else if (rv is BlobResultValue) {
            BlobResultValues.Add(new BlobResultValue() { ResultId = rv.ResultId, Value = (Binary)rv.Value });
          } else {
            throw new ArgumentException("Invalid result value type " + rv.GetType());
          }
        }
      }
    }
  }
  public partial class Result {
    public IQueryable<IResultValue> ResultValues {
      get {
        return IntResultValues
          .AsQueryable()
          .Cast<IResultValue>()
          .Concat(FloatResultValues.Cast<IResultValue>())
          .Concat(CharResultValues.Cast<IResultValue>())
          .Concat(BlobResultValues.Cast<IResultValue>());
      }
    }
  }

  public partial class Problem {
    public IQueryable<IProblemCharacteristicValue> ProblemCharacteristicValues {
      get {
        return IntProblemCharacteristicValues
          .AsQueryable()
          .Cast<IProblemCharacteristicValue>()
          .Concat(FloatProblemCharacteristicValues.Cast<IProblemCharacteristicValue>())
          .Concat(CharProblemCharacteristicValues.Cast<IProblemCharacteristicValue>());
      }
    }
  }
  public partial class ProblemCharacteristic {
    public IQueryable<IProblemCharacteristicValue> ProblemCharacteristicValues {
      get {
        return IntProblemCharacteristicValues
          .AsQueryable()
          .Cast<IProblemCharacteristicValue>()
          .Concat(FloatProblemCharacteristicValues.Cast<IProblemCharacteristicValue>())
          .Concat(CharProblemCharacteristicValues.Cast<IProblemCharacteristicValue>());
      }
    }
  }
  #endregion

  #region Abuse entities to store parameter values for a single (not yet existing experiment)
  public partial class Parameter {
    public IParameterValue ParameterValue {
      get {
        try {
          return IntParameterValues
            .AsQueryable()
            .Cast<IParameterValue>()
            .Concat(FloatParameterValues.Cast<IParameterValue>())
            .Concat(CharParameterValues.Cast<IParameterValue>())
            .Concat(OperatorParameterValues.Cast<IParameterValue>())
            .ToList().Single();
        }
        catch {
          return null;
        }
      }
      set {
        if (value == ParameterValue) // access to ParameterValue ensures that there is a single value
          return;
        if (value == null)
          throw new ArgumentNullException("ParameterValue");
        IParameterValue oldValue = ParameterValue;
        Type t = value.GetType();
        if (oldValue.GetType() != t)
          throw new ArgumentException("cannot assign value with different type");
        if (t == typeof(IntParameterValue)) {
          IntParameterValues.Clear();
          IntParameterValues.Add((IntParameterValue)value);
        } else if (t == typeof(FloatParameterValue)) {
          FloatParameterValues.Clear();
          FloatParameterValues.Add((FloatParameterValue)value);
        } else if (t == typeof(CharParameterValue)) {
          CharParameterValues.Clear();
          CharParameterValues.Add((CharParameterValue)value);
        } else if (t == typeof(OperatorParameterValue)) {
          OperatorParameterValues.Clear();
          OperatorParameterValues.Add((OperatorParameterValue)value);
        } else throw new ArgumentException("invalid parameter value type " + t.Name);
      }
    }
  }
  public partial class Result {
    public IResultValue ResultValue {
      get {
        try {
          return IntResultValues
            .AsQueryable()
            .Cast<IResultValue>()
            .Concat(FloatResultValues.Cast<IResultValue>())
            .Concat(CharResultValues.Cast<IResultValue>())
            .Concat(BlobResultValues.Cast<IResultValue>()).ToList().Single();
        }
        catch {
          return null;
        }
      }
      set {
        if (value == ResultValue)
          return;
        if (value == null)
          throw new ArgumentNullException("ResultValue");
        IResultValue oldValue = ResultValue;
        Type t = value.GetType();
        if (oldValue.GetType() != t)
          throw new ArgumentException("cannot assign value with different type");
        if (t == typeof(IntResultValue)) {
          IntResultValues.Clear();
          IntResultValues.Add((IntResultValue)value);
        } else if (t == typeof(FloatResultValue)) {
          FloatResultValues.Clear();
          FloatResultValues.Add((FloatResultValue)value);
        } else if (t == typeof(CharResultValue)) {
          CharResultValues.Clear();
          CharResultValues.Add((CharResultValue)value);
        } else if (t == typeof(BlobResultValue)) {
          BlobResultValues.Clear();
          BlobResultValues.Add((BlobResultValue)value);
        } else throw new ArgumentException("invalid result value type " + t.Name);
      }
    }
  }
  public partial class Algorithm {
    public IQueryable<Parameter> Parameters {
      get {
        return Algorithm_Parameters.AsQueryable().Select(ap => ap.Parameter);
      }
    }
    public IQueryable<IntParameterValue> IntParameterValues {
      get {
        return Parameters.AsQueryable()
          .Where(p => p.IntParameterValues.Count > 0)
          .Select(p => p.IntParameterValues.Single());
      }
    }
    public IQueryable<FloatParameterValue> FloatParameterValues {
      get {
        return Parameters.AsQueryable()
          .Where(p => p.FloatParameterValues.Count > 0)
          .Select(p => p.FloatParameterValues.Single());
      }
    }
    public IQueryable<CharParameterValue> CharParameterValues {
      get {
        return Parameters.AsQueryable()
          .Where(p => p.CharParameterValues.Count > 0)
          .Select(p => p.CharParameterValues.Single());
      }
    }
    public IQueryable<OperatorParameterValue> OperatorParameterValues {
      get {
        return Parameters.AsQueryable()
          .Where(p => p.OperatorParameterValues.Count > 0)
          .Select(p => p.OperatorParameterValues.Single());
      }
    }
    public IQueryable<IParameterValue> ParameterValues {
      get {
        return Parameters.AsQueryable().Select(p => p.ParameterValue);
      }
    }

    public IQueryable<Result> Results {
      get {
        return Algorithm_Results.AsQueryable().Select(ar => ar.Result);
      }
    }
    public IQueryable<IntResultValue> IntResultValues {
      get {
        return Results.AsQueryable()
          .Where(r => r.IntResultValues.Count > 0)
          .Select(r => r.IntResultValues.Single());
      }
    }
    public IQueryable<FloatResultValue> FloatResultValues {
      get {
        return Results.AsQueryable()
          .Where(r => r.FloatResultValues.Count > 0)
          .Select(r => r.FloatResultValues.Single());
      }
    }
    public IQueryable<CharResultValue> CharResultValues {
      get {

        return Results.AsQueryable()
          .Where(r => r.CharResultValues.Count > 0)
          .Select(r => r.CharResultValues.Single());
      }
    }
    public IQueryable<BlobResultValue> BlobResultValues {
      get {
        return Results.AsQueryable()
          .Where(r => r.BlobResultValues.Count > 0)
          .Select(r => r.BlobResultValues.Single());
      }
    }
    public IQueryable<IResultValue> ResultValues {
      get {
        return Results.AsQueryable().Select(r => r.ResultValue);
      }
    }
  }
  #endregion

  #region Type access
  public partial class DataType {
    public Type Type {
      get {
        return Type.GetType(ClrName, false) ?? typeof(object);
      }
    }
  }
  #endregion

  #region NamedEntities
  public interface INamedEntity {
    int Id { get; }
    string Name { get; }
    string Description { get; }
  }
  public partial class AlgorithmClass : INamedEntity { }
  public partial class Algorithm : INamedEntity { }
  public partial class ProblemClass : INamedEntity { }
  public partial class Problem : INamedEntity { }
  public partial class SolutionRepresentation : INamedEntity { }
  public partial class ProblemCharacteristic : INamedEntity { }
  public partial class Parameter : INamedEntity { }
  public partial class Result : INamedEntity { }
  public partial class Project : INamedEntity { }
  public partial class Platform : INamedValue { }
  #endregion

  #region DataTypes
  public interface IIntValue {
    int Value { get; }
  }
  public partial class IntResultValue : IIntValue { }
  public partial class IntParameterValue : IIntValue { }
  public partial class IntProblemCharacteristicValue : IIntValue { }
  public interface IFloatValue {
    double Value { get; }
  }
  public partial class FloatResultValue : IFloatValue { }
  public partial class FloatParameterValue : IFloatValue { }
  public partial class FloatProblemCharacteristicValue : IFloatValue { }
  public interface ICharValue {
    string Value { get; }
  }
  public partial class CharResultValue : ICharValue { }
  public partial class CharParameterValue : ICharValue { }
  public partial class CharProblemCharacteristicValue : ICharValue { }
  #endregion

  #region NamedValues
  public interface INamedValue {
    string Name { get; }
  }
  public interface IValue<T> : INamedValue {
    T Value { get; }
  }
  public partial class IntResultValue : IValue<int> {
    public string Name { get { return Result.Name; } }
  }
  public partial class FloatResultValue : IValue<double> {
    public string Name { get { return Result.Name; } }
  }
  public partial class CharResultValue : IValue<string> {
    public string Name { get { return Result.Name; } }
  }
  public partial class BlobResultValue : IValue<Binary> {
    public string Name { get { return Result.Name; } }
  }
  public partial class IntParameterValue : IValue<int> {
    public string Name { get { return Parameter.Name; } }
  }
  public partial class FloatParameterValue : IValue<double> {
    public string Name { get { return Parameter.Name; } }
  }
  public partial class CharParameterValue : IValue<string> {
    public string Name { get { return Parameter.Name; } }
  }
  public partial class OperatorParameterValue : IValue<Binary> {
    public string Name { get { return Parameter.Name; } }
  }
  public partial class IntProblemCharacteristicValue : IValue<int> {
    public string Name { get { return ProblemCharacteristic.Name; } }
  }
  public partial class FloatProblemCharacteristicValue : IValue<double> {
    public string Name { get { return ProblemCharacteristic.Name; } }
  }
  public partial class CharProblemCharacteristicValue : IValue<string> {
    public string Name { get { return ProblemCharacteristic.Name; } }
  }
  #endregion

  #region DynamicTypeInformation
  public interface IDynamicParent : INamedEntity {
    DataType DataType { get; }
  }
  public partial class ProblemCharacteristic : IDynamicParent { }
  public partial class Parameter : IDynamicParent { }
  public partial class Result : IDynamicParent { }
  #endregion
}
