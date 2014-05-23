@echo off

if .%1.==.. goto END

call deldir %1\obj
call deldir %1\bin

:END