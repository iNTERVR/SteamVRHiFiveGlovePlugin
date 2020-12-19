using System;
using UnityEngine;
using Zenject;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Example
{
    [CreateAssetMenu(fileName = "IF_VR_Glove_SteamVRHiFive_Example_Settings", menuName = "InterVR/IF/Plugin/VR/Glove/SteamVRHiFive/Example/Settings")]
    public class IF_VR_Glove_SteamVRHiFive_Example_Installer : ScriptableObjectInstaller<IF_VR_Glove_SteamVRHiFive_Example_Installer>
    {
#pragma warning disable 0649
        [SerializeField]
        Settings settings;
#pragma warning restore 0649

        public override void InstallBindings()
        {
            Container.BindInstance(settings).IfNotBound();
        }

        [Serializable]
        public class Settings
        {
            public string Name = "IF Steam VR HiFive Glove Plugin Example Installer";
            public float HandYaw = 180;
        }
    }
}