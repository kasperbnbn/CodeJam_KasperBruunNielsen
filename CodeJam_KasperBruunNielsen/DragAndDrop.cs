using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RectTransform correctPosition;
    public float snapDistance = 10f;
    public float returnSpeed = 5f;

    public float Alp = .6f;
    public float Alph = 1;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private bool isDragging = false;

    //Gets the components
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPosition = rectTransform.anchoredPosition;
    }

    //Method to start the drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = Alp;
        //Block other images, so you can drag over the images.
        canvasGroup.blocksRaycasts = true;
        isDragging = true;
    }

    //Method during the dragging
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    //method for stopping the drag
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = Alph;

        // Calculate the distance between the dropped position and the correct position
        float distance = Vector2.Distance(rectTransform.anchoredPosition, correctPosition.anchoredPosition);

        // If the distance is within the snap distance, snap the image to the correct position
        if (distance <= snapDistance)
        {
            rectTransform.anchoredPosition = correctPosition.anchoredPosition;

            // Check if all images are on the selected position
            int numCorrect = 0;
            foreach (DragAndDrop dnd in FindObjectsOfType<DragAndDrop>())
            {
                if (Vector2.Distance(dnd.rectTransform.anchoredPosition, dnd.correctPosition.anchoredPosition) <= snapDistance)
                {
                    numCorrect++;
                }
            }

            // Switches scene if all images are on their selceted spot.
            if (numCorrect == FindObjectsOfType<DragAndDrop>().Length)
            {
                SceneManager.LoadScene(3);
            }
        }
        else
        {
            // If the object is not placed correctly, return it to its original position
            isDragging = false;
            StartCoroutine(ReturnToStart());
        }

        canvasGroup.blocksRaycasts = true;
    }

    // Coroutine to return the object to its original position
    private IEnumerator ReturnToStart()
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, startPosition) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, startPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }
        rectTransform.anchoredPosition = startPosition;
    }

    // Stop the return coroutine if the object is being dragged again
    private void Update()
    {
        if (isDragging)
        {
            StopCoroutine(ReturnToStart());
        }
    }
}
