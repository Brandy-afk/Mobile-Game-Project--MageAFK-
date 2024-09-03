using System.Collections;
using MageAFK.AI;
using MageAFK.Items;
using MageAFK.Management;
using MageAFK.Tools;
using UnityEngine;

namespace MageAFK
{
    public class BloodPendant_Mine : AbstractDamager
    {

        [SerializeField] private float idleTime = 1f;
        [SerializeField] private float speed = 7f;

        private float Speed => speed / 100;
        private BloodPendant bloodPendant;
        private bool isActive = false;
        private Vector2 targetLoc;
        private bool moveToTarget = false;
        private Animator animator;
        private Collider2D target;

        private void Awake()
        {
            GearHandler gearHandler = ServiceLocator.Get<GearHandler>();
            bloodPendant = gearHandler.ReturnItem(ItemType.Jewelry) as BloodPendant;
            damage = bloodPendant.damage;
            animator = GetComponent<Animator>();
        }

        public void SetActive()
        {
            isActive = true;
            StartCoroutine(IdleThenSetTarget());
        }

        private void Update()
        {
            if (!isActive) return;
            Move();
        }

        private void Move()
        {
            var isInRange = Utility.GetIsInRange(transform.position, targetLoc, 2f);
            if (moveToTarget && !isInRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetLoc, Speed);
            }
            else if (moveToTarget && isInRange)
            {
                moveToTarget = false;
                StartCoroutine(IdleThenSetTarget());
            }
        }


        private IEnumerator IdleThenSetTarget()
        {
            yield return new WaitForSeconds(idleTime);
            targetLoc = Utility.GetRandomMapPosition(maxY: .8f);
            moveToTarget = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isActive)
            {
                isActive = false;
                StopAllCoroutines();
                animator.Play("End");
                target = other;
            }
        }

        private void OnExplosion() => DoDamage(target.GetComponent<NPEntity>());

        private void Disable()
        {
            bloodPendant.ReduceSpawnedObjects();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (gameObject.activeInHierarchy)
                bloodPendant.ReduceSpawnedObjects();
        }


    }
}
