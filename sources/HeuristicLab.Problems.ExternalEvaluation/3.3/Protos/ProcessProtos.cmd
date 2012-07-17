for %%i in ("%ProjectDir%Protos\*.proto") do (
  echo Processing %%i
  ProtoGen --proto_path="%ProjectDir%\Protos" %%i --include_imports -output_directory="%ProjectDir%Protos"
)