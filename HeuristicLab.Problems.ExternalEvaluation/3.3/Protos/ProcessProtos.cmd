cd "%ProjectDir%"Protos

for %%i in (*.proto) do (
  "%SolutionDir%"protoc -otmp %%i
  "%SolutionDir%"ProtoGen tmp
  del tmp)