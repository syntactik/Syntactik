cd ..
call winbuild.bat
cd syntactik
del syntactik*.nupkg
nuget pack Syntactik.csproj -Properties Configuration=Release