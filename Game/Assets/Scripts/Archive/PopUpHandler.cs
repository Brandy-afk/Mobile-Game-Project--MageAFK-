// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using MageAFK.Management;
// using MageAFK.Animation;

// namespace MageAFK.UI
// {
//   public class PopUpHandler : MonoBehaviour, ISingletonInitializer
//   {
//     public static PopUpHandler Instance { get; private set; }

//     [SerializeField] private List<PopUp> popUps;
//     [SerializeField] private List<PopUpReferences> popUpReferences;


//     [Header("Animation")]
//     [SerializeField] private Vector3 leftTargetPopUp;
//     [SerializeField] private Vector3 bounceVariance;
//     [SerializeField] private float animationSpeed;
//     [SerializeField] private float popUpDuration;
//     [SerializeField] private float timeBetweenPopUps;

//     private Dictionary<PanelType, PopUp> popUpDict;
//     private Dictionary<PopUpType, PopUpReferences> references;
//     private Queue<PopUpRequest> popUpQueue;  // Queue for popup requests
//     private bool isDisplayingPopup;  // Flag to track if a popup is currently being displayed

//     private PopUp currentPopUp;

//     private Vector3 leftOrginPopUp;

//     public void InitializeSingleton()
//     {
//       if (Instance == null)
//       {
//         Instance = this;
//       }
//       else
//       {
//         Destroy(gameObject);
//         return;
//       }
//     }
//     private void Awake()
//     {
//       popUpDict = new Dictionary<PanelType, PopUp>();
//       references = new Dictionary<PopUpType, PopUpReferences>();
//       popUpQueue = new Queue<PopUpRequest>();

//       foreach (var popUp in popUps)
//       {
//         popUpDict[popUp.type] = popUp;
//       }
//       foreach (var reference in popUpReferences)
//       {
//         references.Add(reference.type, reference);
//       }

//       leftOrginPopUp = popUps[0].popup.anchoredPosition;


//     }


//     public void RequestPopUp(PanelType panel, PopUpType type, string text)
//     {
//       Sprite image;
//       if (type != PopUpType.level) { image = references[type].sprite; }
//       else { image = null; }

//       popUpQueue.Enqueue(new PopUpRequest(panel, text, image));

//       // If not currently displaying a popup, dequeue the next request
//       if (!isDisplayingPopup)
//       {
//         StartCoroutine(HandleNextPopUp());
//       }
//     }

//     private IEnumerator HandleNextPopUp()
//     {


//       if (popUpQueue.Count == 0)
//       {
//         yield break;
//       }
//       isDisplayingPopup = true;
//       yield return new WaitForSeconds(timeBetweenPopUps);

//       var request = popUpQueue.Dequeue();

//       var popUp = popUpDict[request.Type];
//       currentPopUp = popUp;
//       popUp.text.text = request.Text;
//       if (request.Image != null)
//       {
//         popUp.image.sprite = request.Image;
//       }

//       currentPopUp.popup.gameObject.SetActive(true);
//       UIAnimations.Instance.SlideAndBounce(popUp.popup, leftTargetPopUp, bounceVariance, animationSpeed, () => { StartCoroutine(OnPopUpAnimationFinished()); });

//     }

//     public IEnumerator OnPopUpAnimationFinished()
//     {
//       yield return new WaitForSeconds(popUpDuration);

//       UIAnimations.Instance.SlideLocal(currentPopUp.popup, leftOrginPopUp, animationSpeed, () =>
//       {
//         currentPopUp.popup.gameObject.SetActive(false);
//         currentPopUp = null;


//         isDisplayingPopup = false;
//         // If there are more popups in the queue, handle the next one
//         if (popUpQueue.Count > 0)
//         {
//           StartCoroutine(HandleNextPopUp());
//         }
//       });

//     }




//   }

//   public enum PanelType
//   {
//     DefaultPopUp,
//     LevelPopUp
//   }

//   public enum PopUpType
//   {
//     item,
//     research,
//     order,
//     location,
//     level,
//     spellUnlock



//   }

//   [System.Serializable]
//   public class PopUp
//   {
//     public PanelType type;
//     public RectTransform popup;
//     public TMP_Text text;
//     public Image image;
//   }

//   [System.Serializable]
//   public class PopUpReferences
//   {
//     public PopUpType type;
//     public Sprite sprite;
//   }

//   [System.Serializable]

//   public struct PopUpRequest
//   {
//     public PanelType Type { get; private set; }
//     public string Text { get; private set; }
//     public Sprite Image { get; private set; }

//     public PopUpRequest(PanelType type, string text, Sprite image = null)
//     {
//       Type = type;
//       Text = text;
//       Image = image;
//     }
//   }
// }
