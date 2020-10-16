using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Installer
{
    public class IF_VR_Glove_SteamVRManus_MonoInstaller : MonoInstaller<IF_VR_Glove_SteamVRManus_MonoInstaller>
    {
        public List<ScriptableObjectInstaller> settings;

        public override void InstallBindings()
        {
            var settingsInstaller = settings.Cast<IInstaller>();
            foreach (var installer in settingsInstaller)
            {
                Container.Inject(installer);
                installer.InstallBindings();
            }
        }
    }
}