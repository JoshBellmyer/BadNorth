using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHelper : MonoBehaviour
{
    private static Dictionary<DamageType, float> knockback;
    private static Dictionary<string, int> convertDamage;

    private Unit unit;

    public void Awake () {
        unit = GetComponent<Unit>();
    }

    public void TakeDamage (DamageType damageType, Vector3 direction) {
        if (unit == null) {
            return;
        }
        if (convertDamage == null) {
            Initialize();
        }

        int damage = 1;
        string str = $"{damageType}{unit.Type}";

        if (convertDamage.ContainsKey(str)) {
            damage = convertDamage[str];
        }

        unit.Health -= damage;

        if (knockback.ContainsKey(damageType)) {
            Vector3 knockDir = Vector3.Normalize(direction);
            unit.SetKnockback(knockDir * knockback[damageType]);
        }

        SetRed(0.2f);
        ParticleSpawner.SpawnParticle(transform.position + new Vector3(0, 0.5f, 0), 0, 0.3f);

        unit.lastAttacked = 0.2f;
    }

    private void SetRed (float redTime) {
        if (unit == null) {
            return;
        }

        unit.SetColor(Color.red);
        unit.SetTeamColor(0);

        StartCoroutine( UnsetRed(redTime) );
    }

    private IEnumerator UnsetRed (float redTime) {

        yield return new WaitForSeconds(redTime);

        unit.SetColor(Color.white);
        unit.ResetTeamColor();
    }

    private static void Initialize () {
        knockback = new Dictionary<DamageType, float>();
        knockback.Add(DamageType.Blunt, 0.8f);
        knockback.Add(DamageType.Slashing, 0.2f);
        knockback.Add(DamageType.HeavySlashing, 0.2f);

        convertDamage = new Dictionary<string, int>();
        convertDamage.Add($"{DamageType.Piercing}{UnitType.Axe}", 2);
        convertDamage.Add($"{DamageType.Piercing}{UnitType.Sword}", 2);
        convertDamage.Add($"{DamageType.Piercing}{UnitType.Pike}", 2);
        convertDamage.Add($"{DamageType.Stabbing}{UnitType.Axe}", 2);
        convertDamage.Add($"{DamageType.Stabbing}{UnitType.Shield}", 2);
        convertDamage.Add($"{DamageType.HeavySlashing}{UnitType.Shield}", 2);
        convertDamage.Add($"{DamageType.Slashing}{UnitType.Axe}", 2);
        convertDamage.Add($"{DamageType.Slashing}{UnitType.Pike}", 2);
    }
}

/*
 * Source
 * Destination
 * Damage
 * DamageType
 * Knockback
 * -> Direction
 */
public enum DamageType
{
    Unblockable = 0,
    Impact = 101,
    Fire = 102,
    Blunt = 201,
    Piercing = 202,
    Slashing = 203,
    HeavySlashing = 204,
    Stabbing = 205,
}