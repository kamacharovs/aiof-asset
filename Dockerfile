FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY /app/publish/aiof.asset.core /app/
EXPOSE 80
ENTRYPOINT ["dotnet", "aiof.asset.core.dll"]