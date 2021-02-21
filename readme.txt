To build docker :

docker build . -t images

To run built image:

docker run -d -p 8080:80 images

To run composed file:

docker-compose up
docker-compose up --scale imagefetcher=2

sample query:

http://localhost:8080/api/images?file=01_04_2019_001103.png&

