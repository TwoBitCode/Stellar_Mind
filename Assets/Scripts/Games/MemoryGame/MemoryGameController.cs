using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int numObjects = 4;
    [SerializeField] private float shuffleDelay = 2f;
    [SerializeField] private int pointsToAdd = 50;

    [Header("Prefabs")]
    [SerializeField] private GameObject gridElementPrefab;
    [SerializeField] private GameObject stackElementPrefab;

    [Header("References")]
    [SerializeField] private Transform grid;
    [SerializeField] private Transform stack;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button checkAnswerButton;
    [SerializeField] private ScoreManager scoreManager;

    private GameObject[] gridElements;
    private Color[] initialColors;
    private GameObject[] stackElements;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        GenerateElements(grid, ref gridElements, numObjects, true);
        GenerateElements(stack, ref stackElements, numObjects, false);

        restartButton.gameObject.SetActive(false);
        StartCoroutine(ShuffleAfterDelay(shuffleDelay));
    }

    private void GenerateElements(Transform parent, ref GameObject[] elements, int count, bool assignColor)
    {
        elements = new GameObject[count];
        initialColors = assignColor ? new Color[count] : null;

        for (int i = 0; i < count; i++)
        {
            GameObject element = Instantiate(assignColor ? gridElementPrefab : stackElementPrefab, parent);
            elements[i] = element;

            if (assignColor)
            {
                DraggableItem draggable = element.GetComponentInChildren<DraggableItem>();
                Color randomColor = GetRandomColor();
                draggable.image.color = randomColor;
                initialColors[i] = randomColor;
            }
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    private IEnumerator ShuffleAfterDelay(float delay)
    {
        float countdown = delay;

        while (countdown > 0)
        {
            countdownText.text = Mathf.Ceil(countdown).ToString();
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        countdownText.text = "";
        ShuffleElements();
        MoveToStack();
    }

    private void ShuffleElements()
    {
        for (int i = 0; i < gridElements.Length; i++)
        {
            int randomIndex = Random.Range(0, gridElements.Length);
            Transform tempParent = gridElements[i].transform.parent;
            gridElements[i].transform.SetParent(gridElements[randomIndex].transform.parent);
            gridElements[randomIndex].transform.SetParent(tempParent);
        }
    }

    private void MoveToStack()
    {
        for (int i = 0; i < gridElements.Length; i++)
        {
            gridElements[i].transform.SetParent(stackElements[i].transform);
        }
    }

    public void CheckAnswer()
    {
        for (int i = 0; i < gridElements.Length; i++)
        {
            DraggableItem draggable = gridElements[i].GetComponentInChildren<DraggableItem>();
            if (draggable.image.color != initialColors[i])
            {
                resultText.text = "Incorrect Order!";
                restartButton.gameObject.SetActive(true);
                return;
            }
        }

        resultText.text = "Correct Order!";
        scoreManager.AddScore(pointsToAdd);
    }
}
