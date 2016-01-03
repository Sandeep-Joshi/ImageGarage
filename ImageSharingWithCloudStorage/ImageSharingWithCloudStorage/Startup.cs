using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ImageSharingWithCloudStorage.Startup))]
namespace ImageSharingWithCloudStorage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
