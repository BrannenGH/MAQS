FROM mcr.microsoft.com/powershell:latest


RUN apt-get update && apt-get install openssh-server -y --no-install-recommends && echo "root:Magenic" | chpasswd

COPY ./ssh/* /etc/ssh/
COPY ./authorized_keys/docker_linux.rsa.pub /root/.ssh/authorized_keys
RUN chmod 600 /etc/ssh/* && chmod 600 /root/.ssh/authorized_keys

CMD ["/usr/bin/pwsh"]