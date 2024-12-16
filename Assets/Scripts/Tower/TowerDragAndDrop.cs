using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TowerDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector3 originalPosition;  // To store original position of the button

    public GameObject towerPrefab; // The tower prefab to be placed

    private GridGenerator gridGenerator;  // Reference to GridGenerator to access valid zones

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        originalPosition = transform.position; // Store the original position
        gridGenerator = FindObjectOfType<GridGenerator>(); // Get GridGenerator reference
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Make the button semi-transparent
        canvasGroup.blocksRaycasts = false; // Allow other objects to interact with raycasts
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Move the button with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Restore the button's opacity
        canvasGroup.blocksRaycasts = true; // Block raycasts again

        // Perform the tower placement
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SpawnTower(hit.point);  // Place the tower at the hit point
        }
    }

    private void SpawnTower(Vector3 position)
    {
        float cellSize = 1f;

        // Snap position to the grid
        Vector2Int gridPosition = new Vector2Int(
            Mathf.FloorToInt((position.x + cellSize / 2) / cellSize),
            Mathf.FloorToInt((position.z + cellSize / 2) / cellSize)
        );

        // Validate placement
        if (gridGenerator.towerPlacementZones.Contains(gridPosition) &&
            !gridGenerator.pathPositions.Contains(gridPosition)) // Ensure it's not a path position
        {
            position.x = gridPosition.x * cellSize;
            position.z = gridPosition.y * cellSize;
            position.y = 0; // Align with ground level

            // Instantiate the tower
            Instantiate(towerPrefab, position, Quaternion.identity);

            // Hide the button after tower is placed
            gameObject.SetActive(false);  // Hide the TowerButton

            // Return the button to its original position after some delay
            StartCoroutine(ReturnButtonToOriginalPosition());
        }
        else
        {
            Debug.LogWarning($"Invalid placement! Cannot place tower at {gridPosition}. Not a valid zone.");
        }
    }

    private IEnumerator ReturnButtonToOriginalPosition()
    {
        // Wait for a short time before returning the button
        yield return new WaitForSeconds(1f);

        // Move the button back to its original position
        transform.position = originalPosition;

        // Make the button visible again
        gameObject.SetActive(true);
    }
}
