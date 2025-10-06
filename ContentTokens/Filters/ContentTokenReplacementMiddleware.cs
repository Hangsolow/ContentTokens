using ContentTokens.Services;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Globalization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace ContentTokens.Filters
{
    /// <summary>
    /// Middleware that replaces content tokens in HTML responses.
    /// </summary>
    public class ContentTokenReplacementMiddleware
    {
        private readonly RequestDelegate _next;

        public ContentTokenReplacementMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IContentTokenService tokenService)
        {
            // Only process HTML responses
            var originalBodyStream = context.Response.Body;

            try
            {
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    // Check if we should process this response
                    if (context.Response.ContentType?.Contains("text/html") == true 
                        && context.Response.StatusCode == 200)
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseText = await new StreamReader(responseBody).ReadToEndAsync();

                        // Get the current language
                        var languageCode = ContentLanguage.PreferredCulture?.Name;

                        // Replace tokens
                        var processedText = tokenService.ReplaceTokens(responseText, languageCode);

                        // Write the processed response
                        var processedBytes = Encoding.UTF8.GetBytes(processedText);
                        context.Response.Body = originalBodyStream;
                        context.Response.ContentLength = processedBytes.Length;
                        await context.Response.Body.WriteAsync(processedBytes, 0, processedBytes.Length);
                    }
                    else
                    {
                        // Just copy the original response
                        responseBody.Seek(0, SeekOrigin.Begin);
                        context.Response.Body = originalBodyStream;
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    /// <summary>
    /// Module to register the token replacement middleware.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(ContentTokensInitialization))]
    public class ContentTokenMiddlewareInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // Middleware registration is handled in Startup/Program.cs by the consuming application
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
