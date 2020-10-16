using UnityEngine;
using InterVR.IF.VR.Plugin.Steam.InteractionSystem;
using InterVR.IF.VR.Components;
using InterVR.IF.VR.Modules;
using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Plugin.Steam.Extensions;
using InterVR.IF.VR.Glove.Modules;
using EcsRx.Entities;
using EcsRx.Unity.Extensions;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Modules
{
    public class IF_VR_Glove_SteamVRManus_GrabStatus : IF_VR_IGrabStatus
    {
        private readonly IF_VR_Glove_IInterface vrGloveInterface;

        public IF_VR_Glove_SteamVRManus_GrabStatus(IF_VR_Glove_IInterface vrGloveInterface)
        {
            this.vrGloveInterface = vrGloveInterface;
        }

        public IF_VR_GrabType GetBestGrabbingType(IEntity handEntity, IF_VR_GrabType preferred, bool forcePreference = false)
        {
            var steamVRHand = handEntity.GetUnityComponent<IF_VR_Steam_Hand>();
            if (steamVRHand.noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return preferred;
                else
                    return IF_VR_Steam_GrabTypes.None.ConvertTo();
            }

            //if (preferred == IF_VR_Steam_GrabTypes.Pinch.ConvertTo())
            //{
            //    if (vrGloveInterface.GetGrabState(hand.Type))
            //        return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
            //    else if (forcePreference)
            //        return IF_VR_Steam_GrabTypes.None.ConvertTo();
            //}
            if (preferred == IF_VR_Steam_GrabTypes.Grip.ConvertTo())
            {
                if (vrGloveInterface.GetGrabState(steamVRHand.handType.ConvertTo()))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
                else if (forcePreference)
                    return IF_VR_Steam_GrabTypes.None.ConvertTo();
            }

            //if (vrGloveInterface.GetGrabState(hand.Type))
            //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
            if (vrGloveInterface.GetGrabState(steamVRHand.handType.ConvertTo()))
                return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            return IF_VR_Steam_GrabTypes.None.ConvertTo();
        }

        public IF_VR_GrabType GetGrabEnding(IEntity handEntity, IF_VR_GrabType explicitType = IF_VR_GrabType.None)
        {
            var steamVRHand = handEntity.GetUnityComponent<IF_VR_Steam_Hand>();
            if (explicitType != IF_VR_Steam_GrabTypes.None.ConvertTo())
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonUp(0))
                        return explicitType;
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }

                //if (explicitType == IF_VR_Steam_GrabTypes.Pinch.ConvertTo() && vrGloveInterface.GetGrabStateUp(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (explicitType == IF_VR_Steam_GrabTypes.Grip.ConvertTo() && vrGloveInterface.GetGrabStateUp(steamVRHand.handType.ConvertTo()))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            else
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonUp(0))
                        return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }

                //if (vrGloveInterface.GetGrabStateUp(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (vrGloveInterface.GetGrabStateUp(steamVRHand.handType.ConvertTo()))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            return IF_VR_Steam_GrabTypes.None.ConvertTo();
        }

        public IF_VR_GrabType GetGrabStarting(IEntity handEntity, IF_VR_GrabType explicitType = IF_VR_GrabType.None)
        {
            var steamVRHand = handEntity.GetUnityComponent<IF_VR_Steam_Hand>();
            if (explicitType != IF_VR_Steam_GrabTypes.None.ConvertTo())
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonDown(0))
                        return explicitType;
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }

                // pinch not supoorted
                //if (explicitType == IF_VR_Steam_GrabTypes.Pinch.ConvertTo() && vrGloveInterface.GetGrabStateDown(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (explicitType == IF_VR_Steam_GrabTypes.Grip.ConvertTo() && vrGloveInterface.GetGrabStateDown(steamVRHand.handType.ConvertTo()))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            else
            {
                if (steamVRHand.noSteamVRFallbackCamera)
                {
                    if (Input.GetMouseButtonDown(0))
                        return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
                    else
                        return IF_VR_Steam_GrabTypes.None.ConvertTo();
                }


                //if (vrGloveInterface.GetGrabStateDown(hand.Type))
                //    return IF_VR_Steam_GrabTypes.Pinch.ConvertTo();
                if (vrGloveInterface.GetGrabStateDown(steamVRHand.handType.ConvertTo()))
                    return IF_VR_Steam_GrabTypes.Grip.ConvertTo();
            }
            return IF_VR_Steam_GrabTypes.None.ConvertTo();
        }

        public bool IsGrabbingWithOppositeType(IEntity handEntity, IF_VR_GrabType type)
        {
            var steamVRHand = handEntity.GetUnityComponent<IF_VR_Steam_Hand>();
            if (steamVRHand.noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return true;
                else
                    return false;
            }

            //if (type == IF_VR_Steam_GrabTypes.Pinch.ConvertTo())
            //{
            //    return vrGloveInterface.GetGrabState(hand.Type);
            //}
            //else
            if (type == IF_VR_Steam_GrabTypes.Grip.ConvertTo())
            {
                return vrGloveInterface.GetGrabState(steamVRHand.handType.ConvertTo());
            }
            return false;
        }

        public bool IsGrabbingWithType(IEntity handEntity, IF_VR_GrabType type)
        {
            var steamVRHand = handEntity.GetUnityComponent<IF_VR_Steam_Hand>();
            if (steamVRHand.noSteamVRFallbackCamera)
            {
                if (Input.GetMouseButton(0))
                    return true;
                else
                    return false;
            }

            if (type == IF_VR_Steam_GrabTypes.Grip.ConvertTo())
            {
                return vrGloveInterface.GetGrabState(steamVRHand.handType.ConvertTo());
            }
            //else
            //if (type == IF_VR_Steam_GrabTypes.Pinch.ConvertTo())
            //{
            //    return vrGloveInterface.GetGrabState(hand.Type);
            //}
            return false;
        }
    }
}