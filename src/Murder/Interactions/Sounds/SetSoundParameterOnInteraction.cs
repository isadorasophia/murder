﻿using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Set Parameter On Interaction")]
    [Requires(typeof(SoundParameterComponent))]
    public readonly struct SetSoundParameterOnInteraction : IInteraction
    {
        [Tooltip("Sets a SoundBlackboard.cs field")]
        public readonly ImmutableArray<SoundRuleAction> BlackboardTriggers = ImmutableArray<SoundRuleAction>.Empty;

        [Tooltip("Set a parameter directly in the middleware, bypassing any blackboard fields")]
        public readonly ImmutableArray<ParameterRuleAction> MiddlewareParameterTriggers = ImmutableArray<ParameterRuleAction>.Empty;

        public SetSoundParameterOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            foreach (SoundRuleAction action in BlackboardTriggers)
            {
                if (string.IsNullOrEmpty(action.Fact.Name))
                {
                    // Skip empty values.
                    continue;
                }

                if (action.Value is null)
                {
                    GameLogger.Error("Sound On Enter had incorrect value set on action.");
                    continue;
                }

                switch (action.Kind)
                {
                    case BlackboardActionKind.Add:
                    case BlackboardActionKind.Minus:
                        MurderSaveServices.CreateOrGetSave().BlackboardTracker.SetInt(
                            action.Fact.Blackboard, action.Fact.Name, action.Kind, Convert.ToInt32(action.Value));

                        break;

                    default:
                        MurderSaveServices.CreateOrGetSave().BlackboardTracker
                            .SetValue(action.Fact.Blackboard, action.Fact.Name, Convert.ToInt32(action.Value));
                        break;
                }
            }

            foreach (ParameterRuleAction p in MiddlewareParameterTriggers)
            {
                if (p.Parameter.IsGuidEmpty)
                {
                    // Skip empty values.
                    continue;
                }

                float value = p.Value;
                switch (p.Kind)
                {
                    case BlackboardActionKind.Add:
                        value += SoundServices.GetGlobalParameter(p.Parameter);
                        break;

                    case BlackboardActionKind.Minus:
                        value = SoundServices.GetGlobalParameter(p.Parameter) - value;
                        break;

                    default:
                        break;
                }

                SoundServices.SetGlobalParameter(p.Parameter, value);
            }
        }
    }
}