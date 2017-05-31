for %%i in ("%ProjectDir%Protos\*.proto") do (
  echo Processing %%i
  ProtoGen --proto_path="%SolutionDir%\" "%%i" --include_imports -output_directory="%ProjectDir%Protos"
)