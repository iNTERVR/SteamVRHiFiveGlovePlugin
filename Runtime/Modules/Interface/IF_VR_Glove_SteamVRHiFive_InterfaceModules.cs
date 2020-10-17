using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using InterVR.IF.VR.Glove.Modules;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Modules
{
    public class IF_VR_Glove_SteamVRHiFive_InterfaceModules : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            if (container.HasBinding<IF_VR_Glove_IInterface>())
            {
                container.Unbind<IF_VR_Glove_IInterface>();
            }
            container.Bind<IF_VR_Glove_IInterface, IF_VR_Glove_SteamVRHiFive_Interface>();
        }
    }
}