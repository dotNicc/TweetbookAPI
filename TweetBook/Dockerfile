FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY TweetBook.csproj /build/

RUN dotnet restore ./build/TweetBook.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./TweetBook.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "TweetBook.dll"]