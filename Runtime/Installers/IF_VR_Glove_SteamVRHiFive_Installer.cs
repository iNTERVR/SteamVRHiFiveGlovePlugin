using System;
using UnityEngine;
using Zenject;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Installer
{
    [CreateAssetMenu(fileName = "IF_VR_Glove_SteamVRHiFive", menuName = "InterVR/IF/Plugin/VR/Glove/SteamVRHiFive/Settings")]
    public class IF_VR_Glove_SteamVRHiFive_Installer : ScriptableObjectInstaller<IF_VR_Glove_SteamVRHiFive_Installer>
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
            public string Name = "IF Steam VR HiFive Glove Plugin Installer";
            public GameObject Hi5RightHandPrefab;
            public GameObject Hi5LeftHandPrefab;
        }
    }
}