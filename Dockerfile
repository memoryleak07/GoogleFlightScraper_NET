#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base

# Source: https://github.com/mbruntink/lambda-scraper/tree/main/lambda/scraper
# install chrome dependecies
#RUN yum install unzip atk at-spi2-atk gtk3 cups-libs pango libdrm \ 
    #libXcomposite libXcursor libXdamage libXext libXtst libXt \
    #libXrandr libXScrnSaver alsa-lib -y
# Install necessary dependencies using apt-get
USER root
RUN apt-get update && apt-get install -y \
    curl \
    unzip \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libgtk-3-0 \
    libgtk-3-common \
    libpango-1.0-0 \
    libdrm2 \
    libxcomposite1 \
    libxcursor1 \
    libxdamage1 \
    libxext6 \
    libxtst6 \
    libxt6 \
    libxrandr2 \
    libxss1 \
    libasound2 \
    libglib2.0-0 \
    libnss3-dev \
    && rm -rf /var/lib/apt/lists/*

# Install chromium, chrome-driver
# VERSIONS
# https://omahaproxy.appspot.com/
ARG CHROMIUM_VERSION="105.0.5195.125"
ARG CHROMIUM_BASE_POSITION="1027016"
ARG CHROMIUM_URL="https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Linux_x64%2F$CHROMIUM_BASE_POSITION%2Fchrome-linux.zip?alt=media"

ARG CHROME_DRIVER_VERSION="105.0.5195.52"
ARG CHROME_DRIVER_URL="https://chromedriver.storage.googleapis.com/$CHROME_DRIVER_VERSION/chromedriver_linux64.zip"

# install chromium
RUN mkdir -p "/opt/chromium"
RUN curl -Lo "/opt/chromium/chrome-linux.zip" $CHROMIUM_URL
RUN unzip -q "/opt/chromium/chrome-linux.zip" -d "/opt/chromium"
RUN mv /opt/chromium/chrome-linux/* /opt/chromium/

# install chrome-driver
RUN mkdir -p "/opt/chromedriver"
RUN curl -Lo "/opt/chromedriver/chromedriver_linux64.zip" $CHROME_DRIVER_URL
RUN unzip -q "/opt/chromedriver/chromedriver_linux64.zip" -d "/opt/chromedriver"
RUN chmod +x "/opt/chromedriver/chromedriver"

# cleanup
RUN rm -rf "/opt/chromium/chrome-linux" "/opt/chromium/chrome-linux.zip"
RUN rm -rf "/opt/chromedriver/chromedriver_linux64.zip" 

# Remove unused packages
RUN apt-get remove unzip -y



#USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GFS_NET.csproj", "."]
RUN dotnet restore "./GFS_NET.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./GFS_NET.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GFS_NET.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GFS_NET.dll"]