namespace Example

import System.Web

class HttpHandler(IHttpHandler):

  def ProcessRequest(context as HttpContext):
    context.Response.Write("It works!")
  
  IsReusable as bool:
    get:
      return true
