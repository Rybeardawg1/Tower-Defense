using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector3 originalPosition;  // Store the original position of the button
    private Vector2 originalAnchoredPosition;
    public GameObject towerPrefab;    // Tower prefab to place

    private GridGenerator gridGenerator;  // Reference to the GridGenerator for valid zones

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        originalPosition = rectTransform.position; // Save the button's original position
        originalAnchoredPosition = rectTransform.anchoredPosition; // Anchored position
        gridGenerator = FindObjectOfType<GridGenerator>(); // Find the grid generator
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Make the button semi-transparent
        canvasGroup.blocksRaycasts = false; // Allow raycasts to pass through
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the button with the mouse
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Restore the button's opacity
        canvasGroup.blocksRaycasts = true; // Block raycasts again

        // Perform the tower placement
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PlaceTower(hit.point); // Attempt to place the tower at the hit point
        }

        // Reset the button to its original position in the UI
        ResetButtonPosition();
    }

    private void PlaceTower(Vector3 position)
    {
        float cellSize = 1f;

        // Snap position to the grid
        Vector2Int gridPosition = new Vector2Int(
            Mathf.RoundToInt(position.x / cellSize),
            Mathf.RoundToInt(position.z / cellSize)
        );

        // Validate if the grid position is a valid tower placement zone
        if (gridGenerator.towerPlacementZones.Contains(gridPosition))
        {
            position.x = gridPosition.x * cellSize;
            position.z = gridPosition.y * cellSize;
            position.y = 0; // Align with ground level

            // Instantiate the tower at the position
            Instantiate(towerPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"Invalid placement! Cannot place tower at {gridPosition}.");
        }
    }

    private void ResetButtonPosition()
    {
        // Reset the button position to its original UI position
        // rectTransform.position = originalPosition; // Resets the world position
        rectTransform.anchoredPosition = originalAnchoredPosition;
        rectTransform.localPosition = new Vector3(originalAnchoredPosition.x, originalAnchoredPosition.y, 0);

        // rectTransform.anchoredPosition = Vector2.zero; // Resets local position (optional, if UI hierarchy affects placement)
    }
}
