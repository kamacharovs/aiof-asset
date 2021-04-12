# Overview

All in one finance asset microservice

[![Build Status](https://dev.azure.com/gkamacharov/gkama-cicd/_apis/build/status/kamacharovs.aiof-asset?branchName=main)](https://dev.azure.com/gkamacharov/gkama-cicd/_build/latest?definitionId=26&branchName=main)

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

### Docker compose

From the project root directory

```ps
docker-compose up
```
