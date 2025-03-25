FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

ARG CONFIGURATION=Release

COPY ./src ./HandRoyal/src
COPY ./.editorconfig ./HandRoyal
COPY ./Directory.Build.props ./HandRoyal
COPY ./Menees.Analyzers.Settings.xml ./HandRoyal
COPY ./stylecop.json ./HandRoyal
COPY ./.submodules/Libplanet ./HandRoyal/.submodules/Libplanet

RUN dotnet publish \
    --configuration $CONFIGURATION \
    --output /out \
    --runtime linux-x64 \
    ./HandRoyal/src/HandRoyal.Node/HandRoyal.Node.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "HandRoyal.Node.dll"]
