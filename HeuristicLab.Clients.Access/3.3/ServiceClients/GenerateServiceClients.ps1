#dotnet tool install --global dotnet-svcutil

dotnet-svcutil https://services.heuristiclab.com/AccessService-3.3/AccessService.svc `
  --outputFile AccessServiceClient.cs `
  --outputDir . `
  --namespace *,HeuristicLab.Clients.Access `
  --collectionType 'System.Collections.Generic.List`1' `
  --serializer DataContractSerializer `
  --sync `
  --enableDataBinding