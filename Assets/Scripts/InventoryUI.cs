using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _equipmentGridItemPrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private GameObject _itemSelection;
    [SerializeField] private float _maxCardTilt = 4;
    [SerializeField] private AnimationCurve _tiltDistributionCurve;
    [SerializeField] private TextMeshProUGUI _pageSelectorText;
    [SerializeField] private SelectableItem _prevPage;
    [SerializeField] private SelectableItem _nextPage;

    private List<InventoryGridItem> _gridItems = new List<InventoryGridItem>();
    private List<InventoryGridItem> _filteredList = new List<InventoryGridItem>();
    private int _currentPage;
    private int _maxPage;
    private const int _numPerPage = 5;
    private bool _filtering;

    private bool _atFirstPage => _currentPage == 0;
    private bool _atLastPage => _currentPage == _maxPage - 1;

    public void ChangeSortOrder(SortOrderType order)
    {
        var unsortedList = new List<InventoryGridItem>(_filteredList);
        var sortedList = new List<InventoryGridItem>();

        if (order == SortOrderType.ALPHABET) {
            sortedList = unsortedList.OrderBy(x => x.Data.Name).ToList();
        }
        if (order == SortOrderType.VALUE) {
            sortedList = unsortedList.OrderByDescending(x => x.Data.Cost).ToList();
            print("sortedByCost:");
            foreach (var s in sortedList) print(s.Data.Name + ": " + s.Data.Cost);
        }
        if (order == SortOrderType.RECENCY) {
            print("not yet implemented!");
            return;
        }

        _filtering = true;
        _filteredList = new List<InventoryGridItem>(sortedList);
        for (int i = 0; i < _filteredList.Count; i++) {
            _filteredList[i].transform.SetSiblingIndex(i);
        }

        UpdateDisplay();
    }

    public void SetSearchTerm(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) {
            _currentPage = 0;
            _maxPage = CalculatePageCount(_gridItems.Count);
            _filtering = false;
            return;
        }

        _filteredList.Clear();
        _filtering = true;
        var allList = new List<InventoryGridItem>(_gridItems);
        foreach (var item in allList) if (CheckName(item.Data.name, searchTerm)) _filteredList.Add(item);

        _currentPage = 0;
        _maxPage = CalculatePageCount(_filteredList.Count);
        UpdateDisplay();
    }

    private bool CheckName(string name, string searchTerm)
    {
        name = name.ToUpper().Trim();
        searchTerm = searchTerm.Remove(searchTerm.Length - 1);
        searchTerm = searchTerm.ToUpper().Trim();


        bool matches = name.Contains(searchTerm);
        return matches;
    }

    private int CalculatePageCount(int itemCount) => Mathf.CeilToInt(itemCount / (float)_numPerPage);

    public void Show()
    {
        foreach (var item in _gridItems) Destroy(item.gameObject);
        _gridItems.Clear();

        gameObject.SetActive(true);
        var equipment = PlayerInfo.Inventory.Equipment;
        foreach (var e in equipment) InitializeEquipment(e);
        _filteredList = new List<InventoryGridItem>(_gridItems);
        _currentPage = 0;
        _maxPage = CalculatePageCount(_gridItems.Count);
        _filtering = false;

        UpdateDisplay();
    }

    private void InitializeEquipment(Equipment data)
    {
        var newGridItem = Instantiate(_equipmentGridItemPrefab, _gridParent);
        float tiltAmount = Random.Range(0f, 1f);
        tiltAmount = _tiltDistributionCurve.Evaluate(tiltAmount);
        tiltAmount = ((tiltAmount * 2) - 1) * _maxCardTilt;
        newGridItem.transform.GetChild(0).eulerAngles = new Vector3(0, 0, tiltAmount);
        newGridItem.GetComponent<InventoryGridItem>().Initialize(data, this);
        _gridItems.Add(newGridItem.GetComponent<InventoryGridItem>());
    }

    private void UpdateDisplay()
    {
        _pageSelectorText.text = (_currentPage + 1) + "/" + _maxPage;
        _prevPage.SetEnabled(!_atFirstPage);
        _nextPage.SetEnabled(!_atLastPage);

        HideAllCards();
        var list = _filtering ? _filteredList : _gridItems;
        int startIndex = _currentPage == 0 ? 0 : _currentPage * _numPerPage;
        for (int i = startIndex; i < (_currentPage + 1) * _numPerPage; i++) {
            if (i >= 0 && i < list.Count) list[i].gameObject.SetActive(true);
        }
    }

    public void NextPage()
    {
        if (_currentPage >= _maxPage - 1) return;
        _currentPage += 1;
        UpdateDisplay();
    }

    public void PrevPage()
    {
        if (_currentPage <= 0) return;
        _currentPage -= 1;
        UpdateDisplay();
    }

    private void HideAllCards()
    {
        foreach (var item in _gridItems) item.gameObject.SetActive(false);
    }

    public void ShowItemDetails()
    {
        _itemSelection.SetActive(true);
    }
}
