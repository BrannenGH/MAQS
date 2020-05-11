#!/usr/bin/env pwsh

Start-Transcript -Path logs.txt

# TODO: Find or make container that has PowerShell and SSH already installed.
apt-get update
apt-get install openssh-server -y --no-install-recommends

# Set root password ot Magenic
echo "root:Magenic" | chpasswd

service ssh restart

cat /etc/syslog.conf | grep -i ssh

Get-ChildItem /var/log/

Get-Content -Wait logs.txt