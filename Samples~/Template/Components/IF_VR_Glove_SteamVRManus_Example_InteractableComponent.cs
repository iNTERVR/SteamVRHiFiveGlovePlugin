using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.UnityEditor.MonoBehaviours;
using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Plugin.Steam.InteractionSystem;
using System.Collections.Generic;
using UnityEngine;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Example.Components
{
    public class IF_VR_Glove_SteamVRManus_Example_Interactable : IComponent
    {
        public TextMesh GeneralText { get; set; }
        public TextMesh HoveringText { get; set; }
        public Vector3 oldPosition;
        public Quaternion oldRotation;
        public float AttachTime;
        public IF_VR_Steam_Hand.AttachmentFlags AttachmentFlags;
        public bool LastHovering;
    }

    public class IF_VR_Glove_SteamVRManus_Example_InteractableComponent : RegisterAsEntity
    {
        public TextMesh GeneralText;
        public TextMesh HoveringText;
        IF_VR_Steam_Hand.AttachmentFlags attachmentFlags = IF_VR_Steam_Hand.defaultAttachmentFlags &
            (~IF_VR_Steam_Hand.AttachmentFlags.SnapOnAttach) &
            (~IF_VR_Steam_Hand.AttachmentFlags.DetachOthers) &
            (~IF_VR_Steam_Hand.AttachmentFlags.VelocityMovement);

        public override void Convert(IEntity entity, IComponent component = null)
        {
            var c = component == null ? new IF_VR_Glove_SteamVRManus_Example_Interactable() : component as IF_VR_Glove_SteamVRManus_Example_Interactable;

            c.GeneralText = GeneralText;
            c.HoveringText = HoveringText;
            c.AttachmentFlags = attachmentFlags;

            entity.AddComponentSafe(c);

            Destroy(this);
        }
    }
}