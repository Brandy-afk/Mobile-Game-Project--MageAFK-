

using UnityEngine;
using MageAFK.AI;
using System.Collections.Generic;
using MageAFK.Testing;
using MageAFK.Stats;
using MageAFK.Pooling;
using MageAFK.Management;
using MageAFK.Tools;
using System.Collections;


namespace MageAFK.Spells
{

  public class TornadoProjectile : SpellProjectile, IRangeVisualizer
  {

    //Set up logic based on base class
    public SuctionSpellTesting testing;
    private Dictionary<GameObject, float> lastDamageTime = new();
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float startTime;
    private bool isActive = false;
    private Transform rangeVisual;

    private Animator animator;

    private void Awake() => animator = GetAnimator();

    private void OnEnable() => StartCoroutine(SetActive());

    public IEnumerator SetActive()
    {
      yield return new WaitForSeconds(.3f);
      rangeVisual = ServiceLocator.Get<SpellPooler>().Get(SpellIdentification.SpellUtility_Range, spell.iD).transform;
      rangeVisual.gameObject.SetActive(true);
      rangeVisual.GetComponent<SpellRangeVisualizer>().SetUpVisualizer(spell.ReturnStatValue(Stat.Range), this);
      rangeVisual.SetParent(transform);

      rangeVisual.localPosition = Vector3.zero;

      isActive = true;
      startTime = Time.time;

      startPosition = transform.position;

      float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
      targetPosition = startPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * testing.travelDistance;
    }

    private void Update()
    {
      if (!isActive) return;

      float journeyLength = Vector2.Distance(startPosition, targetPosition);
      float distCovered = (Time.time - startTime) * testing.speed;
      float fracJourney = distCovered / journeyLength;

      // Linearly interpolate from start to target position
      Vector2 currentPos = Vector2.Lerp(startPosition, targetPosition, fracJourney);

      // Calculate offset using sine wave for 'S' pattern
      float sineOffset = Mathf.Sin(fracJourney * Mathf.PI * testing.sineWaveFrequency) * testing.sineWaveMagnitude;
      Vector2 perpendicularDirection = new Vector2(targetPosition.y, -targetPosition.x).normalized;
      currentPos += perpendicularDirection * sineOffset;

      transform.position = currentPos;

      // Check if the tornado has reached its target
      if (fracJourney >= 1.0f)
      {
        OnTargetReached();
      }
    }


    public void OnStay(Collider2D other)
    {
      if (!isActive || !Utility.VerifyTags(targetTags, other)) { return; }
      NPEntity entity = other.GetComponentInParent<NPEntity>();
      Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
      if (rb && entity)
      {
        HandleSuction(entity, rb);
        DoDamage(entity);
      }
    }

    public void DoDamage(NPEntity entity)
    {
      if (!lastDamageTime.TryGetValue(entity.gameObject, out float lastTime) || Time.time - lastTime >= testing.timeBetweenDamage)
      {
        // Update last damage time.
        HandleDamage(entity);
        lastDamageTime[entity.gameObject] = Time.time;
      }
    }

    public void HandleSuction(NPEntity entity, Rigidbody2D rb)
    {
      if (entity.states[States.isRooted] || entity.states[States.isDead]) { return; }
      else
      {
        Vector2 currentVelocity = rb.velocity;
        float forceMagnitude = Mathf.Clamp(currentVelocity.magnitude, 0, testing.maxForce);

        // Apply force in the opposite direction of the current trajectory
        Vector2 forceDirection = -currentVelocity.normalized;

        Vector2 force = forceDirection * forceMagnitude * testing.suctionPower;
        rb.AddForce(force);
      }
    }

    private void OnTargetReached()
    {
      isActive = false;
      animator.Play("End");
      StartCoroutine(AnimationDisable(animator));
      rangeVisual.gameObject.SetActive(false);
      rangeVisual = null;
    }


    #region Not in Use
    public void OnEnter(Collider2D other)
    {

    }

    public void OnLeave(Collider2D other)
    {

    }
    #endregion
  }

}
