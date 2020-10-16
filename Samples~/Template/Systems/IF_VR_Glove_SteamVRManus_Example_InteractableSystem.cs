using EcsRx.Groups;
using EcsRx.Events;
using System.Collections.Generic;
using System;
using EcsRx.Collections.Database;
using InterVR.IF.VR.Components;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Entities;
using InterVR.IF.VR.Events;
using EcsRx.Extensions;
using UniRx;
using InterVR.IF.VR.Glove.Plugin.SteamVRManus.Example.Components;
using EcsRx.Unity.Extensions;
using InterVR.IF.VR.Plugin.Steam.InteractionSystem;
using UnityEngine;
using UniRx.Triggers;
using InterVR.IF.VR.Glove.Modules;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Example.Systems
{
    public class IF_VR_Glove_SteamVRManus_Example_InteractableSystem : ISetupSystem, ITeardownSystem
    {
        public IGroup Group => new Group(typeof(IF_VR_Interactable), typeof(IF_VR_Glove_SteamVRManus_Example_Interactable));

        private Dictionary<IEntity, List<IDisposable>> subscriptionsPerEntity = new Dictionary<IEntity, List<IDisposable>>();
        private readonly IEventSystem eventSystem;
        private readonly IEntityDatabase entityDatabase;
        private readonly IF_VR_Glove_IInterface vrGloveInterface;

        public IF_VR_Glove_SteamVRManus_Example_InteractableSystem(IEventSystem eventSystem,
            IEntityDatabase entityDatabase,
            IF_VR_Glove_IInterface vrGloveInterface)
        {
            this.eventSystem = eventSystem;
            this.entityDatabase = entityDatabase;
            this.vrGloveInterface = vrGloveInterface;
        }

        public void Setup(IEntity entity)
        {
            Debug.Log("Example Interactable System Setup");

            List<IDisposable> subscriptions = new List<IDisposable>();
            subscriptionsPerEntity.Add(entity, subscriptions);

            var baseInteractable = entity.GetUnityComponent<IF_VR_Steam_Interactable>();
            var exampleInteractable = entity.GetComponent<IF_VR_Glove_SteamVRManus_Example_Interactable>();

            exampleInteractable.GeneralText.text = "No Hand Hovering";
            exampleInteractable.HoveringText.text = "Hovering: False";

            var view = entity.GetGameObject();
            view.OnDestroyAsObservable().Subscribe(x =>
            {
                entityDatabase.RemoveEntity(entity);
            }).AddTo(subscriptions);

            eventSystem.Receive<IF_VR_Event_OnHandHoverBegin>().Subscribe(evt =>
            {
                if (evt.TargetEntity.Id == entity.Id)
                {
                    var handView = evt.HandEntity.GetGameObject();
                    exampleInteractable.GeneralText.text = "Hovering hand: " + handView.name;
                }
            }).AddTo(subscriptions);

            eventSystem.Receive<IF_VR_Event_OnHandHoverEnd>().Subscribe(evt =>
            {
                if (evt.TargetEntity.Id == entity.Id)
                {
                    exampleInteractable.GeneralText.text = "No Hand Hovering";
                }
            }).AddTo(subscriptions);

            eventSystem.Receive<IF_VR_Event_HandHoverUpdate>().Subscribe(evt =>
            {
                if (evt.TargetEntity.Id == entity.Id)
                {
                    var hand = evt.HandEntity.GetUnityComponent<IF_VR_Steam_Hand>();
                    var startingGrabType = hand.GetGrabStarting();
                    bool isGrabEnding = hand.IsGrabEnding(view);

                    if (baseInteractable.attachedToHand == null && startingGrabType != IF_VR_Steam_GrabTypes.None)
                    {
                        // Save our position/rotation so that we can restore it when we detach
                        exampleInteractable.oldPosition = view.transform.position;
                        exampleInteractable.oldRotation = view.transform.rotation;

                        // Call this to continue receiving HandHoverUpdate messages,
                        // and prevent the hand from hovering over anything else
                        hand.HoverLock(baseInteractable);

                        // Attach this object to the hand
                        hand.AttachObject(view, startingGrabType, exampleInteractable.AttachmentFlags);
                    }
                    else if (isGrabEnding)
                    {
                        // Detach this object from the hand
                        hand.DetachObject(view);

                        // Call this to undo HoverLock
                        hand.HoverUnlock(baseInteractable);

                        // Restore position/rotation
                        view.transform.position = exampleInteractable.oldPosition;
                        view.transform.rotation = exampleInteractable.oldRotation;
                    }
                }
            }).AddTo(subscriptions);

            eventSystem.Receive<IF_VR_Event_OnAttachedToHand>().Subscribe(evt =>
            {
                if (evt.TargetEntity.Id == entity.Id)
                {
                    var hand = evt.HandEntity.GetUnityComponent<IF_VR_Steam_Hand>();
                    exampleInteractable.GeneralText.text = string.Format("Attached: {0}", hand.name);
                    exampleInteractable.AttachTime = Time.time;
                }
            }).AddTo(subscriptions);

            eventSystem.Receive<IF_VR_Event_OnDetachedToHand>().Subscribe(evt =>
            {
                if (evt.TargetEntity.Id == entity.Id)
                {
                    var hand = evt.HandEntity.GetUnityComponent<IF_VR_Steam_Hand>();
                    exampleInteractable.GeneralText.text = string.Format("Detached: {0}", hand.name);
                }
            }).AddTo(subscriptions);

            Observable.EveryUpdate()
                .Subscribe(x =>
                {
                    if (baseInteractable.isHovering != exampleInteractable.LastHovering)
                    {
                        exampleInteractable.HoveringText.text = string.Format("Hovering: {0}", baseInteractable.isHovering);
                        exampleInteractable.LastHovering = baseInteractable.isHovering;
                    }
                }).AddTo(subscriptions);
        }

        public void Teardown(IEntity entity)
        {
            if (subscriptionsPerEntity.TryGetValue(entity, out List<IDisposable> subscriptions))
            {
                subscriptions.DisposeAll();
                subscriptions.Clear();
                subscriptionsPerEntity.Remove(entity);
            }
        }
    }
}