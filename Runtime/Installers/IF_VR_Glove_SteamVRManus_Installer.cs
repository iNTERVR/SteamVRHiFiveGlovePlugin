using System;
using UnityEngine;
using Zenject;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Installer
{
    [CreateAssetMenu(fileName = "IF_VR_Glove_SteamVRManus_Settings", menuName = "InterVR/IF/Plugin/VR/Glove/SteamVRManus/Settings")]
    public class IF_VR_Glove_SteamVRManus_Installer : ScriptableObjectInstaller<IF_VR_Glove_SteamVRManus_Installer>
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
            public string Name = "IF Steam VR Manus Glove Plugin Installer";
        }
    }
}