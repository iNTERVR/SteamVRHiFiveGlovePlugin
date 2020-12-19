using UnityEngine;
using InterVR.IF.VR.Plugin.Steam.InteractionSystem;
using InterVR.IF.VR.Components;
using InterVR.IF.VR.Modules;
using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Plugin.Steam.Extensions;
using InterVR.IF.VR.Glove.Modules;
using EcsRx.Entities;
using EcsRx.Unity.Extensions;
using HI5;
using System.Collections.Generic;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Modules
{
    public class HI5Interface : IHI5Interface
    {
        public HI5_GloveStatus Status { get; private set; }

        Hi5Bones LeftHandBones;
        Hi5Bones RightHandBones;

        public HI5Interface()
        {
            Status = HI5_Manager.GetGloveStatus();
        }

        public bool IsGloveAvailable(Hand handType)
        {
            return Status.IsGloveAvailable(handType);
        }

        public void SetBones(IF_VR_HandType type, Hi5Bones bones)
        {
            if (type == IF_VR_HandType.Left)
            {
                LeftHandBones = bones;
            }
            else if (type == IF_VR_HandType.Right)
            {
                RightHandBones = bones;
            }
        }

        public bool IsGrab(IF_VR_HandType type)
        {
            if (type == IF_VR_HandType.Left)
            {
                if (LeftHandBones == null)
                    return false;

                return LeftHandBones.IsGrab();
            }
            else if (type == IF_VR_HandType.Right)
            {
                if (RightHandBones == null)
                    return false;

                return RightHandBones.IsGrab();
            }

            return false;
        }
    }
}