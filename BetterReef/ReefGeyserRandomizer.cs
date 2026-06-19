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

        private bool isRolled;
        
        public float GetInhaleMultiplier() => inhaleMultiplier;
        public float GetExhaleMultiplier() => exhaleMultiplier;
        
        private const float minMultiplier = 0.25f;
        private const float maxMultiplier = 4f;
        
        private SchedulerHandle retryHandle;

        protected override void OnSpawn()
        {
            ApplyMultiplier();
            base.OnSpawn();
        }

        protected override void OnCleanUp()
        {
            retryHandle.ClearScheduler();
            base.OnCleanUp();
        }
        
        private void InitMultiplier()
        {
            (inhaleMultiplier, exhaleMultiplier) = CatUtils.Roll(gameObject, minMultiplier, maxMultiplier, 2);
        }

        private void ApplyMultiplier()
        {
            isRolled = (inhaleMultiplier > 0 && exhaleMultiplier > 0);
            
            if (!isRolled) InitMultiplier();
            
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