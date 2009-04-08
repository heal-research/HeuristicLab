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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Constraints;
using StructId = HeuristicLab.GP.StructureIdentification;

namespace HeuristicLab.GP.StructureIdentification {
  public class ConfigurableFunctionLibraryInjector : FunctionLibraryInjector {
    private const string VARIABLES_ALLOWED = "Variables";
    private const string CONSTANTS_ALLOWED = "Constants";
    private const string ADDITION_ALLOWED = "Addition";
    private const string AVERAGE_ALLOWED = "Average";
    private const string AND_ALLOWED = "And";
    private const string COSINUS_ALLOWED = "Cosinus";
    private const string DIVISION_ALLOWED = "Division";
    private const string EQUAL_ALLOWED = "Equal";
    private const string EXPONENTIAL_ALLOWED = "Exponential";
    private const string GREATERTHAN_ALLOWED = "GreaterThan";
    private const string IFTHENELSE_ALLOWED = "IfThenElse";
    private const string LESSTHAN_ALLOWED = "LessThan";
    private const string LOGARTIHM_ALLOWED = "Logarithm";
    private const string MULTIPLICATION_ALLOWED = "Multiplication";
    private const string NOT_ALLOWED = "Not";
    private const string POWER_ALLOWED = "Power";
    private const string OR_ALLOWED = "Or";
    private const string SIGNUM_ALLOWED = "Signum";
    private const string SINUS_ALLOWED = "Sinus";
    private const string SQRT_ALLOWED = "SquareRoot";
    private const string SUBTRACTION_ALLOWED = "Subtraction";
    private const string TANGENS_ALLOWED = "Tangens";
    private const string XOR_ALLOWED = "Xor";


    public override string Description {
      get { return @"Injects a configurable function library for regression and classification problems."; }
    }

    public ConfigurableFunctionLibraryInjector()
      : base() {

      AddVariableInfo(new Core.VariableInfo(VARIABLES_ALLOWED, VARIABLES_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(VARIABLES_ALLOWED).Local = true;
      AddVariable(new Core.Variable(VARIABLES_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(CONSTANTS_ALLOWED, CONSTANTS_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(CONSTANTS_ALLOWED).Local = true;
      AddVariable(new Core.Variable(CONSTANTS_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(ADDITION_ALLOWED, ADDITION_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(ADDITION_ALLOWED).Local = true;
      AddVariable(new Core.Variable(ADDITION_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(AVERAGE_ALLOWED, AVERAGE_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(AVERAGE_ALLOWED).Local = true;
      AddVariable(new Core.Variable(AVERAGE_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(AND_ALLOWED, AND_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(AND_ALLOWED).Local = true;
      AddVariable(new Core.Variable(AND_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(COSINUS_ALLOWED, COSINUS_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(COSINUS_ALLOWED).Local = true;
      AddVariable(new Core.Variable(COSINUS_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(DIVISION_ALLOWED, DIVISION_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(DIVISION_ALLOWED).Local = true;
      AddVariable(new Core.Variable(DIVISION_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(EQUAL_ALLOWED, EQUAL_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(EQUAL_ALLOWED).Local = true;
      AddVariable(new Core.Variable(EQUAL_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(EXPONENTIAL_ALLOWED, EXPONENTIAL_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(EXPONENTIAL_ALLOWED).Local = true;
      AddVariable(new Core.Variable(EXPONENTIAL_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(GREATERTHAN_ALLOWED, GREATERTHAN_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(GREATERTHAN_ALLOWED).Local = true;
      AddVariable(new Core.Variable(GREATERTHAN_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(IFTHENELSE_ALLOWED, IFTHENELSE_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(IFTHENELSE_ALLOWED).Local = true;
      AddVariable(new Core.Variable(IFTHENELSE_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(LESSTHAN_ALLOWED, LESSTHAN_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(LESSTHAN_ALLOWED).Local = true;
      AddVariable(new Core.Variable(LESSTHAN_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(LOGARTIHM_ALLOWED, LOGARTIHM_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(LOGARTIHM_ALLOWED).Local = true;
      AddVariable(new Core.Variable(LOGARTIHM_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(MULTIPLICATION_ALLOWED, MULTIPLICATION_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(MULTIPLICATION_ALLOWED).Local = true;
      AddVariable(new Core.Variable(MULTIPLICATION_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(NOT_ALLOWED, NOT_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(NOT_ALLOWED).Local = true;
      AddVariable(new Core.Variable(NOT_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(POWER_ALLOWED, POWER_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(POWER_ALLOWED).Local = true;
      AddVariable(new Core.Variable(POWER_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(OR_ALLOWED, OR_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(OR_ALLOWED).Local = true;
      AddVariable(new Core.Variable(OR_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(SIGNUM_ALLOWED, SIGNUM_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(SIGNUM_ALLOWED).Local = true;
      AddVariable(new Core.Variable(SIGNUM_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(SINUS_ALLOWED, SINUS_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(SINUS_ALLOWED).Local = true;
      AddVariable(new Core.Variable(SINUS_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(SQRT_ALLOWED, SQRT_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(SQRT_ALLOWED).Local = true;
      AddVariable(new Core.Variable(SQRT_ALLOWED, new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(SUBTRACTION_ALLOWED, SUBTRACTION_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(SUBTRACTION_ALLOWED).Local = true;
      AddVariable(new Core.Variable(SUBTRACTION_ALLOWED,  new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(TANGENS_ALLOWED, TANGENS_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(TANGENS_ALLOWED).Local = true;
      AddVariable(new Core.Variable(TANGENS_ALLOWED, new BoolData(true)));

      AddVariableInfo(new Core.VariableInfo(XOR_ALLOWED, XOR_ALLOWED + " allowed", typeof(BoolData), Core.VariableKind.New));
      GetVariableInfo(XOR_ALLOWED).Local = true;
      AddVariable(new Core.Variable(XOR_ALLOWED,  new BoolData(true)));
    }

    //public override IView CreateView() {
    //  return new ConfigurableFunctionLibraryInjectorView(this);
    //}

    public override IOperation Apply(IScope scope) {
      base.Apply(scope);

      GPOperatorLibrary functionLibrary = (GPOperatorLibrary)scope.GetVariable(scope.TranslateName(FUNCTIONLIBRARY)).Value;

      if (!((BoolData)GetVariable(VARIABLES_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Variable)));
      if (!((BoolData)GetVariable(CONSTANTS_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Constant)));
      if (!((BoolData)GetVariable(ADDITION_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Addition)));
      if (!((BoolData)GetVariable(AVERAGE_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Average)));
      if (!((BoolData)GetVariable(AND_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(And)));
      if (!((BoolData)GetVariable(COSINUS_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Cosinus)));
      if (!((BoolData)GetVariable(DIVISION_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Division)));
      if (!((BoolData)GetVariable(EQUAL_ALLOWED).Value).Data)                                                       
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Equal)));
      if (!((BoolData)GetVariable(EXPONENTIAL_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Exponential)));
      if (!((BoolData)GetVariable(GREATERTHAN_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(GreaterThan)));
      if (!((BoolData)GetVariable(IFTHENELSE_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(IfThenElse)));
      if (!((BoolData)GetVariable(LESSTHAN_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(LessThan)));
      if (!((BoolData)GetVariable(LOGARTIHM_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Logarithm)));
      if (!((BoolData)GetVariable(MULTIPLICATION_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Multiplication)));
      if (!((BoolData)GetVariable(NOT_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Not)));
      if (!((BoolData)GetVariable(POWER_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Power)));
      if (!((BoolData)GetVariable(OR_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Or)));
      if (!((BoolData)GetVariable(SIGNUM_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Signum)));
      if (!((BoolData)GetVariable(SINUS_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Sinus)));
      if (!((BoolData)GetVariable(SQRT_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Sqrt)));
      if (!((BoolData)GetVariable(SUBTRACTION_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Subtraction)));
      if (!((BoolData)GetVariable(TANGENS_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Tangens)));
      if (!((BoolData)GetVariable(XOR_ALLOWED).Value).Data)
        functionLibrary.GPOperatorGroup.RemoveOperator(FindOperator(functionLibrary.GPOperatorGroup, typeof(Xor)));


      return null;
    }

    private IFunction FindOperator(GPOperatorGroup g, Type t)  {
      foreach (IFunction func in g.Operators)
        if (func.GetType() == t)
          return func;
      return null;
  }


  }
}
