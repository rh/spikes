using System.Web;

namespace Example
{
    public class HttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write("It works!");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}