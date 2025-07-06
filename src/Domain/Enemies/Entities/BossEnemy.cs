using System;
using Game.Domain.Enemies.ValueObjects;

namespace Game.Domain.Enemies.Entities;

public class BossEnemy : Enemy
{
    public float ScaleMultiplier { get; }
    public bool IsImmuneToDamage { get; private set; }
    public int SpecialAbilityCooldown { get; private set; }
    public bool CanUseSpecialAbility => SpecialAbilityCooldown <= 0;

    public BossEnemy(EnemyStats stats, float x, float y, float scaleMultiplier = 2.0f) 
        : base(stats, x, y)
    {
        ScaleMultiplier = scaleMultiplier;
        IsImmuneToDamage = false;
        SpecialAbilityCooldown = 0;
    }

    public override void TakeDamage(int damage, float currentTime)
    {
        if (IsImmuneToDamage)
        {
            return;
        }

        base.TakeDamage(damage, currentTime);
    }

    public void ActivateDamageImmunity(int durationSeconds)
    {
        IsImmuneToDamage = true;
        SpecialAbilityCooldown = durationSeconds;
    }

    public void UpdateSpecialAbility(float deltaTime)
    {
        if (SpecialAbilityCooldown > 0)
        {
            SpecialAbilityCooldown = Math.Max(0, SpecialAbilityCooldown - (int)deltaTime);
            
            if (SpecialAbilityCooldown <= 0)
            {
                IsImmuneToDamage = false;
            }
        }
    }

    public void UseSpecialAbility()
    {
        if (!CanUseSpecialAbility)
        {
            return;
        }

        ActivateDamageImmunity(3);
    }

    public override float HealthPercentage => Stats.MaxHealth > 0 ? (float)CurrentHealth / Stats.MaxHealth : 0f;

    public bool IsInFinalPhase => HealthPercentage <= 0.25f;

    public override string ToString()
    {
        return $"BossEnemy(Id:{Id:N}, HP:{CurrentHealth}/{Stats.MaxHealth}, Scale:{ScaleMultiplier:F1}x, Pos:({X:F1},{Y:F1}), Alive:{IsAlive}, Immune:{IsImmuneToDamage})";
    }
}
