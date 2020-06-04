#!/usr/bin/env pwsh
service ssh restart

Start-Transcript -Path logs.txt

Get-Content -Wait logs.txt