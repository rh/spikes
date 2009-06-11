# Compile with booc -t:exe server.boo #=> server.exe

import System
import System.Net
import System.Text
import System.Threading

class Server:

  listener as HttpListener

  def constructor():
    ThreadPool.SetMaxThreads(50, 1000)
    ThreadPool.SetMinThreads(50, 50)
    listener = HttpListener()
    listener.Prefixes.Add("http://localhost:8001/")

  def Start():
    listener.Start()
    while true:
      try:
        request = listener.GetContext()
        ThreadPool.QueueUserWorkItem(ProcessRequest, request)
      except e as HttpListenerException:
        break
      except e as InvalidOperationException:
        break

  def Stop():
    listener.Stop()

  def ProcessRequest(request):
    try:
      context = cast(HttpListenerContext, request)
      print context.Request.RawUrl
      context.Response.StatusCode = cast(int, HttpStatusCode.OK)
      bytes = Encoding.UTF8.GetBytes(context.Request.RawUrl)
      context.Response.ContentLength64 = bytes.Length
      using stream = context.Response.OutputStream:
        stream.Write(bytes, 0, bytes.Length)
    except e:
      print "ERROR: ${e.Message}" 

print "Starting server on http://localhost:8001/"
print "Press Ctrl-C to shutdown"
server = Server()
server.Start()