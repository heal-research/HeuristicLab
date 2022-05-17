using Newtonsoft.Json;

namespace HEAL.Hive.RestClient.HiveRestClient {
  partial class HiveRestClient {
    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings) {
      settings.Converters.Add(new Newtonsoft.Json.Converters.VersionConverter());
    }
  }
}

