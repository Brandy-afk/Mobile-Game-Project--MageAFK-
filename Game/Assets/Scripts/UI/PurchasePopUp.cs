using System;
using MageAFK.Animation;
using MageAFK.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PurchasePopUp : MonoBehaviour
{
  [SerializeField] protected TMP_Text title;
  [SerializeField] protected Image image;
  [SerializeField] protected TMP_Text cost;
  [SerializeField] protected TMP_Text desc;
  [SerializeField] protected Button blackMask;
  [SerializeField] protected Button[] purchaseButtons;
  public event Action<bool> OnDecision;
  [SerializeField] private GameObject callerObjectPanel;

  protected virtual void OnEnable()
  {
    purchaseButtons[0].onClick.AddListener(() => OnChoicePressed(false));
    purchaseButtons[1].onClick.AddListener(() => OnChoicePressed(true));
    blackMask.onClick.AddListener(() => OnChoicePressed(false));
  }

  protected virtual void OnDisable()
  {
    blackMask.onClick.RemoveAllListeners();
    purchaseButtons[0].onClick.RemoveAllListeners();
    purchaseButtons[1].onClick.RemoveAllListeners();
  }

  protected virtual void OpenPanel()
  {
    blackMask.gameObject.SetActive(true);
    OverlayAnimationHandler.SetIsAnimating(true);
    callerObjectPanel.SetActive(false);
    UIAnimations.Instance.OpenPanel(gameObject, () => OverlayAnimationHandler.SetIsAnimating(false));
  }

  protected virtual void ClosePanel()
  {
    
      gameObject.SetActive(false);
      OverlayAnimationHandler.SetIsAnimating(false);
      blackMask.gameObject.SetActive(false);
      callerObjectPanel.SetActive(true);
  }

  protected virtual void SetButtonStates(bool state)
  {
    for (int i = 0; i < purchaseButtons.Length; i++)
    {
      purchaseButtons[i].gameObject.SetActive(state);
    }
    cost.gameObject.SetActive(state);
  }

  public void OnChoicePressed(bool purchase)
  {
    OnDecision.Invoke(purchase);
    ClosePanel();
    OnDecision = null;
  }
}