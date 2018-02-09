using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LostStuffs.Startup))]
namespace LostStuffs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
