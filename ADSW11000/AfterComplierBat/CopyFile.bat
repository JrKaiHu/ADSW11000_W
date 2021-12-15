@title 複製檔案到指定地方
@echo off
SET sourcefile=%1 
SET destfile=%2
mkdir %2
copy %1 %2
@echo on