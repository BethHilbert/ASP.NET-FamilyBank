using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GenieBank.Startup))]
namespace GenieBank
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
