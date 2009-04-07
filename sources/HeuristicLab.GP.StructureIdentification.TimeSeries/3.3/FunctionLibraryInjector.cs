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

using System.Collections.Generic;
using Core = HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Constraints;

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  public class FunctionLibraryInjector : Core.OperatorBase {
    private const string TARGETVARIABLE = "TargetVariable";
    private const string AUTOREGRESSIVE = "Autoregressive";
    private const string ALLOWEDFEATURES = "AllowedFeatures";
    private const string MINTIMEOFFSET = "MinTimeOffset";
    private const string MAXTIMEOFFSET = "MaxTimeOffset";
    private const string FUNCTIONLIBRARY = "FunctionLibrary";

    private Variable variable;
    private Differential differential;
    private GPOperatorLibrary operatorLibrary;

    public override string Description {
      get { return @"Injects a default function library for time series modeling."; }
    }

    public FunctionLibraryInjector()
      : base() {
      AddVariableInfo(new Core.VariableInfo(TARGETVARIABLE, "The target variable", typeof(IntData), Core.VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(AUTOREGRESSIVE, "Switch to turn on/off autoregressive modeling (wether to allow the target variable as input)", typeof(BoolData), Core.VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(ALLOWEDFEATURES, "List of indexes of allowed features", typeof(ItemList<IntData>), Core.VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(MINTIMEOFFSET, "Minimal time offset for all features", typeof(IntData), Core.VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(MAXTIMEOFFSET, "Maximal time offset for all feature", typeof(IntData), Core.VariableKind.In));
      AddVariableInfo(new Core.VariableInfo(FUNCTIONLIBRARY, "Preconfigured default operator library", typeof(GPOperatorLibrary), Core.VariableKind.New));
    }

    public override Core.IOperation Apply(Core.IScope scope) {
      IntData minTimeOffset = GetVariableValue<IntData>(MINTIMEOFFSET, scope, true);
      IntData maxTimeOffset = GetVariableValue<IntData>(MAXTIMEOFFSET, scope, true);
      ItemList<IntData> allowedFeatures = GetVariableValue<ItemList<IntData>>(ALLOWEDFEATURES, scope, true);
      int targetVariable = GetVariableValue<IntData>(TARGETVARIABLE, scope, true).Data;
      bool autoregressive = GetVariableValue<BoolData>(AUTOREGRESSIVE, scope, true).Data;

      if(autoregressive) {
        // make sure the target-variable occures in list of allowed features
        if(!allowedFeatures.Exists(d => d.Data == targetVariable)) allowedFeatures.Add(new IntData(targetVariable));
      } else {
        // remove the target-variable in case it occures in allowed features
        List<IntData> ts = allowedFeatures.FindAll(d => d.Data == targetVariable);
        foreach(IntData t in ts) allowedFeatures.Remove(t);
      }

      InitDefaultOperatorLibrary();

      int[] allowedIndexes = new int[allowedFeatures.Count];
      for(int i = 0; i < allowedIndexes.Length; i++) {
        allowedIndexes[i] = allowedFeatures[i].Data;
      }

      variable.SetConstraints(allowedIndexes, minTimeOffset.Data, maxTimeOffset.Data);
      differential.SetConstraints(allowedIndexes, minTimeOffset.Data, maxTimeOffset.Data);

      scope.AddVariable(new Core.Variable(scope.TranslateName(FUNCTIONLIBRARY), operatorLibrary));
      return null;
    }

    private void InitDefaultOperatorLibrary() {
      variable = new Variable();
      differential = new Differential();
      Constant constant = new Constant();

      Addition addition = new Addition();
      And and = new And();
      Average average = new Average();
      Cosinus cosinus = new Cosinus();
      Division division = new Division();
      Equal equal = new Equal();
      Exponential exponential = new Exponential();
      GreaterThan greaterThan = new GreaterThan();
      IfThenElse ifThenElse = new IfThenElse();
      LessThan lessThan = new LessThan();
      Logarithm logarithm = new Logarithm();
      Multiplication multiplication = new Multiplication();
      Not not = new Not();
      Or or = new Or();
      Power power = new Power();
      Signum signum = new Signum();
      Sinus sinus = new Sinus();
      Sqrt sqrt = new Sqrt();
      Subtraction subtraction = new Subtraction();
      Tangens tangens = new Tangens();
      Xor xor = new Xor();

      IFunction[] booleanFunctions = new IFunction[] {
        and,
        equal,
        greaterThan,
        lessThan,
        not,
        or,
        xor };
      IFunction[] doubleFunctions = new IFunction[] {
        variable,
        differential,
        constant,
        addition,
        average,
        cosinus,
        division,
        exponential,
        ifThenElse,
        logarithm,
        multiplication,
        power,
        signum,
        sinus,
        sqrt,
        subtraction,
        tangens};

      SetAllowedSubOperators(and, booleanFunctions);
      SetAllowedSubOperators(equal, doubleFunctions);
      SetAllowedSubOperators(greaterThan, doubleFunctions);
      SetAllowedSubOperators(lessThan, doubleFunctions);
      SetAllowedSubOperators(not, booleanFunctions);
      SetAllowedSubOperators(or, booleanFunctions);
      SetAllowedSubOperators(xor, booleanFunctions);
      SetAllowedSubOperators(addition, doubleFunctions);
      SetAllowedSubOperators(average, doubleFunctions);
      SetAllowedSubOperators(cosinus, doubleFunctions);
      SetAllowedSubOperators(division, doubleFunctions);
      SetAllowedSubOperators(exponential, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, 0, booleanFunctions);
      SetAllowedSubOperators(ifThenElse, 1, doubleFunctions);
      SetAllowedSubOperators(ifThenElse, 2, doubleFunctions);
      SetAllowedSubOperators(logarithm, doubleFunctions);
      SetAllowedSubOperators(multiplication, doubleFunctions);
      SetAllowedSubOperators(power, doubleFunctions);
      SetAllowedSubOperators(signum, doubleFunctions);
      SetAllowedSubOperators(sinus, doubleFunctions);
      SetAllowedSubOperators(sqrt, doubleFunctions);
      SetAllowedSubOperators(subtraction, doubleFunctions);
      SetAllowedSubOperators(tangens, doubleFunctions);

      operatorLibrary = new GPOperatorLibrary();
      operatorLibrary.GPOperatorGroup.AddOperator(variable);
      operatorLibrary.GPOperatorGroup.AddOperator(differential);
      operatorLibrary.GPOperatorGroup.AddOperator(constant);
      operatorLibrary.GPOperatorGroup.AddOperator(addition);
      operatorLibrary.GPOperatorGroup.AddOperator(average);
      operatorLibrary.GPOperatorGroup.AddOperator(and);
      operatorLibrary.GPOperatorGroup.AddOperator(cosinus);
      operatorLibrary.GPOperatorGroup.AddOperator(division);
      operatorLibrary.GPOperatorGroup.AddOperator(equal);
      operatorLibrary.GPOperatorGroup.AddOperator(exponential);
      operatorLibrary.GPOperatorGroup.AddOperator(greaterThan);
      operatorLibrary.GPOperatorGroup.AddOperator(ifThenElse);
      operatorLibrary.GPOperatorGroup.AddOperator(lessThan);
      operatorLibrary.GPOperatorGroup.AddOperator(logarithm);
      operatorLibrary.GPOperatorGroup.AddOperator(multiplication);
      operatorLibrary.GPOperatorGroup.AddOperator(not);
      operatorLibrary.GPOperatorGroup.AddOperator(power);
      operatorLibrary.GPOperatorGroup.AddOperator(or);
      operatorLibrary.GPOperatorGroup.AddOperator(signum);
      operatorLibrary.GPOperatorGroup.AddOperator(sinus);
      operatorLibrary.GPOperatorGroup.AddOperator(sqrt);
      operatorLibrary.GPOperatorGroup.AddOperator(subtraction);
      operatorLibrary.GPOperatorGroup.AddOperator(tangens);
      operatorLibrary.GPOperatorGroup.AddOperator(xor);
    }

    private void SetAllowedSubOperators(IFunction f, IFunction[] gs) {
      foreach(Core.IConstraint c in f.Constraints) {
        if(c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          typeConstraint.Clear();
          foreach(IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        } else if(c is AllSubOperatorsTypeConstraint) {
          AllSubOperatorsTypeConstraint typeConstraint = c as AllSubOperatorsTypeConstraint;
          typeConstraint.Clear();
          foreach(IFunction g in gs) {
            typeConstraint.AddOperator(g);
          }
        }
      }
    }

    private void SetAllowedSubOperators(IFunction f, int p, IFunction[] gs) {
      foreach(Core.IConstraint c in f.Constraints) {
        if(c is SubOperatorTypeConstraint) {
          SubOperatorTypeConstraint typeConstraint = c as SubOperatorTypeConstraint;
          if(typeConstraint.SubOperatorIndex.Data == p) {
            typeConstraint.Clear();
            foreach(IFunction g in gs) {
              typeConstraint.AddOperator(g);
            }
          }
        }
      }
    }
  }
}
