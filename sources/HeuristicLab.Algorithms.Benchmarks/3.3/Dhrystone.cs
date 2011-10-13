using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.Benchmarks {
  [Item("Dhrystone Algorithm", "A Dhrystone benchmark algorithm.")]
  [Creatable("Benchmarks")]
  [StorableClass]
  public class Dhrystone : Algorithm {
    private DateTime lastUpdateTime;

    [Storable]
    private ResultCollection results;

    #region Benchmark Fields

    private const int Ident_1 = 0;
    private const int Ident_2 = 1;
    private const int Ident_3 = 2;
    private const int Ident_4 = 3;
    private const int Ident_5 = 4;

    private Record_Type Record_Glob;
    private Record_Type Next_Record_Glob;

    private int Int_Glob;
    private Boolean Bool_Glob;

    private char Char_Glob_1;
    private char Char_Glob_2;

    private int[] Array_Glob_1 = new int[128];
    private int[][] Array_Glob_2 = new int[128][];

    private Record_Type First_Record = new Record_Type();
    private Record_Type Second_Record = new Record_Type();

    private long Number_Of_Runs = 10000000;

    #endregion

    #region Properties

    public override ResultCollection Results {
      get { return results; }
    }

    #endregion

    #region Costructors

    public Dhrystone()
      : base() {
      results = new ResultCollection();
    }

    private Dhrystone(Dhrystone original, Cloner cloner)
      : base(original, cloner) {
      results = new ResultCollection();
    }

    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Dhrystone(this, cloner);
    }

    public override void Prepare() {
      results.Clear();
      OnPrepared();
    }

    public override void Start() {
      var cancellationTokenSource = new CancellationTokenSource();
      OnStarted();
      Task task = Task.Factory.StartNew(Run, cancellationTokenSource.Token, cancellationTokenSource.Token);
      task.ContinueWith(t => {
        try {
          t.Wait();
        }
        catch (AggregateException ex) {
          try {
            ex.Flatten().Handle(x => x is OperationCanceledException);
          }
          catch (AggregateException remaining) {
            if (remaining.InnerExceptions.Count == 1) OnExceptionOccurred(remaining.InnerExceptions[0]);
            else OnExceptionOccurred(remaining);
          }
        }
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
        OnStopped();
      });
    }

    private void Run(object state) {
      CancellationToken cancellationToken = (CancellationToken)state;
      lastUpdateTime = DateTime.Now;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        RunBenchmark();
      }
      finally {
        timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Stop();
        ExecutionTime += DateTime.Now - lastUpdateTime;
      }

      cancellationToken.ThrowIfCancellationRequested();
    }

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      System.Timers.Timer timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.Now;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }

    #region Dhrystone Benchmark

    private void RunBenchmark() {
      int Int_Loc_1;
      int Int_Loc_2;
      int Int_Loc_3;

      int[] Int_Loc_1_Ref = new int[1];
      int[] Int_Loc_3_Ref = new int[1];

      char Char_Index;
      int[] Enum_Loc = new int[1];

      string String_Loc_1;
      string String_Loc_2;

      long total_time;

      int Run_Index;

      Next_Record_Glob = Second_Record;
      Record_Glob = First_Record;

      Record_Glob.Record_Comp = Next_Record_Glob;
      Record_Glob.Discr = Ident_1;
      Record_Glob.Enum_Comp = Ident_3;
      Record_Glob.Int_Comp = 40;
      Record_Glob.String_Comp = "DHRYSTONE PROGRAM, SOME STRING";
      String_Loc_1 = "DHRYSTONE PROGRAM, 1'ST STRING";

      for (int i = 0; i < 128; i++) {
        Array_Glob_2[i] = new int[128];
      }

      //System.Console.WriteLine("Execution start, " + Number_Of_Runs + " runs through Dhrystone");
      Stopwatch sw = new Stopwatch();
      sw.Start();


      for (Run_Index = 1; Run_Index <= Number_Of_Runs; ++Run_Index) {
        Proc_5();
        Proc_4();

        Int_Loc_1 = 2;
        Int_Loc_2 = 3;

        String_Loc_2 = "DHRYSTONE PROGRAM, 2'ND STRING";

        Enum_Loc[0] = Ident_2;
        Bool_Glob = !Func_2(String_Loc_1, String_Loc_2);

        while (Int_Loc_1 < Int_Loc_2) {
          Int_Loc_3_Ref[0] = 5 * Int_Loc_1 - Int_Loc_2;
          Proc_7(Int_Loc_1, Int_Loc_2, Int_Loc_3_Ref);
          Int_Loc_1 += 1;
        }

        Int_Loc_3 = Int_Loc_3_Ref[0];
        Proc_8(Array_Glob_1, Array_Glob_2, Int_Loc_1, Int_Loc_3);
        Proc_1(Record_Glob);

        for (Char_Index = 'A'; Char_Index <= Char_Glob_2; ++Char_Index) {
          if (Enum_Loc[0] == Func_1(Char_Index, 'C'))
            Proc_6(Ident_1, Enum_Loc);
        }

        Int_Loc_3 = Int_Loc_2 * Int_Loc_1;
        Int_Loc_2 = Int_Loc_3 / Int_Loc_1;
        Int_Loc_2 = 7 * (Int_Loc_3 - Int_Loc_2) - Int_Loc_1;

        Int_Loc_1_Ref[0] = Int_Loc_1;
        Proc_2(Int_Loc_1_Ref);
        Int_Loc_1 = Int_Loc_1_Ref[0];
      }

      sw.Stop();
      total_time = sw.ElapsedMilliseconds;
      //System.Console.WriteLine("total time: {0} ms", total_time);
      //System.Console.WriteLine("Result: " + Number_Of_Runs * 1000 / total_time + " dhrystone/ms.");

      Results.Add(new Result("DIPS", new DoubleValue(Number_Of_Runs * 1000 / total_time)));
    }

    private int Func_1(char Char_Par_1_Val, char Char_Par_2_Val) {
      char Char_Loc_1;
      char Char_Loc_2;

      Char_Loc_1 = Char_Par_1_Val;
      Char_Loc_2 = Char_Loc_1;
      if (Char_Loc_2 != Char_Par_2_Val)
        return Ident_1;
      else
        return Ident_2;
    }

    private bool Func_2(string String_Par_1_Ref, string String_Par_2_Ref) {
      int Int_Loc;
      char Char_Loc = '\0';

      Int_Loc = 2;

      while (Int_Loc <= 2) {
        if (Func_1(String_Par_1_Ref[Int_Loc], String_Par_2_Ref[Int_Loc + 1]) == Ident_1) {
          Char_Loc = 'A';
          Int_Loc += 1;
        }
      }
      if (Char_Loc >= 'W' && Char_Loc < 'z')
        Int_Loc = 7;
      if (Char_Loc == 'X')
        return true;
      else {
        if (String_Par_1_Ref.CompareTo(String_Par_2_Ref) > 0) {
          Int_Loc += 7;
          return true;
        } else
          return false;
      }
    }

    private bool Func_3(int Enum_Par_Val) {
      int Enum_Loc;

      Enum_Loc = Enum_Par_Val;
      if (Enum_Loc == Ident_3)
        return true;
      else
        return false;
    }

    private void Proc_1(Record_Type Pointer_Par_Val) {
      Record_Type Next_Record = Pointer_Par_Val.Record_Comp;

      Pointer_Par_Val.Record_Comp = Record_Glob;

      Pointer_Par_Val.Int_Comp = 5;

      Next_Record.Int_Comp = Pointer_Par_Val.Int_Comp;
      Next_Record.Record_Comp = Pointer_Par_Val.Record_Comp;
      Proc_3(Next_Record.Record_Comp);

      int[] Int_Ref = new int[1];

      if (Next_Record.Discr == Ident_1) {
        Next_Record.Int_Comp = 6;
        Int_Ref[0] = Next_Record.Enum_Comp;
        Proc_6(Pointer_Par_Val.Enum_Comp, Int_Ref);
        Next_Record.Enum_Comp = Int_Ref[0];
        Next_Record.Record_Comp = Record_Glob.Record_Comp;
        Int_Ref[0] = Next_Record.Int_Comp;
        Proc_7(Next_Record.Int_Comp, 10, Int_Ref);
        Next_Record.Int_Comp = Int_Ref[0];
      } else
        Pointer_Par_Val = Pointer_Par_Val.Record_Comp;
    }

    private void Proc_2(int[] Int_Par_Ref) {
      int Int_Loc;
      int Enum_Loc;

      Int_Loc = Int_Par_Ref[0] + 10;
      Enum_Loc = 0;

      do
        if (Char_Glob_1 == 'A') {
          Int_Loc -= 1;
          Int_Par_Ref[0] = Int_Loc - Int_Glob;
          Enum_Loc = Ident_1;
        }
      while (Enum_Loc != Ident_1);
    }

    private void Proc_3(Record_Type Pointer_Par_Ref) {
      if (Record_Glob != null)
        Pointer_Par_Ref = Record_Glob.Record_Comp;
      else
        Int_Glob = 100;

      int[] Int_Comp_Ref = new int[1];
      Int_Comp_Ref[0] = Record_Glob.Int_Comp;
      Proc_7(10, Int_Glob, Int_Comp_Ref);
      Record_Glob.Int_Comp = Int_Comp_Ref[0];
    }

    private void Proc_4() {
      bool Bool_Loc;

      Bool_Loc = Char_Glob_1 == 'A';
      Bool_Loc = Bool_Loc || Bool_Glob;
      Char_Glob_2 = 'B';
    }

    private void Proc_5() {
      Char_Glob_1 = 'A';
      Bool_Glob = false;
    }

    private void Proc_6(int Enum_Par_Val, int[] Enum_Par_Ref) {

      Enum_Par_Ref[0] = Enum_Par_Val;

      if (!Func_3(Enum_Par_Val))
        Enum_Par_Ref[0] = Ident_4;

      switch (Enum_Par_Val) {
        case Ident_1:
          Enum_Par_Ref[0] = Ident_1;
          break;

        case Ident_2:
          if (Int_Glob > 100)
            Enum_Par_Ref[0] = Ident_1;
          else
            Enum_Par_Ref[0] = Ident_4;
          break;

        case Ident_3:
          Enum_Par_Ref[0] = Ident_2;
          break;

        case Ident_4:
          break;

        case Ident_5:
          Enum_Par_Ref[0] = Ident_3;
          break;
      }
    }

    private void Proc_7(int Int_Par_Val1, int Int_Par_Val2, int[] Int_Par_Ref) {
      int Int_Loc;
      Int_Loc = Int_Par_Val1 + 2;
      Int_Par_Ref[0] = Int_Par_Val2 + Int_Loc;
    }

    private void Proc_8(int[] Array_Par_1_Ref, int[][] Array_Par_2_Ref, int Int_Par_Val_1, int Int_Par_Val_2) {
      int Int_Index;
      int Int_Loc;

      Int_Loc = Int_Par_Val_1 + 5;
      Array_Par_1_Ref[Int_Loc] = Int_Par_Val_2;
      Array_Par_1_Ref[Int_Loc + 1] = Array_Par_1_Ref[Int_Loc];
      Array_Par_1_Ref[Int_Loc + 30] = Int_Loc;

      for (Int_Index = Int_Loc; Int_Index <= Int_Loc + 1; ++Int_Index)
        Array_Par_2_Ref[Int_Loc][Int_Index] = Int_Loc;
      Array_Par_2_Ref[Int_Loc][Int_Loc - 1] += 1;
      Array_Par_2_Ref[Int_Loc + 20][Int_Loc] = Array_Par_1_Ref[Int_Loc];
      Int_Glob = 5;
    }

    #endregion

    #region Private Class

    private class Record_Type {
      public Record_Type Record_Comp;
      public int Discr;
      public int Enum_Comp;
      public int Int_Comp;
      public string String_Comp;
      /*
      public int Enum_Comp_2;
      public string String_Comp_2;
      public char Char_Comp_1;
      public char Char_Comp_2;
       */
    }

    #endregion
  }
}
