# WebGIS

This is a project to discover the possibilities of a publisher/subscriber architecture using the Azure Service Bus and Docker containers. It came from a class at PSU for the MGIS program, the term paper describing the purpose is here in the repository. 

## Docker CMDs
git clone https://github.com/johnmaloney/WebGIS.git

 docker build ./WebGIS/GeospatialAPI/ -t geo

docker run -d -p 8080:5000 geo

docker inspect geo

## Docker clean up CMDs
docker stop $(docker ps -a -q)

docker rmi -f $(docker images)

docker rm $(docker ps -a -q)

