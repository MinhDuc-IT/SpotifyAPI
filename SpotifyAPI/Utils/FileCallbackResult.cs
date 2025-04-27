using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace SpotifyAPI.Utils
{
    public class FileCallbackResult : FileResult
    {
        private readonly Func<Stream, Task> _callback;

        public FileCallbackResult(string contentType, Func<Stream, Task> callback) : base(contentType)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = ContentType;

            await _callback(response.Body); // Stream trực tiếp về client
        }
    }


}
