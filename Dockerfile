FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
EXPOSE 80

COPY /aiof.asset.core /app/
ENTRYPOINT ["dotnet", "aiof.asset.core.dll"]