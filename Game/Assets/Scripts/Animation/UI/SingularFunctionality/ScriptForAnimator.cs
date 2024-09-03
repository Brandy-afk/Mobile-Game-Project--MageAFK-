using MageAFK.UI;
using UnityEngine;

namespace MageAFK.Animation
{

  public class ScriptWithAnimator : MonoBehaviour
  {

    [SerializeField] private BookTabGroup group;
    [SerializeField] private OverlayAnimationHandler gameOverlayGroup;

    public void OpenAnimationFinished()
    {
      gameOverlayGroup.OnOpenBookAnimationFinished();
      group.FadeInPanel();

    }

    public void CloseAnimationFinished() => gameOverlayGroup.OnBookClosedCallBack();
  }




}