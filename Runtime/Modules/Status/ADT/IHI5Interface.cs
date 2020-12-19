using InterVR.IF.VR.Defines;
using EcsRx.Entities;
using UniRx;
using UnityEngine;
using InterVR.IF.VR.Components;
using HI5;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Modules
{
    public interface IHI5Interface
    {
        HI5_GloveStatus Status { get; }
        bool IsGloveAvailable(Hand handType);
        void SetBones(IF_VR_HandType type, Hi5Bones bones);
        bool IsGrab(IF_VR_HandType type);
    }
}