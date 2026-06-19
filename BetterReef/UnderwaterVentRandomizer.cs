using System.Collections.Generic;
using CatUtilLib;
using KSerialization;
using UnityEngine;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class UnderwaterVentRandomizer: KMonoBehaviour, IGameObjectEffectDescriptor
    {
        [Serialize] private float bubbleMultiplier = -1f;
        [Serialize] private float buildupMultiplier = -1f;
        [Serialize] private float solidMultiplier = -1f;

        private bool isRolled;
        private float minBubbleMultiplier = 0.12f;
        private float maxBubbleMultiplier = 12f;
        private float minBuildupMultiplier = 0.5f;
        private float maxBuildupMultiplier = 2f;
        private float minSolidMultiplier = 0.25f;
        private float maxSolidMultiplier = 4f;
        
        private SchedulerHandle retryHandler;
        
        public float GetEmittingRate() => bubbleMultiplier * 0.083333336f;
        public float GetBuilduDuration() => buildupMultiplier * 1200f;
        public float GetSolidMass() => solidMultiplier * 1000f;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            ApplyMultiplier();
        }

        protected override void OnCleanUp()
        {
            retryHandler.ClearScheduler();
            base.OnCleanUp();
        }

        private void InitMultiplier()
        {
            bubbleMultiplier = CatUtils.Roll(gameObject, minBubbleMultiplier, maxBubbleMultiplier);
            buildupMultiplier = 2 * CatUtils.Roll(gameObject, minBuildupMultiplier, maxBuildupMultiplier) / bubbleMultiplier;
            solidMultiplier = CatUtils.Roll(gameObject, minSolidMultiplier, maxSolidMultiplier) / buildupMultiplier;
        }

        private void ApplyMultiplier()
        {
            isRolled = (bubbleMultiplier>0 && buildupMultiplier>0 && solidMultiplier>0);
            if (!isRolled) InitMultiplier();
            
            UnderwaterVent.Instance smi = GetComponent<StateMachineController>()?.GetSMI<UnderwaterVent.Instance>();
            if (smi == null)
            {
                SchedualedRetry();
                return;
            }
            
            retryHandler.ClearScheduler();
            
            UnderwaterVent.Def def = smi.def;
            UnderwaterVent.Def target = new UnderwaterVent.Def
            {
                data = new UnderwaterVent.Data
                {
                    BubbleMassRate = def.data.BubbleMassRate * bubbleMultiplier,
                    BuildUpDuration = def.data.BuildUpDuration * buildupMultiplier,
                    SolidMass = def.data.SolidMass * solidMultiplier,
                    BubbleElement = def.data.BubbleElement,
                    BubbleSpawnOffset = def.data.BubbleSpawnOffset,
                    BubbleTemp = def.data.BubbleTemp,
                    SolidElement = def.data.SolidElement,
                    SolidSpawnOffset = def.data.SolidSpawnOffset,
                    SolidTemp = def.data.SolidTemp
                }
            };
            smi.def = target;
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            return new List<Descriptor>
            {
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTEMIT,
                        GameUtil.GetFormattedMass(GetEmittingRate())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTEMIT,
                        GameUtil.GetFormattedMass(GetEmittingRate()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTDURATION,
                        GameUtil.GetFormattedCycles(GetBuilduDuration())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTDURATION,
                        GameUtil.GetFormattedCycles(GetBuilduDuration()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTSOLID,
                        GameUtil.GetFormattedMass(GetSolidMass())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTSOLID,
                        GameUtil.GetFormattedMass(GetSolidMass())))
            };
        }

        private void SchedualedRetry()
        {
            if (isRolled || retryHandler.IsValid || GameScheduler.Instance == null)
            {
                return;
            }

            retryHandler = GameScheduler.Instance.ScheduleNextFrame(
                nameof(ApplyMultiplier),
                _ =>
                {
                    retryHandler = default;
                    if (this != null) ApplyMultiplier();
                }
            );
        }
    }
}