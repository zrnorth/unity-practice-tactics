using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;
    [SerializeField]
    private Texture2D _highlightTexture;

    private Vector3Int _tileCellUnderMouse;
    private GameObject _highlightSpriteGameObject;
    private Sprite _highlightSprite;
    private SpriteRenderer _highlightRenderer;

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
    }

    private void OnDestroy()
    {
        if (_highlightSprite != null)
        {
            Destroy(_highlightSpriteGameObject);
        }
    }
}
