FROM mcr.microsoft.com/mssql/server:latest

COPY . .

ENV ACCEPT_EULA Y
ENV SA_PASSWORD magenicMAQS2
ENV MSSQL_PID Developer

# Elevate ourselves to root to handle permissions
USER root
RUN chmod +x ./initialize_and_start_sqlserver.sh

USER mssql
RUN /bin/bash ./initialize_and_start_sqlserver.sh

CMD /opt/mssql/bin/sqlservr