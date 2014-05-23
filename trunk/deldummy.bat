@echo off

call delfile %1\*.pdb
call delfile %1\ilmerge.exe
call delfile %1\Reflector.exe
call delfile %1\*.vshost.*
call delfile %1\*.exp
call delfile %1\*.lib
call delfile %1\Interop.IWshRuntimeLibrary.dll
call delfile %1\SimpleAssemblyExplorer.vshost.*

call delfile %1\Plugins\*.pdb
call delfile %1\Plugins\Interop.IWshRuntimeLibrary.dll
call delfile %1\Plugins\SimpleAssemblyExplorer.Plugin.*
call delfile %1\Plugins\SimpleUtils.*
call delfile %1\Plugins\ILMerge.*

