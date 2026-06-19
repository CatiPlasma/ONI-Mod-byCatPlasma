using System;
using CatUtilLib;
using KSerialization;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReefGeyserRandomizer: KMonoBehaviour
    {
        [Serialize] private float inhaleMultiplier = -1f;
        [Serialize] private float exhaleMultiplier = -1f;
        [Serialize] private bool isRolled = false;
        
        public float GetInhaleMultiplier() => inhaleMultiplier;
        public float GetExhaleMultiplier() => exhaleMultiplier;
        
        private const float minExhaleMultiplier = 0.25f;
        private const float maxExhaleMultiplier = 4f;
        private const float minInhaleMultiplier = 0.25f;
        private const float maxInhaleMultiplier = 4f;
        
        private SchedulerHandle retryHandle;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (!isRolled) InitMultiplier();
            ApplyMultiplier();
        }

        protected override void OnCleanUp()
        {
            retryHandle.ClearScheduler();
            base.OnCleanUp();
        }
        
        private void InitMultiplier()
        {
            inhaleMultiplier = CatUtils.Roll(gameObject, minInhaleMultiplier, maxInhaleMultiplier);
            exhaleMultiplier = CatUtils.Roll(gameObject, minExhaleMultiplier, maxExhaleMultiplier);
            
            isRolled = true;
        }

        private void ApplyMultiplier()
        {
            if (!TryGetTarget(
                    out Storage storage,
                    out ElementConsumer elementConsumer,
                    out BreathingGeyser.Instance smi))
            {
                SchedualedRetry();
                return;
            }
            
            BreathingGeyser.Def original = smi.def;
            BreathingGeyser.Def target = new BreathingGeyser.Def
            {
                inhaleRate = original.inhaleRate,
                exhaleRate = original.exhaleRate,
                diseaseIdx = original.diseaseIdx,
                germsPerKg = original.germsPerKg
            };
            
            target.inhaleRate = original.inhaleRate * inhaleMultiplier;
            target.exhaleRate = original.exhaleRate * exhaleMultiplier;
            
            smi.def = target;
            
            storage.capacityKg *= Math.Max(inhaleMultiplier, exhaleMultiplier);
            elementConsumer.capacityKG = storage.capacityKg;
            elementConsumer.consumptionRate = target.inhaleRate;
        }

        private bool TryGetTarget(
            out Storage storage,
            out ElementConsumer elementConsumer,
            out BreathingGeyser.Instance smi)
        {
            storage = GetComponent<Storage>();
            elementConsumer = GetComponent<ElementConsumer>();
            smi = GetComponent<StateMachineController>()?.GetSMI<BreathingGeyser.Instance>();
            return storage != null && elementConsumer != null && smi != null;
        }
        
        private void SchedualedRetry()
        {
            if (isRolled || retryHandle.IsValid || GameScheduler.Instance == null)
            {
                return;
            }

            retryHandle = GameScheduler.Instance.ScheduleNextFrame(
                nameof(ApplyMultiplier),
                _ =>
                {
                    retryHandle = default;
                    if (this != null) ApplyMultiplier();
                }
            );
        }
    }
}