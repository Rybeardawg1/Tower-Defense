using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MathProblemManager : MonoBehaviour
{
    public TMP_Text easyMathProblemText;
    public TMP_Text mediumMathProblemText;
    public TMP_Text hardMathProblemText;

    public TMP_Text easyError;
    public TMP_Text mediumError;
    public TMP_Text hardError;

    [SerializeField] private TMP_InputField easyInputField;
    [SerializeField] private TMP_InputField mediumInputField;
    [SerializeField] private TMP_InputField hardInputField;

    public Button mathPanelButton;
    public TMP_Text mathButtonText;
    public bool isOpen;

    private HashSet<string> generatedNumbers = new HashSet<string>();
    private System.Random random = new System.Random();
    private const int Min = 0;
    private const int Max = 19;


    public HashSet<int> solvedEasyProblems;
    public HashSet<int> solvedMediumProblems;
    public HashSet<int> solvedHardProblems;

    public int currentEasyProblem;
    public int currentMediumProblem;
    public int currentHardProblem;
    public GameObject Panel;
    public GameManager gameManager;

    public enum ProblemType
    {
        Easy,
        Medium,
        Hard
    }

    void Start()
    {
        isOpen = true;
        GameObject game_manager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = game_manager.GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Internal error: could not find the GameManager object - did you remove its 'GameManager' tag?");
            return;
        }

        solvedEasyProblems = new HashSet<int>();
        solvedMediumProblems = new HashSet<int>();
        solvedHardProblems = new HashSet<int>();

        GetNextEasyProblem();
        GetNextMediumProblem();
        GetNextHardProblem();

        easyError.gameObject.SetActive(false);
        mediumError.gameObject.SetActive(false);
        hardError.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void solveEasy()
    {
        string userInput = easyInputField.text;
        string solution = MathProblemData.EasyProblems[currentEasyProblem].Solution;
        if (userInput == solution)
        {
            gameManager.UpdateBalance(20);
            showMessage(ProblemType.Easy, false);
            GetNextEasyProblem();
        }
        else
        {
            showMessage(ProblemType.Easy, true);
        }
    }

    public void solveMedium()
    {
        string userInput = mediumInputField.text;
        string solution = MathProblemData.MediumProblems[currentMediumProblem].Solution;
        if (userInput == solution)
        {
            gameManager.UpdateBalance(20);
            showMessage(ProblemType.Medium, false);
            GetNextMediumProblem();
        }
        else{
            showMessage(ProblemType.Medium, true);
        }
    }

    public void solveDifficult()
    {
        string userInput = hardInputField.text;
        string solution = MathProblemData.HardProblems[currentHardProblem].Solution;
        if (userInput == solution)
        {
            gameManager.UpdateBalance(20);
            showMessage(ProblemType.Hard, false);

            GetNextHardProblem();
        }
        else{
            showMessage(ProblemType.Hard, true);
        }
    }

    private void GetNextEasyProblem()
    {
        // Ensure we don't exceed the range's capacity
        if (solvedEasyProblems.Count >= (Max - Min + 1))
        {
            throw new InvalidOperationException("All numbers in the range have been generated.");
        }

        int number;
        do
        {
            number = random.Next(Min, Max + 1);
        }
        while (!solvedEasyProblems.Add(number)); // Try again if the number is already in the set

        currentEasyProblem = number;
        string problem = MathProblemData.EasyProblems[currentEasyProblem].Problem;
        easyMathProblemText.text = problem;
        easyInputField.text = "";
    }

    private void GetNextMediumProblem()
    {
        // Ensure we don't exceed the range's capacity
        if (solvedMediumProblems.Count >= (Max - Min + 1))
        {
            throw new InvalidOperationException("All numbers in the range have been generated.");
        }

        int number;
        do
        {
            number = random.Next(Min, Max + 1);
        }
        while (!solvedMediumProblems.Add(number)); // Try again if the number is already in the set

        currentMediumProblem = number;
        string problem = MathProblemData.MediumProblems[currentMediumProblem].Problem;
        mediumMathProblemText.text = problem;
        mediumInputField.text = "";
    }

    private void GetNextHardProblem()
    {
        // Ensure we don't exceed the range's capacity
        if (solvedHardProblems.Count >= (Max - Min + 1))
        {
            throw new InvalidOperationException("All numbers in the range have been generated.");
        }

        int number;
        do
        {
            number = random.Next(Min, Max + 1);
        }
        while (!solvedHardProblems.Add(number)); // Try again if the number is already in the set

        currentHardProblem = number;
        string problem = MathProblemData.HardProblems[currentHardProblem].Problem;
        hardMathProblemText.text = problem;
        hardInputField.text = "";
    }

    private async void showMessage(ProblemType type, bool error)
    {
        if (type == ProblemType.Easy)
        {
            easyInputField.text = "";
            if (error)
                easyError.text = "Incorrect. Try Again!!";
            else
                easyError.text = "Perfect!!";
            easyError.gameObject.SetActive(true);
            await Task.Delay(3000);
            easyError.gameObject.SetActive(false);
        }
        else if (type == ProblemType.Medium)
        {
            mediumInputField.text = "";
            if (error)
                mediumError.text = "Incorrect. Try Again!!";
            else
                mediumError.text = "Perfect!!";
            mediumError.gameObject.SetActive(true);
            await Task.Delay(3000);
            mediumError.gameObject.SetActive(false);

        }
        else if (type == ProblemType.Hard)
        {
            hardInputField.text = "";
            if (error)
                hardError.text = "Incorrect. Try Again!!";
            else
                hardError.text = "Perfect!!";
            hardError.gameObject.SetActive(true);
            await Task.Delay(3000);
            hardError.gameObject.SetActive(false);
        }
    }

    public void togglePanel()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            Panel.SetActive(true);
            mathButtonText.text = "OPEN MATH PROBLMS";
        }
        else
        {
            Panel.SetActive(false);
            mathButtonText.text = "CLOSE MATH PROBLMS";
        }
    }
}