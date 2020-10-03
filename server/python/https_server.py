from http.server import HTTPServer, BaseHTTPRequestHandler;
from io import BytesIO;
import base64, ssl;
import sys, select;

import argparse; 


class SimpleHTTPRequestHandler(BaseHTTPRequestHandler):

    def do_GET(self):
        self.send_response(200);
        self.end_headers();
        print ('IN:>', end='');

        i, o, e = select.select( [sys.stdin], [], [], 10 );

        if (i):
              name = sys.stdin.readline().strip();
        else:
              name = '\x00'

        self.wfile.write(base64.b64encode(name.encode()));

    def do_POST(self):
        content_length = int(self.headers['Content-Length'])
        body = self.rfile.read(content_length);
        print(base64.b64decode(body).decode());
        self.send_response(200);
        self.end_headers();
        response = BytesIO();
        #response.write(b'Post ');
        #response.write(b'RECVD\n');
        #response.write(body);
        self.wfile.write(response.getvalue());


def main():
    
    parser = argparse.ArgumentParser();
    parser.add_argument("--key");
    parser.add_argument("--cert");
    parser.add_argument("--host");
    parser.add_argument("--port");

    args = parser.parse_args();

    ssl_key = args.key;
    ssl_cert = args.cert;
    host = args.host;
    port = int(args.port);

    httpd = HTTPServer((host, port), SimpleHTTPRequestHandler);
    httpd.socket = ssl.wrap_socket (httpd.socket, keyfile=ssl_key, certfile=ssl_cert, server_side=True);
    httpd.serve_forever();

if __name__ == "__main__":
    main();
