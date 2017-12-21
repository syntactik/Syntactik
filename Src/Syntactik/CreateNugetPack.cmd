cd ..\..\build\
call winbuild.bat
cd ..\src\syntactik
del syntactik*.nupkg
nuget pack Syntactik.csproj -Properties Configuration=Release