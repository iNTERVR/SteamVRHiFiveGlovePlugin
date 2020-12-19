using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Glove.Modules;
using UniRx;
using UnityEngine;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Modules
{
    public class IF_VR_Glove_SteamVRHiFive_Interface : IF_VR_Glove_IInterface
    {
        public int PlayerNumber
        {
            get; private set;
        }

        public FloatReactiveProperty HandYawOffsetLeft { get; private set; }
        public FloatReactiveProperty HandYawOffsetRight { get; private set; }

        public IF_VR_Glove_SteamVRHiFive_Interface(IHI5Interface hI5Interface)
        {
            HandYawOffsetLeft = new FloatReactiveProperty();
            HandYawOffsetRight = new FloatReactiveProperty();
            this.hI5Interface = hI5Interface;
        }

        Transform rootTransform;
        private readonly IHI5Interface hI5Interface;

        public Transform GetRootTransform()
        {
            return rootTransform;
        }

        public void SetRootTransform(Transform root)
        {
            rootTransform = root;
        }

        // 현재 grab 중인지 여부
        public bool GetGrabState(IF_VR_HandType handType)
        {
            return hI5Interface.IsGrab(handType);
        }

        // 현재 grab 시작 여부
        public bool GetGrabStateDown(IF_VR_HandType handType)
        {
            return hI5Interface.IsGrab(handType);
        }

        // 현재 grab 끝내기 여부
        public bool GetGrabStateUp(IF_VR_HandType handType)
        {
            return hI5Interface.IsGrab(handType);
        }

        public void Dispose()
        {
            HandYawOffsetLeft.Dispose();
            HandYawOffsetRight.Dispose();
        }
    }
}