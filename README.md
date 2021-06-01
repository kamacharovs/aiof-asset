# Overview

All in one finance asset microservice

[![build](https://github.com/kamacharovs/aiof-asset/actions/workflows/build.yml/badge.svg)](https://github.com/kamacharovs/aiof-asset/actions/workflows/build.yml)
[![Build Status](https://gkamacharov.visualstudio.com/gkama-cicd/_apis/build/status/kamacharovs.aiof-asset?branchName=main)](https://gkamacharov.visualstudio.com/gkama-cicd/_build/latest?definitionId=31&branchName=main)

## How to run it

The recommended way is to use `docker-compose`. That pulls down all the Docker images needed and you will have the full microservices architecture locally in order to get authentication from `aiof-auth` and data from `aiof-data`

### Docker

Pull the latest image

```ps
docker pull gkama/aiof-asset:latest
```

Run it

```ps
docker run -it --rm -p 8000:80 gkama/aiof-asset:latest
```

Make API calls to

```text
http://localhost:8000/
```

(Optional) Clean up `none` images

```ps
docker rmi $(docker images -f “dangling=true” -q)
```

### Docker local

Build the image from `Dockerfile.local`

```ps
docker build -t aiof-api:latest -f Dockerfile.local .
```

### Docker compose

From the project root directory

```ps
docker-compose up
```

## EF migrations

Migrations are currently managed in the `ef-migrations` branch

```ps
dotnet ef migrations add {migration name} -s .\aiof.asset.core -p .\aiof.asset.data
dotnet ef migrations script -s .\aiof.asset.core\ -p .\aiof.asset.data\
dotnet ef migrations remove -s .\aiof.asset.core -p .\aiof.asset.data
```
