using System;
using MageAFK.Animation;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Intended to be a base class for all popups during waves so they can naturally sub to players death and handle closing themselfs.
/// </summary>
public abstract class WavePopUp : SerializedMonoBehaviour
{

  public event Action OnClosed;
  protected GameObject parent;
  protected virtual void OnEnable() => ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(Close, true);

  protected virtual void OnDisable() => ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(Close, false);

  protected virtual void Open()
  {
    if (parent == null) parent = transform.parent.gameObject;
    parent.SetActive(true);
    OverlayAnimationHandler.SetIsAnimating(true);
    UIAnimations.Instance.OpenPanel(gameObject, () => OverlayAnimationHandler.SetIsAnimating(false));
  }

  protected virtual void Close()
  {
    OnClosed?.Invoke();
    OnClosed = null;
    OverlayAnimationHandler.SetIsAnimating(true);
    UIAnimations.Instance.ClosePanel(gameObject, () =>
    {
      OverlayAnimationHandler.SetIsAnimating(false);
      parent.SetActive(false);
    });
  }
}