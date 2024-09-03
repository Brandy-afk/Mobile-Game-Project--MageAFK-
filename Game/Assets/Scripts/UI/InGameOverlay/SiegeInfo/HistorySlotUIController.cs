using UnityEngine;
using TMPro;
using MageAFK.Core;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MageAFK.Management;

namespace MageAFK.UI
{
  public class HistorySlotUIController : MonoBehaviour, IPointerClickHandler
  {


    [Header("Interaction Display")]
    [SerializeField] private HistoryPopUp historyPopUp;

    [Header("UI Display")]
    [SerializeField] private TMP_Text wave;
    [SerializeField] private TMP_Text location;
    [SerializeField] private TMP_Text date;
    [SerializeField] private Image locationImage;
    [SerializeField] private GameObject highScore;


    [HideInInspector] public SiegeStatistic stats;
    public bool active = false;

    public void OnPointerClick(PointerEventData eventData)
    {
      if (!active) return;
      historyPopUp.OpenPopUp(stats);
    }

    public void BuildSlot(SiegeStatistic siegeStatistics)
    {
      active = true;
      stats = siegeStatistics;

      var image = GetComponent<Image>();
      Color color = image.color;
      color.a = 1f;
      image.color = color;

      transform.GetChild(0).gameObject.SetActive(true);

      wave.text = "Wave " + siegeStatistics.wave.ToString();
      location.text = $"The {siegeStatistics.location}";
      locationImage.sprite = ServiceLocator.Get<LocationHandler>().ReturnLocationData(siegeStatistics.location).image;
      date.text = siegeStatistics.dateFinished;
      highScore.SetActive(siegeStatistics.isBestWave);
    }
  }

}