using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Glove.Modules;
using UniRx;
using UnityEngine;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Modules
{
    public class IF_VR_Glove_SteamVRManus_Interface : IF_VR_Glove_IInterface
    {
        public int PlayerNumber
        {
            get; private set;
        }

        public FloatReactiveProperty HandYawOffsetLeft { get; private set; }
        public FloatReactiveProperty HandYawOffsetRight { get; private set; }

        public IF_VR_Glove_SteamVRManus_Interface()
        {
            HandYawOffsetLeft = new FloatReactiveProperty();
            HandYawOffsetRight = new FloatReactiveProperty();
        }

        Transform rootTransform;

        public Transform GetRootTransform()
        {
            return rootTransform;
        }

        public void SetRootTransform(Transform root)
        {
            rootTransform = root;
        }

        public bool GetGrabState(IF_VR_HandType handType)
        {
            return false;
        }

        public bool GetGrabStateDown(IF_VR_HandType handType)
        {
            return false;
        }

        public bool GetGrabStateUp(IF_VR_HandType handType)
        {
            return false;
        }

        public void Dispose()
        {
            HandYawOffsetLeft.Dispose();
            HandYawOffsetRight.Dispose();
        }
    }
}