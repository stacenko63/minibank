FROM mcr.microsoft.com/dotnet/sdk:5.0 AS src

WORKDIR /src


COPY . .

RUN dotnet build Minibank.Web -c Release -r linux-x64

RUN dotnet test Minibank.Core.Tests --no-build 

RUN dotnet pushish Minibank.Web -c Release -r linux-x64 --no-build -o /dist



FROM mcr.microsoft.com/dotnet/aspnet:5.0 as final

WORKDIR /app

COPY --from=src /dist .

ENV ASPNETCORE_URLS=http://localhost:5001;http://localhost:5000

EXPOSE 5000 5001

ENTRYPOINT ["dotnet", "Minibank.Web.dll"]
