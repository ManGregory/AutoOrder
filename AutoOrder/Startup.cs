using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AutoOrder.Startup))]
namespace AutoOrder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
