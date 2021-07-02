FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
EXPOSE 80

COPY /app/publish/aiof.asset.core /app/
ENTRYPOINT ["dotnet", "aiof.asset.core.dll"]