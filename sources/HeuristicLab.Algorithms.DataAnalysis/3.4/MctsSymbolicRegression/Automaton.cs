#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using System.IO;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {
  // this is the core class for generating expressions. 
  // the automaton determines which expressions are allowed
  internal class Automaton {
    public const int StateExpr = 1;
    public const int StateExprEnd = 2;
    public const int StateTermStart = 3;
    public const int StateTermEnd = 4;
    public const int StateFactorStart = 5;
    public const int StateFactorEnd = 6;
    public const int StateVariableFactorStart = 7;
    public const int StateVariableFactorEnd = 8;
    public const int StateExpFactorStart = 9;
    public const int StateExpFactorEnd = 10;
    public const int StateLogFactorStart = 11;
    public const int StateLogFactorEnd = 12;
    public const int StateInvFactorStart = 13;
    public const int StateInvFactorEnd = 14;
    public const int StateExpFStart = 15;
    public const int StateExpFEnd = 16;
    public const int StateLogTStart = 17;
    public const int StateLogTEnd = 18;
    public const int StateLogTFStart = 19;
    public const int StateLogTFEnd = 20;
    public const int StateInvTStart = 21;
    public const int StateInvTEnd = 22;
    public const int StateInvTFStart = 23;
    public const int StateInvTFEnd = 24;
    private const int FirstDynamicState = 25;

    private const int StartState = StateExpr;
    public int CurrentState { get; private set; }

    public readonly List<string> stateNames;
    private List<int>[] followStates;
    private List<Action>[,] actions; // not every follow state is possible but this representation should be efficient
    private List<string>[,] actionStrings; // just for printing 
    private readonly CodeGenerator codeGenerator;
    private readonly ConstraintHandler constraintHandler;

    public Automaton(double[][] vars, int maxVarsInExpression = 100,
       bool allowProdOfVars = true,
       bool allowExp = true,
       bool allowLog = true,
       bool allowInv = true,
       bool allowMultipleTerms = false) {
      int nVars = vars.Length;
      stateNames = new List<string>() { string.Empty, "Expr", "ExprEnd", "TermStart", "TermEnd", "FactorStart", "FactorEnd", "VarFactorStart", "VarFactorEnd", "ExpFactorStart", "ExpFactorEnd", "LogFactorStart", "LogFactorEnd", "InvFactorStart", "InvFactorEnd", "ExpFStart", "ExpFEnd", "LogTStart", "LogTEnd", "LogTFStart", "LogTFEnd", "InvTStart", "InvTEnd", "InvTFStart", "InvTFEnd" };
      codeGenerator = new CodeGenerator();
      constraintHandler = new ConstraintHandler(maxVarsInExpression);
      BuildAutomaton(nVars, allowProdOfVars, allowExp, allowLog, allowInv, allowMultipleTerms);

      Reset();
#if DEBUG
      PrintAutomaton();
#endif
    }

    // reverse notation ops
    // Expr -> c 0 Term { '+' Term } '+' '*' c '+' 'exit'
    // Term -> c Fact { '*' Fact } '*'
    // Fact -> VarFact | ExpFact | LogFact | InvFact
    // VarFact -> var_1 ... var_n
    // ExpFact -> 1 ExpF { '*' ExpF } '*' c '*' 'exp' // c must be at end to allow scaling in evaluator
    // ExpF    -> var_1 ... var_n 
    // LogFact -> 0 LogT { '+' LogT } '+' c '+' 'log' // c must be at end to allow scaling in evaluator
    // LogT    -> c LogTF { '*' LogTF } '*' 
    // LogTF   -> var_1 ... var_n 
    // InvFact -> 1 InvT { '+' InvT } '+' 'inv'
    // InvT    -> (var_1 ... var_n) c '*' 
    private void BuildAutomaton(int nVars,
      bool allowProdOfVars = true,
       bool allowExp = true,
       bool allowLog = true,
       bool allowInv = true,
       bool allowMultipleTerms = false) {

      int nStates = FirstDynamicState + 4 * nVars;
      followStates = new List<int>[nStates];
      actions = new List<Action>[nStates, nStates];
      actionStrings = new List<string>[nStates, nStates];

      // Expr -> c 0 Term { '+' Term } '+' '*' c '+' 'exit'
      AddTransition(StateExpr, StateTermStart, () => {
        codeGenerator.Reset();
        codeGenerator.Emit1(OpCodes.LoadParamN);
        codeGenerator.Emit1(OpCodes.LoadConst0);
        constraintHandler.Reset();
      }, "c 0, Reset");
      AddTransition(StateTermEnd, StateExprEnd, () => {
        codeGenerator.Emit1(OpCodes.Add);
        codeGenerator.Emit1(OpCodes.Mul);
        codeGenerator.Emit1(OpCodes.LoadParamN);
        codeGenerator.Emit1(OpCodes.Add);
        codeGenerator.Emit1(OpCodes.Exit);
      }, "+*c+ exit");
      if (allowMultipleTerms)
        AddTransition(StateTermEnd, StateTermStart, () => {
          codeGenerator.Emit1(OpCodes.Add);
        }, "+");

      // Term -> c Fact { '*' Fact } '*'
      AddTransition(StateTermStart, StateFactorStart,
        () => {
          codeGenerator.Emit1(OpCodes.LoadParamN);
          constraintHandler.StartTerm();
        },
        "c, StartTerm");
      AddTransition(StateFactorEnd, StateTermEnd,
        () => {
          codeGenerator.Emit1(OpCodes.Mul);
          constraintHandler.EndTerm();
        },
        "*, EndTerm");

      AddTransition(StateFactorEnd, StateFactorStart,
        () => { codeGenerator.Emit1(OpCodes.Mul); },
        "*");


      // Fact -> VarFact | ExpFact | LogFact | InvFact
      if (allowProdOfVars)
        AddTransition(StateFactorStart, StateVariableFactorStart, () => {
          constraintHandler.StartFactor(StateVariableFactorStart);
        }, "StartFactor");
      if (allowExp)
        AddTransition(StateFactorStart, StateExpFactorStart, () => {
          constraintHandler.StartFactor(StateExpFactorStart);
        }, "StartFactor");
      if (allowLog)
        AddTransition(StateFactorStart, StateLogFactorStart, () => {
          constraintHandler.StartFactor(StateLogFactorStart);
        }, "StartFactor");
      if (allowInv)
        AddTransition(StateFactorStart, StateInvFactorStart, () => {
          constraintHandler.StartFactor(StateInvFactorStart);
        }, "StartFactor");
      AddTransition(StateVariableFactorEnd, StateFactorEnd, () => { constraintHandler.EndFactor(); }, "EndFactor");
      AddTransition(StateExpFactorEnd, StateFactorEnd, () => { constraintHandler.EndFactor(); }, "EndFactor");
      AddTransition(StateLogFactorEnd, StateFactorEnd, () => { constraintHandler.EndFactor(); }, "EndFactor");
      AddTransition(StateInvFactorEnd, StateFactorEnd, () => { constraintHandler.EndFactor(); }, "EndFactor");

      // VarFact -> var_1 ... var_n
      // add dynamic states for each variable 
      int curDynVarState = FirstDynamicState;
      for (int i = 0; i < nVars; i++) {
        short varIdx = (short)i;
        var varState = curDynVarState;
        stateNames.Add("var_1");
        AddTransition(StateVariableFactorStart, curDynVarState,
          () => {
            codeGenerator.Emit2(OpCodes.LoadVar, varIdx);
            constraintHandler.AddVarToCurrentFactor(varState);
          },
          "var_" + varIdx + ", AddVar");
        AddTransition(curDynVarState, StateVariableFactorEnd);
        curDynVarState++;
      }

      // ExpFact -> 1 ExpF { '*' ExpF } '*' c '*' 'exp' 
      AddTransition(StateExpFactorStart, StateExpFStart,
        () => {
          codeGenerator.Emit1(OpCodes.LoadConst1);
        },
        "1");
      AddTransition(StateExpFEnd, StateExpFactorEnd,
        () => {
          codeGenerator.Emit1(OpCodes.Mul);
          codeGenerator.Emit1(OpCodes.LoadParamN);
          codeGenerator.Emit1(OpCodes.Mul);
          codeGenerator.Emit1(OpCodes.Exp);
        },
        "*c*exp");
      AddTransition(StateExpFEnd, StateExpFStart,
        () => { codeGenerator.Emit1(OpCodes.Mul); },
        "*");

      // ExpF    -> var_1 ... var_n 
      for (int i = 0; i < nVars; i++) {
        short varIdx = (short)i;
        int varState = curDynVarState;
        stateNames.Add("var_2");
        AddTransition(StateExpFStart, curDynVarState,
          () => {
            codeGenerator.Emit2(OpCodes.LoadVar, varIdx);
            constraintHandler.AddVarToCurrentFactor(varState);
          },
          "var_" + varIdx + ", AddVar");
        AddTransition(curDynVarState, StateExpFEnd);
        curDynVarState++;
      }

      // must have c at end because of adjustment of c in evaluator
      // LogFact -> 0 LogT { '+' LogT } '+' c '+' 'log'
      AddTransition(StateLogFactorStart, StateLogTStart,
        () => {
          codeGenerator.Emit1(OpCodes.LoadConst0);
        },
        "0");
      AddTransition(StateLogTEnd, StateLogFactorEnd,
        () => {
          codeGenerator.Emit1(OpCodes.Add);
          codeGenerator.Emit1(OpCodes.LoadParamN);
          codeGenerator.Emit1(OpCodes.Add);
          codeGenerator.Emit1(OpCodes.Log);
        },
        "+c+log");
      AddTransition(StateLogTEnd, StateLogTStart,
        () => { codeGenerator.Emit1(OpCodes.Add); },
        "+");

      // LogT    -> c LogTF { '*' LogTF } '*' 
      AddTransition(StateLogTStart, StateLogTFStart,
        () => {
          codeGenerator.Emit1(OpCodes.LoadParamN);
        },
        "c");
      AddTransition(StateLogTFEnd, StateLogTEnd,
        () => {
          codeGenerator.Emit1(OpCodes.Mul);
        },
        "*");
      AddTransition(StateLogTFEnd, StateLogTFStart,
        () => {
          codeGenerator.Emit1(OpCodes.Mul);
        },
        "*");

      // LogTF   -> var_1 ... var_n
      for (int i = 0; i < nVars; i++) {
        short varIdx = (short)i;
        int varState = curDynVarState;
        stateNames.Add("var_3");
        AddTransition(StateLogTFStart, curDynVarState,
          () => {
            codeGenerator.Emit2(OpCodes.LoadVar, varIdx);
            constraintHandler.AddVarToCurrentFactor(varState);
          },
          "var_" + varIdx + ", AddVar");
        AddTransition(curDynVarState, StateLogTFEnd);
        curDynVarState++;
      }

      // InvFact -> 1 InvT { '+' InvT } '+' 'inv'
      AddTransition(StateInvFactorStart, StateInvTStart,
        () => {
          codeGenerator.Emit1(OpCodes.LoadConst1);
        },
        "c");
      AddTransition(StateInvTEnd, StateInvFactorEnd,
        () => {
          codeGenerator.Emit1(OpCodes.Add);
          codeGenerator.Emit1(OpCodes.Inv);
        },
        "+inv");
      AddTransition(StateInvTEnd, StateInvTStart,
        () => { codeGenerator.Emit1(OpCodes.Add); },
        "+");

      // InvT    -> c InvTF { '*' InvTF } '*' 
      AddTransition(StateInvTStart, StateInvTFStart,
        () => {
          codeGenerator.Emit1(OpCodes.LoadParamN);
        },
        "c");
      AddTransition(StateInvTFEnd, StateInvTEnd,
        () => {
          codeGenerator.Emit1(OpCodes.Mul);
        },
        "*");
      AddTransition(StateInvTFEnd, StateInvTFStart,
        () => {
          codeGenerator.Emit1(OpCodes.Mul);
        },
        "*");

      // InvTF    -> (var_1 ... var_n) c '*' 
      for (int i = 0; i < nVars; i++) {
        short varIdx = (short)i;
        int varState = curDynVarState;
        stateNames.Add("var_4");
        AddTransition(StateInvTFStart, curDynVarState,
          () => {
            codeGenerator.Emit2(OpCodes.LoadVar, varIdx);
            constraintHandler.AddVarToCurrentFactor(varState);
          },
          "var_" + varIdx + ", AddVar");
        AddTransition(curDynVarState, StateInvTFEnd);
        curDynVarState++;
      }

      followStates[StateExprEnd] = new List<int>(); // no follow states
    }

    private void AddTransition(int fromState, int toState) {
      if (followStates[fromState] == null) followStates[fromState] = new List<int>();
      followStates[fromState].Add(toState);
    }
    private void AddTransition(int fromState, int toState, Action action, string str) {
      if (followStates[fromState] == null) followStates[fromState] = new List<int>();
      followStates[fromState].Add(toState);

      if (actions[fromState, toState] == null) {
        actions[fromState, toState] = new List<Action>();
        actionStrings[fromState, toState] = new List<string>();
      }

      actions[fromState, toState].Add(action);
      actionStrings[fromState, toState].Add(str);
    }

    private readonly int[] followStatesBuf = new int[1000];
    public void FollowStates(int state, out int[] buf, out int nElements) {
      // return followStates[state]
      //   .Where(s => s < FirstDynamicState || s >= minVarIdx) // for variables we only allow non-decreasing state sequences
      //   // the following states imply an additional variable being added to the expression
      //   // F, Sum, Prod
      //   .Where(s => (s != StateF && s != StateSum && s != StateProd) || variablesRemaining > 0);

      // for loop instead of where iterator
      var fs = followStates[state];
      int j = 0;
      //Console.Write(stateNames[CurrentState] + " allowed: ");
      for (int i = 0; i < fs.Count; i++) {
        var s = fs[i];
        if (constraintHandler.IsAllowedFollowState(state, s)) {
          //Console.Write(s + " ");
          followStatesBuf[j++] = s;
        }
      }
      //Console.WriteLine();
      buf = followStatesBuf;
      nElements = j;
    }


    public void Goto(int targetState) {
      //Console.WriteLine("->{0}", stateNames[targetState]);
      // Contract.Assert(FollowStates(CurrentState).Contains(targetState));

      if (actions[CurrentState, targetState] != null)
        actions[CurrentState, targetState].ForEach(a => a()); // execute all actions
      CurrentState = targetState;
    }

    public bool IsFinalState(int s) {
      return s == StateExprEnd;
    }

    public void GetCode(out byte[] code, out int nParams) {
      codeGenerator.GetCode(out code, out nParams);
    }

    public void Reset() {
      CurrentState = StartState;
      codeGenerator.Reset();
      constraintHandler.Reset();
    }

#if DEBUG
    public void PrintAutomaton() {
      using (var writer = new StreamWriter("automaton.gv")) {
        writer.WriteLine("digraph {");
        // writer.WriteLine("rankdir=LR");
        int[] fs;
        int nFs;
        for (int s = StartState; s < stateNames.Count; s++) {
          for (int i = 0; i < followStates[s].Count; i++) {
            if (followStates[s][i] <= 0) continue;
            var followS = followStates[s][i];
            var label = actionStrings[s, followS] != null ? string.Join(" , ", actionStrings[s, followS]) : "";
            writer.WriteLine("{0} -> {1} [ label = \"{2}\" ];", stateNames[s], stateNames[followS], label);
          }
        }
        writer.WriteLine("}");
      }
    }
#endif
  }
}
