using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DonationEvents.Startup))]
namespace DonationEvents
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
