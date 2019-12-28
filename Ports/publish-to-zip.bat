del /s /q Ports.zip
rmdir /s /q Ports
md Ports
xcopy .\bin\Release .\Ports\
"C:\Program Files\WinRAR\WinRAR.exe" a Ports.zip .\Ports
rmdir /s /q Ports
pause