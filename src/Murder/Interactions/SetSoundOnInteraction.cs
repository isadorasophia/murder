using Bang;
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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace Murder.Interactions
{
    [CustomName("\uf2a2 Set Sound On Enter")]
    [Requires(typeof(SoundParameterComponent))]
    public readonly struct SetSoundOnInteraction : Interaction
    {
        [Tooltip("Blackboard variables")]
        public readonly ImmutableArray<SoundRuleAction> Triggers = ImmutableArray<SoundRuleAction>.Empty;

        [Tooltip("Global parameters")]
        public readonly ImmutableArray<ParameterRuleAction> Parameters = ImmutableArray<ParameterRuleAction>.Empty;

        public SetSoundOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            foreach (SoundRuleAction action in Triggers)
            {
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
                            .SetValue(action.Fact.Blackboard, action.Fact.Name, action.Value);
                        break;
                }
            }

            foreach (ParameterRuleAction p in Parameters)
            {
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
