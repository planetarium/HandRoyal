FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /app

COPY ./src ./HandRoyal/src
COPY ./.editorconfig ./HandRoyal
COPY ./Directory.Build.props ./HandRoyal
COPY ./Menees.Analyzers.Settings.xml ./HandRoyal
COPY ./stylecop.json ./HandRoyal
COPY ./.submodules/Libplanet ./HandRoyal/.submodules/Libplanet
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
RUN dotnet restore \
    --arch $TARGETARCH \
    ./HandRoyal/src/HandRoyal.Executable/HandRoyal.Executable.csproj

RUN dotnet publish \
    --no-restore \
    --configuration Release \
    --output /out \
    --arch $TARGETARCH \
    ./HandRoyal/src/HandRoyal.Executable/HandRoyal.Executable.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /out ./

VOLUME /app/.logs
VOLUME /app/.store

EXPOSE 8080

ENTRYPOINT ["dotnet", "HandRoyal.Executable.dll"]
