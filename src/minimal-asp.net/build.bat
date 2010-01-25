if not exist bin mkdir bin
booc -debug- -target:library -r:System.Web -o:bin\Example.dll HttpHandler.boo
REM gmcs -debug- -target:library -reference:System.Web -out:bin\Example.dll HttpHandler.cs
REM csc /debug- /target:library /reference:System.Web.dll /out:bin\Example.dll HttpHandler.cs