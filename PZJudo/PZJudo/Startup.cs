using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PZJudo.Startup))]
namespace PZJudo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
