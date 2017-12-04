"..\tools\nuget-binary\NuGet.exe" restore ..\src\Syntactik.sln
MSBuild.exe ..\src\Syntactik.sln /m /p:Configuration=Release /p:Platform="Any CPU"