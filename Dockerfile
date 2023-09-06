# Stage 1: Define base image that will be used for production
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
# EXPOSE 80
# EXPOSE 443
EXPOSE 5000

#Environment Variable
LABEL maintainer="abbas0324"
# ENV ASPNETCORE_URLS=http://*:80
ENV ASPNETCORE_URLS=http://+:5000
# ENV ASPNETCORE_ENVIRONMENT="production"
ENV ASPNETCORE_ENVIRONMENT="QA"

# Stage 2: Build and publish the code
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Authority-STS/Code/AuthoritySTS.csproj", ""]
#RUN dotnet restore "Authority-STS/Code/AuthoritySTS.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Authority-STS/Code/AuthoritySTS.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "Authority-STS/Code/AuthoritySTS.csproj" -c Release -o /app/publish

# Stage 3: Run the code
FROM base AS final
WORKDIR /app
# COPY ./outputs . 
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthoritySTS.dll", "--server.urls", "https://+:5000"]
# ENTRYPOINT ["dotnet", "AuthoritySTS.dll", "--server.urls", "http://+:80;https://+:443"]
# ENTRYPOINT ["dotnet", "AuthoritySTS.dll"]

# docker build . -t triggerauthority-local 
# docker run --rm -p 5000:5000 triggerauthority-local 

# docker build -t triggerauthority .
# docker run --rm -p 80:5000 -d triggerauthority:latest

# docker build . -t triggerauthority
# docker run --rm --net triggerredevelopbridge --name triggerauthority -p 5000:5000 -d triggerauthority:latest

# docker build . -t triggerauthority
# docker run --rm --net triggerredevelopbridge --name triggerauthority -p 5000:5000 -d triggerauthority:latest