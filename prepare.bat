
@echo off
echo Cleaning ...

call clean ILSpy\ICSharpCode.Decompiler
call clean ILSpy\ILSpy.BamlDecompiler

call clean ILSpy\NRefactory\ICSharpCode.NRefactory
call clean ILSpy\NRefactory\ICSharpCode.NRefactory.CSharp
call clean ILSpy\NRefactory\ICSharpCode.NRefactory.VB


call clean Mono.Cecil
call clean Mono.Cecil\Test
call clean Mono.Cecil\rocks


call clean SAE.de4dot
call clean SAE.Deobf9RayHelper
call clean SAE.DeobfPluginSample
call clean SAE.EditFile
call clean SAE.FileDisassembler
call clean SAE.ILMerge
call clean SAE.MethodSearcher
call clean SAE.PluginSample
call clean SAE.Reflector
call clean SimpleAssemblyExplorer.Core
call clean SimpleAssemblyExplorer.plugin

call deldir SimpleProfiler\Debug 
call deldir SimpleProfiler\Release
call deldir SimpleProfiler\Win32
call deldir SimpleProfiler\x64
call delfile SimpleProfiler\SimpleProfiler.ncb

call clean SimpleUtils

call clean TestProject
call delfile TestProject\Assembly\HWISD_nat.dll
call delfile TestProject\Temp\*.*

call clean TestSample

call delfile SimpleAssemblyExplorer.ncb
call delfile SimpleAssemblyExplorer.sdf

call deldir SimpleAssemblyExplorer\obj
call deldir SimpleAssemblyExplorer\bin\x86
call deldir SimpleAssemblyExplorer\bin\x64
call delfile SimpleAssemblyExplorer\bin\debug\*.*
call delfile SimpleAssemblyExplorer\bin\debug\Plugins\*.*

call deldummy SimpleAssemblyExplorer\bin\Release

echo.
echo Checking Version ...

support\AppVer SimpleAssemblyExplorer\bin\release\SimpleAssemblyExplorer.exe sae.autoupdate.xml

echo.
pause