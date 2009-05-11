using System.IO;

namespace HeuristicLab.Visualization.DataExport {
  public class CSVWriter {
    private readonly StreamWriter sw;

    private string seperator = ";";
    private string stringQuotes = "\"";

    private bool firstInLine = true;

    public CSVWriter(StreamWriter sw) {
      this.sw = sw;
    }

    public void AddString(string text) {
      Add(stringQuotes + text + stringQuotes);
    }

    public void AddNumber(double value) {
      Add(value.ToString());
    }

    public void AddEmpty() {
      Add("");
    }

    private void Add(string text) {
      if (!firstInLine)
        sw.Write(seperator);

      sw.Write(text);

      firstInLine = false;
    }

    public void NewLine() {
      sw.WriteLine();
      firstInLine = true;
    }

    public string Seperator {
      get { return seperator; }
      set { seperator = value; }
    }

    public string StringQuotes {
      get { return stringQuotes; }
      set { stringQuotes = value; }
    }
  }
}