using System.Web.Hosting;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Topical.Repository;
using Topical.Services;

namespace Topical
{
    public class DIConfig
    {
        public static void Configure(HttpConfiguration httpConfiguration)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new LuceneProvider(HostingEnvironment.MapPath("~/")))
                   .SingleInstance();

            builder.RegisterType<TopicService>()
                   .As<ITopicService>()
                   .InstancePerApiRequest();

            builder.RegisterType<CommentService>()
                   .As<ICommentService>()
                   .InstancePerApiRequest();

            builder.RegisterApiControllers(typeof(DIConfig).Assembly);

            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());
        }
    }
}