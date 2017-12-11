FROM microsoft/dotnet:2.0-sdk
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.0-sdk
WORKDIR /app
COPY ./ ./app/out
CMD ["GeoSpatialAPI.dll"]

