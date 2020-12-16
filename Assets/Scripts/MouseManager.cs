using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    [SerializeField]
    private Grid _grid; // The entire grid both inside and outside the map (for highlighting)
    [SerializeField]
    private Map _map; // The tilemap layer specified as pathable by units
    [SerializeField]
    private Texture2D _highlightTexture, _selectTexture;

    private Vector3Int _tileCellUnderMouse;
    private GameObject _highlightSpriteGameObject;
    private Sprite _highlightSprite;
    private SpriteRenderer _highlightRenderer;
    private GameObject _selectSpriteGameObject;
    private Sprite _selectSprite;
    private SpriteRenderer _selectRenderer;


    private void Start()
    {
        // Create a child object which holds the highlight sprite
        _highlightSpriteGameObject = new GameObject("Highlight Sprite", typeof(SpriteRenderer));
        _highlightSpriteGameObject.transform.parent = this.transform;
        _highlightRenderer = _highlightSpriteGameObject.GetComponent<SpriteRenderer>();
        // Initialize the sprite
        Rect rect = new Rect(0, 0, _highlightTexture.width, _highlightTexture.height);
        Vector2 pivot = new Vector2(0, 0);
        _highlightSprite = Sprite.Create(_highlightTexture, rect, pivot, 16);
        _highlightRenderer.sprite = _highlightSprite;
        _highlightRenderer.sortingLayerName = "Foreground";

        // Create a child object which holds the select sprite
        _selectSpriteGameObject = new GameObject("Select Sprite", typeof(SpriteRenderer));
        _selectSpriteGameObject.transform.parent = this.transform;
        _selectRenderer = _selectSpriteGameObject.GetComponent<SpriteRenderer>();
        // Initialize the sprite
        rect = new Rect(0, 0, _selectTexture.width, _selectTexture.height);
        pivot = new Vector2(0, 0);
        _selectSprite = Sprite.Create(_selectTexture, rect, pivot, 16);
        _selectRenderer.sortingLayerName = "Foreground";
        // Start with nothing selected i.e. _selectSprite.sprite == null
    }

    void Update()
    {
        // Get the currently hovered tile cell
        Vector3Int newTileCellUnderMouse = _grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (newTileCellUnderMouse != _tileCellUnderMouse)
        {
            _tileCellUnderMouse = newTileCellUnderMouse;
            // Move the highlight sprite to be on the currently hovered tile cell.
            _highlightSpriteGameObject.transform.position = _grid.CellToWorld(_tileCellUnderMouse);
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    private void HandleLeftClick()
    {
        Debug.Log("mouse down");
        bool selected = _map.SelectTile(_tileCellUnderMouse);
        if (selected)
        {
            _selectSpriteGameObject.transform.position = _grid.CellToWorld(_tileCellUnderMouse);
            _selectRenderer.sprite = _selectSprite;
        }
        else
        {
            _selectRenderer.sprite = null;
        }
    }

    private void HandleRightClick()
    {
        // Right click moves units, so tell the map we want to update the selected unit's path.
        _map.GeneratePathTo(_tileCellUnderMouse.x, _tileCellUnderMouse.y);
    }

    private void OnDestroy()
    {
        if (_highlightSprite != null)
        {
            Destroy(_highlightSpriteGameObject);
        }
    }
}
