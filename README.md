#Setup docker postgres db container
docker run --name dev_db -e POSTGRES_USER=adminuser -e POSTGRES_PASSWORD=secret -p 5432:5432 -d postgres:latest

#drop database
dotnet ef database drop -p Persistence -s API

#create new dotnet ef migrations
dotnet ef migrations add PostgresInitial -p Persistence -s API

#run API, cd into API folder
dotnet run or dotnet watch run


#stop the running API, cd into root folder, run docker build to build the image
docker build -t basiljungpapilio/reactivities . 

#run the image
docker run --rm -it -p 8080:80 basiljungpapilio/reactivities

#push docker image to docker-hub (login to docker)
docker push basiljungpapilio/reactivities:latest