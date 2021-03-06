using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoDiff
{
    partial class CompiledDifferentiator<T>
    {
        private class EvalVisitor : Compiled.ITapeVisitor
        {
            private readonly Compiled.TapeElement[] tape;

            public EvalVisitor(Compiled.TapeElement[] tape)
            {
                this.tape = tape;
            }

            public void Visit(Compiled.Constant elem)
            {
            }

            public void Visit(Compiled.Exp elem)
            {
                elem.Value = Math.Exp(ValueOf(elem.Arg));
            }

            public void Visit(Compiled.Log elem)
            {
                elem.Value = Math.Log(ValueOf(elem.Arg));
            }

            public void Visit(Compiled.ConstPower elem)
            {
                elem.Value = Math.Pow(ValueOf(elem.Base), elem.Exponent);
            }

            public void Visit(Compiled.TermPower elem)
            {
                elem.Value = Math.Pow(ValueOf(elem.Base), ValueOf(elem.Exponent));
            }

            public void Visit(Compiled.Product elem)
            {
                elem.Value = ValueOf(elem.Left) * ValueOf(elem.Right);
            }

            public void Visit(Compiled.Sum elem)
            {
                elem.Value = 0;
                for (int i = 0; i < elem.Terms.Length; ++i)
                    elem.Value += ValueOf(elem.Terms[i]);
            }

            public void Visit(Compiled.Variable var)
            {
            }

            public void Visit(Compiled.UnaryFunc elem)
            {
                elem.Value = elem.Eval(ValueOf(elem.Arg));
            }

            public void Visit(Compiled.BinaryFunc elem)
            {
                elem.Value = elem.Eval(ValueOf(elem.Left), ValueOf(elem.Right));
            }

            public void Visit(Compiled.NaryFunc elem)
            {
                double[] args = new double[elem.Terms.Length];
                for(int i=0;i<args.Length;i++)
                    args[i] = ValueOf(elem.Terms[i]);
                elem.Value = elem.Eval(args);
            }

            private double ValueOf(int index)
            {
                return tape[index].Value;
            }


        }
    }
}
