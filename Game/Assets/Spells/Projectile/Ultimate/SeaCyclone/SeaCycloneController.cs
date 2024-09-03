using System.Collections;
using MageAFK.Spells;
using MageAFK.Stats;
using UnityEngine;

namespace MageAFK
{
    public class SeaCycloneController : MonoBehaviour
    {
        [SerializeField] private SeaCyclone spell;
        private int shotNumber = 0;

        private bool active = false;
        private float spawnTime;
        private float duration;

        public void ToggleActive()
        {
            spawnTime = Time.time;
            duration = spell.ReturnStatValue(Stat.SpellDuration);
            active = true;
            StartCoroutine(ShootProjectiles());
        }

        private void Update()
        {
            if (active && Time.time > spawnTime + duration)
            {
                active = false;
                GetComponent<Animator>()?.Play("End");
            }

        }

        private IEnumerator ShootProjectiles()
        {
            while (active)
            {
                shotNumber = spell.ShootWaterShot(shotNumber, transform.position);
                yield return new WaitForSeconds(spell.ReturnStatValue(Stat.AttackCooldown, modStat: Stat.Cooldown));
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
