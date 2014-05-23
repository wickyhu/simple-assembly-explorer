@echo off
for /d /r %%c in (.svn) do if exist %%c (
	echo Removing %%c
	rd /s /q %%c 
)
