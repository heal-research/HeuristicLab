using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Custom json writer for own formatting for templates.
  /// It collapses arrays into a single line.
  /// </summary>
  public class SingleLineArrayJsonWriter : JsonTextWriter {
    private int arrayDepth = 0;
    public override void WriteStartArray() {
      if (arrayDepth > 0) 
        base.Formatting = Formatting.None;
      arrayDepth++;
      base.WriteStartArray();
    }

    public override void WriteEndArray() {
      arrayDepth--;
      base.WriteEndArray();
    }

    public override void WritePropertyName(string name) {
      base.Formatting = Formatting.Indented;
      base.WritePropertyName(name);
    }

    public override void WriteStartObject() {
      base.Formatting = Formatting.Indented;
      base.WriteStartObject();
    }

    public override void WriteEndObject() {
      base.Formatting = Formatting.Indented;
      base.WriteEndObject();
    }

    public SingleLineArrayJsonWriter(TextWriter writer) : base(writer) { }

    public static string Serialize(JToken token) {
      JsonSerializer serializer = new JsonSerializer();
      StringWriter sw = new StringWriter();
      SingleLineArrayJsonWriter writer = new SingleLineArrayJsonWriter(sw);
      writer.Formatting = Formatting.Indented;
      serializer.Serialize(writer, token);
      return sw.ToString();
    }
  }
}
