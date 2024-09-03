# AngularApp1 Project

This project is a .NET and Angular application that can be built and run using Docker. The application is designed to work with a MongoDB database and can be accessed locally via `localhost:8080`.

## Requirements

- Docker: Make sure Docker is installed on your system. You can download it from [Docker's official website](https://www.docker.com/products/docker-desktop).
- Docker Compose: This project requires Docker Compose, which is included with Docker Desktop.

## Building and Running the Application

To build and run the application in detached mode, use the following command in the root directory of the project (where your `docker-compose.yml` file is located):

```bash
docker-compose up --build -d
```

Accessing the Application
Once the application is running, you can access it in your web browser at:

```bash
http://localhost:8080
```
