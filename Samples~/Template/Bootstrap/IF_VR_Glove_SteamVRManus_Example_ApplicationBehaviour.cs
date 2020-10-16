using EcsRx.Infrastructure.Extensions;
using InterVR.IF.Installer;
using InterVR.IF.Modules;
using InterVR.IF.VR.Glove.Installer;
using InterVR.IF.VR.Glove.Modules;
using InterVR.IF.VR.Glove.Plugin.SteamVRManus.Modules;
using InterVR.IF.VR.Modules;
using InterVR.IF.VR.Plugin.Steam.Modules;
using UnityEngine;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Example
{
    [DefaultExecutionOrder(-20000)]
    public class IF_VR_Glove_SteamVRManus_Example_ApplicationBehaviour : IF_ApplicationBehaviour
    {
        protected override void BindSystems()
        {
            base.BindSystems();

            Container.BindApplicableSystems(
                "InterVR.IF.Systems",
                "InterVR.IF.ViewResolvers");
            Container.BindApplicableSystems(
                "InterVR.IF.VR.Systems",
                "InterVR.IF.VR.ViewResolvers");
            Container.BindApplicableSystems(
                "InterVR.IF.VR.Plugin.Steam.Systems",
                "InterVR.IF.VR.Plugin.Steam.ViewResolvers");
            Container.BindApplicableSystems(
                "InterVR.IF.VR.Glove.Systems",
                "InterVR.IF.VR.Glove.ViewResolvers");
            Container.BindApplicableSystems(
                "InterVR.IF.VR.Glove.Plugin.SteamVRManus.Systems",
                "InterVR.IF.VR.Glove.Plugin.SteamVRManus.ViewResolvers");
        }

        protected override void LoadModules()
        {
            base.LoadModules();

            Container.LoadModule<IF_ToolModules>();
            Container.LoadModule<IF_VR_InterfaceModules>();
            Container.LoadModule<IF_VR_StatusModules>();
            Container.LoadModule<IF_VR_Steam_InterfaceModules>();
            Container.LoadModule<IF_VR_Steam_StatusModules>();
            Container.LoadModule<IF_VR_Steam_ComponentBuilderModules>();
            Container.LoadModule<IF_VR_Steam_MessageToEventModules>();
            Container.LoadModule<IF_VR_Glove_InterfaceModules>();
            Container.LoadModule<IF_VR_Glove_SteamVRManus_InterfaceModules>();
            Container.LoadModule<IF_VR_Glove_SteamVRManus_StatusModules>();
        }

        protected override void LoadPlugins()
        {
            base.LoadPlugins();
        }

        protected override void ApplicationStarted()
        {
            base.ApplicationStarted();

            var settings = Container.Resolve<IF_VR_Glove_SteamVRManus_Example_Installer.Settings>();
            var interSettings = Container.Resolve<IF_Installer.Settings>();
            Debug.Log($"settings.Name is {settings.Name} in {interSettings.Name}");
        }

        private void OnDestroy()
        {
            StopAndUnbindAllSystems();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == false)
            {
            }
        }

        private void OnApplicationFocus(bool focus)
        {

        }
    }
}