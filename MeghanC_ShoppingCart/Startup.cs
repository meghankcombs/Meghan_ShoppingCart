using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MeghanC_ShoppingCart.Startup))]
namespace MeghanC_ShoppingCart
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
