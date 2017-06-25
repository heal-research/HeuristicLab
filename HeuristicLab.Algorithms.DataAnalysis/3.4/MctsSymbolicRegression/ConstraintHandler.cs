#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics.Contracts;
using System.Linq;

namespace HeuristicLab.Algorithms.DataAnalysis.MctsSymbolicRegression {

  // This class restricts the set of allowed transitions of the automaton to prevent exploration of duplicate expressions.
  // It would be possible to implement this class in such a way that the search never visits a duplicate expression. However,
  // it seems very intricate to detect this robustly and in all cases while generating an expression because 
  // some for of lookahead is necessary. 
  // Instead the constraint handler only catches the obvious duplicates directly, but does not guarantee that the search always produces a valid expression.
  // The ratio of the number of unsuccessful searches, that need backtracking should be tracked in the MCTS alg (MctsSymbolicRegressionStatic)

  // All changes to this class should be tested through unit tests. It is important that the ConstraintHandler is not too restrictive.

  // the constraints are derived from a canonical form for expressions.
  // overall we can enforce a limited number of variable references
  // 
  // an expression is a sum of terms t_1 ... t_n where terms are ordered according to a relation t_i (<=)_term t_j for each pair t_i, t_j and i <= j
  // a term is a product of factors where factors are ordered according to relation f_i (<=)_factor f_j for each pair f_i,f_j and i <= j

  // we want to enforce lower-order terms before higher-order terms in expressions (based on number of variable references)
  // factors can have different types (variable, exp, log, inverse)

  // (<=)_term  [IsSmallerOrEqualTerm(t_i, t_j)]
  //   1.  NumberOfVarRefs(t_i) < NumberOfVarRefs(t_j)  --> true           enforce terms with non-decreasing number of var refs
  //   2.  NumberOfVarRefs(t_i) > NumberOfVarRefs(t_j)  --> false
  //   3.  NumFactors(t_i) > NumFactors(t_j)            --> true           enforce terms with non-increasing number of factors
  //   4.  NumFactors(t_i) < NumFactors(t_j)            --> false
  //   5.  for all k factors: Factor(k, t_i) (<=)_factor  Factor(k, t_j) --> true // factors must be non-decreasing
  //   6.  all factors are (=)_factor                   --> true
  //   7.  else false

  // (<=)_factor  [IsSmallerOrEqualFactor(f_i, f_j)]
  //   1.  FactorType(t_i) < FactorType(t_j)  --> true           enforce terms with non-decreasing factor type (var < exp < log < inv)
  //   2.  FactorType(t_i) > FactorType(t_j)  --> false
  //   3.  Compare the two factors specifically
  //     - variables: varIdx_i <= varIdx_j (only one var reference)
  //     - exp: number of variable references and then varIdx_i <= varIdx_j for each position
  //     - log: number of variable references and ...
  //     - inv: number of variable references and ...
  //

  // for log and inverse factors we allow all polynomials as argument
  // a polynomial is a sum of terms t_1 ... t_n where terms are ordered according to a relation t_i (<=)_poly t_j for each pair t_i, t_j and i <= j

  // (<=)_poly  [IsSmallerOrEqualPoly(t_i, t_j)]
  //  1. NumberOfVarRefs(t_i) < NumberOfVarRefs(t_j)         --> true // enforce non-decreasing number of var refs
  //  2. NumberOfVarRefs(t_i) > NumberOfVarRefs(t_j)         --> false // enforce non-decreasing number of var refs
  //  3. for all k variables: VarIdx(k,t_i) > VarIdx(k, t_j) --> false // enforce non-decreasing variable idx


  // we store the following to make comparsions:
  // - prevTerm (complete & containing all factors)
  // - curTerm  (incomplete & containing all completed factors)
  // - curFactor (incomplete)
  internal class ConstraintHandler {
    private int nVars;
    private readonly int maxVariables;
    private bool invalidExpression;

    public bool IsInvalidExpression {
      get { return invalidExpression; }
    }


    private TermInformation prevTerm;
    private TermInformation curTerm;
    private FactorInformation curFactor;


    private class TermInformation {
      public int numVarReferences { get { return factors.Sum(f => f.numVarReferences); } }
      public List<FactorInformation> factors = new List<FactorInformation>();
    }

    private class FactorInformation {
      public int numVarReferences = 0;
      public int factorType; // use the state number to represent types

      // for variable factors
      public int variableState = -1;

      // for exp  factors
      public List<int> expVariableStates = new List<int>();

      // for log and inv factors
      public List<List<int>> polyVariableStates = new List<List<int>>();
    }


    public ConstraintHandler(int maxVars) {
      this.maxVariables = maxVars;
    }

    // the order relations for terms and factors

    private static int CompareTerms(TermInformation a, TermInformation b) {
      if (a.numVarReferences < b.numVarReferences) return -1;
      if (a.numVarReferences > b.numVarReferences) return 1;

      if (a.factors.Count > b.factors.Count) return -1;  // terms with more factors should be ordered first
      if (a.factors.Count < b.factors.Count) return +1;

      var aFactors = a.factors.GetEnumerator();
      var bFactors = b.factors.GetEnumerator();
      while (aFactors.MoveNext() & bFactors.MoveNext()) {
        var c = CompareFactors(aFactors.Current, bFactors.Current);
        if (c < 0) return -1;
        if (c > 0) return 1;
      }
      // all factors are the same => terms are the same
      return 0;
    }

    private static int CompareFactors(FactorInformation a, FactorInformation b) {
      if (a.factorType < b.factorType) return -1;
      if (a.factorType > b.factorType) return +1;
      // same factor types
      if (a.factorType == Automaton.StateVariableFactorStart) {
        return a.variableState.CompareTo(b.variableState);
      } else if (a.factorType == Automaton.StateExpFactorStart) {
        return CompareStateLists(a.expVariableStates, b.expVariableStates);
      } else {
        if (a.numVarReferences < b.numVarReferences) return -1;
        if (a.numVarReferences > b.numVarReferences) return +1;
        if (a.polyVariableStates.Count > b.polyVariableStates.Count) return -1; // more terms in the poly should be ordered first
        if (a.polyVariableStates.Count < b.polyVariableStates.Count) return +1;
        // log and inv
        var aTerms = a.polyVariableStates.GetEnumerator();
        var bTerms = b.polyVariableStates.GetEnumerator();
        while (aTerms.MoveNext() & bTerms.MoveNext()) {
          var c = CompareStateLists(aTerms.Current, bTerms.Current);
          if (c != 0) return c;
        }
        return 0; // all terms in the polynomial are the same
      }
    }

    private static int CompareStateLists(List<int> a, List<int> b) {
      if (a.Count < b.Count) return -1;
      if (a.Count > b.Count) return +1;
      for (int i = 0; i < a.Count; i++) {
        if (a[i] < b[i]) return -1;
        if (a[i] > b[i]) return +1;
      }
      return 0; // all states are the same
    }


    private bool IsNewTermAllowed() {
      // next term must have at least as many variable references as the previous term
      return prevTerm == null || nVars + prevTerm.numVarReferences <= maxVariables;
    }

    private bool IsNewFactorAllowed() {
      // next factor must have a larger or equal type compared to the previous factor.
      // if the types are the same it must have at least as many variable references.
      // so if the prevFactor is any other than invFactor (last possible type) then we only need to be able to add one variable
      // otherwise we need to be able to add at least as many variables as the previous factor
      return !curTerm.factors.Any() ||
             (nVars + curTerm.factors.Last().numVarReferences <= maxVariables);
    }

    private bool IsAllowedAsNextFactorType(int followState) {
      // IsNewTermAllowed already ensures that we can add a term with enough variable references

      // enforce constraints within terms (compare to prev factor)
      if (curTerm.factors.Any()) {
        // enforce non-decreasing factor types
        if (curTerm.factors.Last().factorType > followState) return false;
        // when the factor type is the same, starting a new factor is only allowed if we can add at least the number of variables of the prev factor
        if (curTerm.factors.Last().factorType == followState && nVars + curTerm.factors.Last().numVarReferences > maxVariables) return false;
      }

      // enforce constraints on terms (compare to prev term)
      // meaning that we must ensure non-decreasing terms
      if (prevTerm != null) {
        // a factor type is only allowed if we can then produce a term that is larger or equal to the prev term
        // (1) if we the number of variable references still remaining is larger than the number of variable references in the prev term 
        //     then it is always possible to build a larger term
        // (2) otherwise we try to build the largest possible term starting from current factors in the term.
        //     

        var numVarRefsRemaining = maxVariables - nVars;
        Contract.Assert(!curTerm.factors.Any() || curTerm.factors.Last().numVarReferences <= numVarRefsRemaining);

        if (prevTerm.numVarReferences < numVarRefsRemaining) return true;

        // variable factors must be handled differently because they can only contain one variable reference
        if (followState == Automaton.StateVariableFactorStart) {
          // append the variable factor and the maximum possible state from the previous factor to create a larger factor
          var varF = CreateLargestPossibleFactor(Automaton.StateVariableFactorStart, 1);
          var maxF = CreateLargestPossibleFactor(prevTerm.factors.Max(f => f.factorType), numVarRefsRemaining - 1);
          var origFactorCount = curTerm.factors.Count;
          // add this factor to the current term
          curTerm.factors.Add(varF);
          curTerm.factors.Add(maxF);
          var c = CompareTerms(prevTerm, curTerm);
          // restore term
          curTerm.factors.RemoveRange(origFactorCount, 2);
          // if the prev term is still larger then this followstate is not allowed
          if (c > 0) {
            return false;
          }
        } else {
          var newF = CreateLargestPossibleFactor(followState, numVarRefsRemaining);

          var origFactorCount = curTerm.factors.Count;
          // add this factor to the current term
          curTerm.factors.Add(newF);
          var c = CompareTerms(prevTerm, curTerm);
          // restore term
          curTerm.factors.RemoveAt(origFactorCount);
          // if the prev term is still larger then this followstate is not allowed
          if (c > 0) {
            return false;
          }
        }
      }
      return true;
    }

    // largest possible factor of the given kind
    private FactorInformation CreateLargestPossibleFactor(int factorType, int numVarRefs) {
      var newF = new FactorInformation();
      newF.factorType = factorType;
      if (factorType == Automaton.StateVariableFactorStart) {
        newF.variableState = int.MaxValue;
        newF.numVarReferences = 1;
      } else if (factorType == Automaton.StateExpFactorStart) {
        for (int i = 0; i < numVarRefs; i++)
          newF.expVariableStates.Add(int.MaxValue);
        newF.numVarReferences = numVarRefs;
      } else if (factorType == Automaton.StateInvFactorStart || factorType == Automaton.StateLogFactorStart) {
        for (int i = 0; i < numVarRefs; i++) {
          newF.polyVariableStates.Add(new List<int>());
          newF.polyVariableStates[i].Add(int.MaxValue);
        }
        newF.numVarReferences = numVarRefs;
      }
      return newF;
    }

    private bool IsAllowedAsNextVariableFactor(int variableState) {
      Contract.Assert(variableState >= Automaton.FirstDynamicState);
      return !curTerm.factors.Any() || curTerm.factors.Last().variableState <= variableState;
    }

    private bool IsAllowedAsNextInExp(int variableState) {
      Contract.Assert(variableState >= Automaton.FirstDynamicState);
      if (curFactor.expVariableStates.Any() && curFactor.expVariableStates.Last() > variableState) return false;
      if (curTerm.factors.Any()) {
        // try and compare with prev factor      
        curFactor.numVarReferences++;
        curFactor.expVariableStates.Add(variableState);
        var c = CompareFactors(curTerm.factors.Last(), curFactor);
        curFactor.numVarReferences--;
        curFactor.expVariableStates.RemoveAt(curFactor.expVariableStates.Count - 1);
        return c <= 0;
      }
      return true;
    }

    private bool IsNewTermAllowedInPoly() {
      return nVars + curFactor.polyVariableStates.Last().Count() <= maxVariables;
    }

    private bool IsAllowedAsNextInPoly(int variableState) {
      Contract.Assert(variableState >= Automaton.FirstDynamicState);
      return !curFactor.polyVariableStates.Any() ||
             !curFactor.polyVariableStates.Last().Any() ||
              curFactor.polyVariableStates.Last().Last() <= variableState;
    }
    private bool IsTermCompleteInPoly() {
      var nTerms = curFactor.polyVariableStates.Count;
      return nTerms == 1 ||
             curFactor.polyVariableStates[nTerms - 2].Count <= curFactor.polyVariableStates[nTerms - 1].Count;

    }
    private bool IsCompleteExp() {
      return !curTerm.factors.Any() || CompareFactors(curTerm.factors.Last(), curFactor) <= 0;
    }

    public bool IsAllowedFollowState(int currentState, int followState) {
      // an invalid action was taken earlier on => nothing can be done anymore
      if (invalidExpression) return false;
      // states that have no alternative are always allowed
      // some ending states are only allowed if enough variables have been used in the term
      if (
        currentState == Automaton.StateTermStart ||           // no alternative
        currentState == Automaton.StateExpFactorStart ||
        currentState == Automaton.StateLogFactorStart ||
        currentState == Automaton.StateInvFactorStart ||
        followState == Automaton.StateVariableFactorEnd ||    // no alternative
        followState == Automaton.StateExpFEnd ||              // no alternative
        followState == Automaton.StateLogTFEnd ||             // no alternative
        followState == Automaton.StateInvTFEnd ||             // no alternative
        followState == Automaton.StateFactorEnd ||            // always allowed because no alternative
        followState == Automaton.StateExprEnd                 // we could also constrain the minimum number of terms here
      ) return true;


      // starting a new term is only allowed if we can add a term with at least the number of variables of the prev term
      if (followState == Automaton.StateTermStart && !IsNewTermAllowed()) return false;
      if (followState == Automaton.StateFactorStart && !IsNewFactorAllowed()) return false;
      if (currentState == Automaton.StateFactorStart && !IsAllowedAsNextFactorType(followState)) return false;
      if (followState == Automaton.StateTermEnd && prevTerm != null && CompareTerms(prevTerm, curTerm) > 0) return false;

      // all of these states add at least one variable
      if (
          followState == Automaton.StateVariableFactorStart ||
          followState == Automaton.StateExpFactorStart || followState == Automaton.StateExpFStart ||
          followState == Automaton.StateLogFactorStart || followState == Automaton.StateLogTStart ||
          followState == Automaton.StateLogTFStart ||
          followState == Automaton.StateInvFactorStart || followState == Automaton.StateInvTStart ||
          followState == Automaton.StateInvTFStart) {
        if (nVars + 1 > maxVariables) return false;
      }

      if (currentState == Automaton.StateVariableFactorStart && !IsAllowedAsNextVariableFactor(followState)) return false;
      else if (currentState == Automaton.StateExpFStart && !IsAllowedAsNextInExp(followState)) return false;
      else if (followState == Automaton.StateLogTStart && !IsNewTermAllowedInPoly()) return false;
      else if (currentState == Automaton.StateLogTFStart && !IsAllowedAsNextInPoly(followState)) return false;
      else if (followState == Automaton.StateInvTStart && !IsNewTermAllowedInPoly()) return false;
      else if (currentState == Automaton.StateInvTFStart && !IsAllowedAsNextInPoly(followState)) return false;
      // finishing an exponential factor is only allowed when the number of variable references is large enough
      else if (followState == Automaton.StateExpFactorEnd && !IsCompleteExp()) return false;
      // finishing a polynomial (in log or inv) is only allowed when the number of variable references is large enough
      else if (followState == Automaton.StateInvTEnd && !IsTermCompleteInPoly()) return false;
      else if (followState == Automaton.StateLogTEnd && !IsTermCompleteInPoly()) return false;

      else if (nVars > maxVariables) return false;
      else return true;
    }


    public void Reset() {
      nVars = 0;
      prevTerm = null;
      curTerm = null;
      curFactor = null;
      invalidExpression = false;
    }

    public void StartTerm() {
      curTerm = new TermInformation();
    }

    public void StartFactor(int state) {
      curFactor = new FactorInformation();
      curFactor.factorType = state;
    }


    public void AddVarToCurrentFactor(int state) {
      Contract.Assert(Automaton.FirstDynamicState <= state);
      Contract.Assert(curTerm != null);
      Contract.Assert(curFactor != null);

      nVars++;
      curFactor.numVarReferences++;

      if (curFactor.factorType == Automaton.StateVariableFactorStart) {
        Contract.Assert(curFactor.variableState < 0); // not set before
        curFactor.variableState = state;
      } else if (curFactor.factorType == Automaton.StateExpFactorStart) {
        curFactor.expVariableStates.Add(state);
      } else if (curFactor.factorType == Automaton.StateLogFactorStart ||
                 curFactor.factorType == Automaton.StateInvFactorStart) {
        curFactor.polyVariableStates.Last().Add(state);
      } else throw new InvalidProgramException();
    }

    public void StartNewTermInPoly() {
      curFactor.polyVariableStates.Add(new List<int>());
    }

    public void EndFactor() {
      // enforce non-decreasing factors 
      if (curTerm.factors.Any() && CompareFactors(curTerm.factors.Last(), curFactor) > 0)
        invalidExpression = true;
      curTerm.factors.Add(curFactor);
      curFactor = null;
    }

    public void EndTerm() {
      // enforce non-decreasing terms (TODO: equal terms should not be allowed)
      if (prevTerm != null && CompareTerms(prevTerm, curTerm) > 0)
        invalidExpression = true;
      prevTerm = curTerm;
      curTerm = null;
    }
  }
}
