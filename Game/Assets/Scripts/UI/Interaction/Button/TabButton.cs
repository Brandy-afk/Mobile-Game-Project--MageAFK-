
using UnityEngine;
using UnityEngine.EventSystems;
using MageAFK.Items;
using MageAFK.Spells;

namespace MageAFK.UI
{
  public class TabButton : MonoBehaviour, IPointerClickHandler
  {
    [SerializeField] protected TabGroup tabGroup;

    [Header("Custom Fields")]
    [SerializeField] protected int buttonID = 0;
    [SerializeField] protected bool DontIntializeOnStart;


    private void Awake()
    {
      if (!DontIntializeOnStart)
      {
        Initialize();
      }
    }
    public virtual void Initialize() => tabGroup.Subscribe(this);
    public virtual void OnPointerClick(PointerEventData eventData) => tabGroup.OnTabSelected(this);
    public virtual int GetID() => buttonID;
  }





  public interface ISpellType
  {
    SpellType ReturnType();

  }

  public interface IUIPanelProvider
  {
    UIPanel ReturnPanel();
  }



}