using System;
using System.Collections.Generic;
using UnityEngine;

namespace MageAFK.UI
{
  public class Pagination<T>
  {
    private IList<T> items; // The list of all items
    private int currentPage; // The current page
    private int itemsPerPage; // Number of items per page
    private int totalPages; // Total number of pages
    private IPagination<T> script;

    // Initialize the pagination
    public Pagination(IList<T> items, IPagination<T> script, int itemsPerPage, int currentPage = 1)
    {
      this.items = items;
      this.itemsPerPage = itemsPerPage;
      var itemsCount = items != null ? items.Count : 0;
      totalPages = (int)Math.Ceiling(itemsCount / (double)itemsPerPage);
      this.currentPage = currentPage;
      this.script = script;

    }

    // Update the displayed items based on the current page
    public void UpdateDisplay()
    {
      script.UpdatePageButtons();
      script.CustomPaginationBehaviour();

      int startIndex = (currentPage - 1) * itemsPerPage;

      // Calculate items to be shown on the current page
      int itemsOnThisPage = Mathf.Min(itemsPerPage, (items?.Count ?? 0) - startIndex);

      for (int i = 0; i < itemsOnThisPage; i++)
      {
        script.UpdateSlot(items[startIndex + i], i);
      }

      // If there are any remaining slots, set them to null
      for (int i = itemsOnThisPage; i < itemsPerPage; i++)
      {

        script.UpdateSlot(default, i);
      }
    }

    // Go to the next page
    public void NextPage()
    {
      if (currentPage < totalPages)
      {
        currentPage++;
        UpdateDisplay();
      }
    }

    // Go to the previous page
    public void PreviousPage()
    {
      if (currentPage > 1)
      {
        currentPage--;
        UpdateDisplay();
      }
    }

    public void SetPage(int page)
    {
      if (page >= 1 && page <= totalPages)
      {
        currentPage = page;
        UpdateDisplay();
      }

    }

    public int ReturnTotalPages() => totalPages;
    public int ReturnCurrentPage() => currentPage;
    public bool ReturnIfOnLastPage() => !(currentPage < totalPages);
    public bool ReturnIfOnFirstPage() => !(currentPage > 1);

    public static void UpdatePageButtons(Pagination<T> p, ButtonUpdateClass[] buttons)
    {

      if (p == null)
      {
        foreach (var button in buttons)
        {
          button.black.SetActive(true);
          button.button.interactable = false;
        }
        return;
      }

      bool leftState = p.ReturnIfOnFirstPage();
      bool rightState = p.ReturnIfOnLastPage();

      buttons[0].black.SetActive(leftState);
      buttons[0].button.interactable = !leftState;
      buttons[1].black.SetActive(rightState);
      buttons[1].button.interactable = !rightState;
    }
  }

  public interface IPagination<T>
  {
    void UpdatePageButtons();

    void UpdateSlot(T type, int index);

    void CustomPaginationBehaviour();

    void AlterPagePressed(bool isNext);

  }

}
