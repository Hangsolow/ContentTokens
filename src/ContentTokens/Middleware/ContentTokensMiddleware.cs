using ContentTokens.Services;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;

namespace ContentTokens.Middleware
{
    /// <summary>
    /// ASP.NET Core middleware that intercepts HTML responses and replaces
    /// <c>{{TokenName}}</c> placeholders with their configured values.
    /// Register with <c>app.UseContentTokens()</c> after <c>UseRouting()</c> and
    /// <c>UseAuthentication()</c>.
    /// </summary>
    public class ContentTokensMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IContentTokenService _tokenService;

        public ContentTokensMiddleware(RequestDelegate next, IContentTokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            try
            {
                await _next(context);
            }
            finally
            {
                context.Response.Body = originalBody;
            }

            buffer.Position = 0;

            if (IsHtmlResponse(context.Response))
            {
                var bodyText = await new StreamReader(buffer, Encoding.UTF8).ReadToEndAsync();
                var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var replaced = _tokenService.ReplaceTokens(bodyText, languageCode);
                var bytes = Encoding.UTF8.GetBytes(replaced);
                context.Response.ContentLength = bytes.Length;
                await originalBody.WriteAsync(bytes);
            }
            else
            {
                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody);
            }
        }

        private static bool IsHtmlResponse(HttpResponse response) =>
            response.ContentType?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true;
    }
}
