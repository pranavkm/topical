using System;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Topical.Models;
using Topical.Services;

namespace Topical.Infrastructure
{
    public class BasicAuthHandler : DelegatingHandler
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");
        private readonly IDependencyResolver _dependencyResolver;

        public BasicAuthHandler(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        private IUserService UserService
        {
            get { return (IUserService)_dependencyResolver.GetService(typeof(IUserService)); }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken cancellationToken)
        {
            var credentials = ParseAuthorizationHeader(request);
            User user = credentials != null ? UserService.Get(credentials.Item1, credentials.Item2) : null;

            if (user != null)
            {
                var identity = new GenericIdentity(credentials.Item1);
                var principal = new GenericPrincipal(identity, roles: null);

                Thread.CurrentPrincipal = principal;
                var requestContext = request.GetRequestContext();
                if (requestContext != null)
                {
                    requestContext.Principal = principal;
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        protected Tuple<string, string> ParseAuthorizationHeader(HttpRequestMessage request)
        {
            var auth = request.Headers.Authorization;
            if (auth == null || 
                !String.Equals(auth.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) || 
                String.IsNullOrEmpty(auth.Parameter))
            {
                return null;
            }

            string authHeader = _encoding.GetString(Convert.FromBase64String(auth.Parameter));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
            {
                return null;
            }

            return Tuple.Create(tokens[0], tokens[1]);
        }
    }
}