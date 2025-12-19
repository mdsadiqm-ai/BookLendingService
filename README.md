# Book Lending Service API

## Overview
This repository contains a .NET 8 Web API for a fictional Book Lending Service. It supports adding books, listing books, checking out books, and returning books. The service is containerized with Docker and includes Terraform infrastructure as code for an AWS ECS Fargate deployment.

## Features
1. POST /books to add a book
2. GET /books to list books
3. POST /books/{id}/checkout to check out a book
4. POST /books/{id}/return to return a book
5. GET /healthz for a simple health check
6. Swagger UI at the root path in Development

## Tech choices and trade offs
1. Persistence uses SQLite via Entity Framework Core by default for a realistic local persistence story.
2. An in memory repository is included for unit tests and for easy switching if desired.
3. The domain model uses optimistic concurrency via a RowVersion to protect against double checkout when multiple callers race.
4. For book table i am only asking for basic book information like Title & Author but i could have easily capture other important details too like:
   PublicationYear, Edition, Publisher, Language, & PageCount but i decided not to given the scale of this project time to 3 hours :) 
5. I have also added lock on checkout to avoid concurrency exception if more than one person try to checkout at the same time.
6. I have not implemented any authentication/authorization for these endpoints, given the 3 hour timeline.
7. I could have implemented the link table for checkout history information which could help us in generating reports like how many times a book got checked out this year, so we know the popularity of a book. There are so many of those features we could think and implement given 3 hrs window, I decided not to as a trade off
   

## Local run with .NET
Prerequisites
1. .NET SDK 8
2. Optional: Docker

Steps
1. From repository root, run:
   dotnet restore src/BookLendingService.Api/BookLendingService.Api.csproj
2. Run:
   dotnet run --project src/BookLendingService.Api/BookLendingService.Api.csproj
3. Open:
   http://localhost:63078/swagger
Swagger UI Images:
   <img width="1515" height="786" alt="image" src="https://github.com/user-attachments/assets/0265f836-01c5-4136-ba83-883de316f31b" />

   books added to database:
   <img width="1415" height="594" alt="image" src="https://github.com/user-attachments/assets/296b13a4-a5f4-44c0-8254-10b57a874785" />



## Local run with Docker
1. Build image:
   docker build -t booklendingservice:local .
2. Run container:
   docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development booklendingservice:local
3. Open:
   http://localhost:8080/swagger

## Local run with Docker Compose
1. Run:
   docker compose up --build
2. Open:
   http://localhost:8080/swagger

## API examples
Add a book
curl -X POST http://localhost:8080/books \
  -H "Content-Type: application/json" \
  -d '{ "title": "Clean Code", "author": "Robert C Martin" }'

List books
curl http://localhost:8080/books

Checkout a book
curl -X POST http://localhost:8080/books/{id}/checkout

Return a book
curl -X POST http://localhost:8080/books/{id}/return

## AWS deploy approach
Target platform
1. AWS ECS with Fargate
2. Application Load Balancer
3. CloudWatch logs
4. Terraform as infrastructure as code

High level steps
1. Build and push a container image to ECR (or your registry of choice).
2. Set the Terraform variables for image URI and region.
3. Apply Terraform to provision ECS, ALB, and networking dependencies.

Terraform quick start
1. Change directory:
   cd infra/terraform
2. Initialize:
   terraform init
3. Plan:
   terraform plan -var="container_image=<your image uri>"
4. Apply:
   terraform apply -var="container_image=<your image uri>"

Notes
1. The Terraform stack is designed to work with a default VPC to reduce resource noise for a technical test.
2. You can extend it to provision a dedicated VPC if required.

## Tests
1. Run:
   dotnet test tests/BookLendingService.Tests/BookLendingService.Tests.csproj
