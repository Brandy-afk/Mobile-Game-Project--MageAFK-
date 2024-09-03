using MageAFK.Core;
using UnityEngine;

namespace MageAFK.Animation
{
    public class LootAM : MonoBehaviour
  {

    UnityEngine.Material material;
    [SerializeField] float animationSpeed;
    [SerializeField] float speed;
    [SerializeField] string TagLocation;

    [SerializeField] CurrencyType currencyType;
    Vector3 targetPosition;




    int value = 0;
    private int spawnValue = 0;
    bool allowMovement = false;
    bool isFadingIn = false;

    float fade = 0;





    private void Start()
    {
      material = GetComponent<SpriteRenderer>().material;

      RectTransform targetTransform = GameObject.FindWithTag(TagLocation).GetComponent<RectTransform>();
      // Get the screen position of the UI element
      Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, targetTransform.position);
      // Get the local position of the UI element within its RectTransform
      Vector2 localPosition;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(targetTransform, screenPosition, null, out localPosition);
      // Convert the local position to a world position
      Vector3 worldPosition = targetTransform.TransformPoint(localPosition);
      // Convert the world position to a camera-relative position
      targetPosition = Camera.main.ScreenToWorldPoint(worldPosition);
    }

    private void OnEnable()
    {
      //Initial Animation upon being enabled
      spawnValue += 1;
      if (spawnValue <= 1) { return; }
      isFadingIn = true;
      transform.LeanMoveLocal(new Vector2(transform.position.x, transform.position.y + .75f), animationSpeed).setEaseOutQuart().setOnComplete(AllowMovement);

    }

    private void Update()
    {
      ObjectMovement();
      OnObjectDisable();
      if (isFadingIn)
      {
        InitialFadeIn();
      }
    }

    //Fade in shader
    private void InitialFadeIn()
    {
      fade += Time.deltaTime;
      if (fade >= 1f)
      {
        isFadingIn = false;
      }
      material.SetFloat("_Fade", fade);
    }

    //Actions carried out on disable
    private void OnObjectDisable()
    {
      if (transform.position.x == targetPosition.x && transform.position.y == targetPosition.y)
      {
        allowMovement = false;
        // PlayerData.Instance.GetCurrencyHandler().AddCurrency(currencyType, value);
        fade = 0f;
        gameObject.SetActive(false);
      }
    }

    //Handles objects movement torwards position
    private void ObjectMovement()
    {
      if (allowMovement)
      {
        float delta = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, delta);
      }
    }

    //Function that simply allows (ObjectMovement) to function
    private void AllowMovement()
    {
      allowMovement = true;
    }

    //The value of this demon soul is passed through the AIController upon Spawn.
    public void GiveValue(int v)
    {
      value = v;
    }

  }

}