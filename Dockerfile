FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY AccidentesMadrid.Back/AccidentesMadrid.Back.csproj AccidentesMadrid.Back/
RUN dotnet restore AccidentesMadrid.Back/AccidentesMadrid.Back.csproj

COPY AccidentesMadrid.Back/ AccidentesMadrid.Back/
RUN dotnet publish AccidentesMadrid.Back/AccidentesMadrid.Back.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AccidentesMadrid.Back.dll"]
