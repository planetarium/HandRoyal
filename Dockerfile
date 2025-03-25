# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Define a build argument for the build configuration
ARG BUILD_CONFIGURATION=Release

# Copy the entire source code and build the application
COPY ./src ./HandRoyal/src
COPY ./.editorconfig ./HandRoyal
COPY ./Directory.Build.props ./HandRoyal
COPY ./Menees.Analyzers.Settings.xml ./HandRoyal
COPY ./stylecop.json ./HandRoyal
COPY ./.submodules/Libplanet ./HandRoyal/.submodules/Libplanet

RUN dotnet restore ./HandRoyal/src/HandRoyal.Node/HandRoyal.Node.csproj
RUN dotnet build --no-restore --configuration $BUILD_CONFIGURATION \
    ./HandRoyal/src/HandRoyal.Node/HandRoyal.Node.csproj
RUN dotnet publish --no-restore --no-build \
    ./HandRoyal/src/HandRoyal.Node/HandRoyal.Node.csproj \
    --configuration $BUILD_CONFIGURATION --output /out

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /out ./

# Expose the port for the application
EXPOSE 8080

# Set the entry point for the application
ENTRYPOINT ["dotnet", "HandRoyal.Node.dll"]
