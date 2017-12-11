# WebGIS

1
## Docker CMDs
git clone https://github.com/johnmaloney/WebGIS.git

 docker build ./WebGIS/GeospatialAPI/ -t geo

docker run -d -p 8080:5000 geo

docker inspect geo

## Docker clean up CMDs
docker stop $(docker ps -a -q)

docker rmi -f $(docker images)

docker rm $(docker ps -a -q)

