using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Message_app_backend.Shared
{
    public class MessageResponse<T>
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}
