using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ImageSharingWithAuth.Startup))]
namespace ImageSharingWithAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
