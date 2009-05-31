using System;
using System.Drawing;

namespace HeuristicLab.Visualization{
  public class ChartDataRowsModelDummy : ChartDataRowsModel  {
    
    public ChartDataRowsModelDummy(){
      Random rand = new Random();

      // test rows

      // TODO change to call new DataRow("Datarow 1");
      DataRow row1 = new DataRow();
      row1.RowSettings.Label = "Datarow 1";
      row1.RowSettings.Color = Color.Red;
      DataRow row2 = new DataRow();
      row2.RowSettings.Label = "Datarow 2";
      row2.RowSettings.Color = Color.Blue;
      DataRow row3 = new DataRow();
      row3.RowSettings.Label = "Datarow 3";
      row3.RowSettings.Color = Color.DeepPink;

      AddDataRow(row1);
      AddDataRow(row2);
      AddDataRow(row3);
      
      for (int i = 0; i < 10; i++){

        // TODO Test AddDataRow mit bereits befüllter Row

        row1.AddValue(rand.NextDouble() * 100);
        row2.AddValues(new double[] { rand.NextDouble() * 100,  
          rand.NextDouble() * 100 });
        row3.AddValue(11.0);
        // TODO after implemention of modifyValue: row1.ModifyValue(row1.getval[i]+100, i);
      }
    }
  }
}