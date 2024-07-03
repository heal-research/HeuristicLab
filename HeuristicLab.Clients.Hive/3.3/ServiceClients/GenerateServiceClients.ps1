#dotnet tool install --global dotnet-svcutil

dotnet-svcutil https://services.heuristiclab.com/Hive-3.4/HiveService.svc `
  --outputFile HiveServiceClient.cs `
  --outputDir . `
  --namespace *,HeuristicLab.Clients.Hive `
  --collectionType 'System.Collections.Generic.List`1' `
  --serializer DataContractSerializer `
  --sync `
  --enableDataBinding
