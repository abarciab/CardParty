using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InventorySorting : MonoBehaviour
{
    [SerializeField] private InventoryUI _controller;
    [SerializeField] private TextMeshProUGUI _searchBoxText;
    [SerializeField] private SelectableItem _alphaButton;
    [SerializeField] private SelectableItem _valueButton;
    [SerializeField] private SelectableItem _recencyButton;

    private const int _alphabeticalOrder = 0;
    private const int _valueOrder = 1;
    private const int _recencyOrder = 2;

    private void OnEnable()
    {
        _searchBoxText.text = "";
        ChangeSortOrder(0);
    }

    public async void OnTextChanged()
    {
        await Task.Delay(10);
        var searchTerm = _searchBoxText.text;
        print("searching with new term: " + searchTerm);
        _controller.SetSearchTerm(searchTerm.ToUpper());
    }

    public void ChangeSortOrder(int order)
    {
        _alphaButton.SetState(order == _alphabeticalOrder);
        _valueButton.SetState(order == _valueOrder);
        _recencyButton.SetState(order == _recencyOrder);

        _controller.ChangeSortOrder((SortOrderType) order);
    }
}
