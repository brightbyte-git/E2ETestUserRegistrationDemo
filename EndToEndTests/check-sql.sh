#!/bin/bash

echo "Waiting for SQL Server to be available..."
until /opt/mssql-tools/bin/sqlcmd -S db,1433 -U sa -P "VeryStr0ngP@ssw0rd" -Q "SELECT 1" &>/dev/null; do
  echo "SQL Server is not ready yet. Retrying in 5 seconds..."
  sleep 5
done

echo "SQL Server is up. Proceeding..."
exec "$@"