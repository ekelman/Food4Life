using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Food4Life.Startup))]
namespace Food4Life
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
