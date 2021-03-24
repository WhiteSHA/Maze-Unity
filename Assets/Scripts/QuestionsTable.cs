using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<QuizEntry> quizEntryList;
    private List<Transform> quizEntryTransformList;

    [SerializeField] Dropdown idsDropdown;
    [SerializeField] InputField questionInput;
    [SerializeField] InputField correctAnswerInput;
    [SerializeField] InputField wrongAnswerInput;
    [SerializeField] InputField wrongAnswer2Input;

    private void Awake()
    {
        entryContainer = transform.Find("AllQuizzesContainer");

        if (entryContainer == null)
            return;

        entryTemplate = entryContainer.Find("EntryTemplate");

        if (entryTemplate == null)
            return;

        entryTemplate.gameObject.SetActive(false);

        // Get data from Player Prefs
        Quizes quizes = new Quizes();
        for (int i = 0; i < 20; i++)
        {
            Debug.Log(PlayerPrefs.GetString("QuizzesDataTable" + i));
            string jsonOfData = PlayerPrefs.GetString("QuizzesDataTable" + i);
            if (jsonOfData.Length < 1)
                break;

            QuizEntry quizFromDB = JsonUtility.FromJson<QuizEntry>(jsonOfData);
            quizes.quizEntryList.Add(quizFromDB);
        }
        Debug.Log(quizes.ToString());
        //        

        quizEntryList = quizes.quizEntryList;

        quizEntryTransformList = new List<Transform>();
        foreach(QuizEntry quizEntry in quizEntryList)
        {
            CreateQuizeEntryTransform(quizEntry, entryContainer, quizEntryTransformList);
        }

        idsDropdown.options.Clear();
        for (int i = 0; i < quizEntryList.Count + 1; ++i)
        {
            Dropdown.OptionData data = new Dropdown.OptionData { text = (i + 1).ToString() };
            idsDropdown.options.Add(data);
        }
    }

    private void CreateQuizeEntryTransform(QuizEntry quizEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 30f;

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count + 12.5f);
        entryTransform.gameObject.SetActive(true);

        entryTransform.Find("id").GetComponent<Text>().text = quizEntry.id.ToString();
        entryTransform.Find("Question").GetComponent<Text>().text = quizEntry.question;
        entryTransform.Find("CorrectAnswer").GetComponent<Text>().text = quizEntry.correctAnswer;
        entryTransform.Find("WrongAnswer1").GetComponent<Text>().text = quizEntry.wrongAnswer;
        entryTransform.Find("WrongAnswer2").GetComponent<Text>().text = quizEntry.wrongAnswer2;

        entryTransform.gameObject.name = "quiz" + transformList.Count + 1;

        transformList.Add(entryTransform);
    }

    public void AddOrInsertData(int id, string question, string correctAnswer, string wrongAnswer, string wrongAnswer2)
    {
        if (id > quizEntryTransformList.Count + 1)
            AddQuizEntry(id, question, correctAnswer, wrongAnswer, wrongAnswer2);
        else
            EditQuizEntry(id, question, correctAnswer, wrongAnswer, wrongAnswer2);
    }

    private void AddQuizEntry(int id, string question, string correctAnswer, string wrongAnswer, string wrongAnswer2)
    {
        QuizEntry newEntry = new QuizEntry
        {
            id = id, question = question, correctAnswer = correctAnswer, wrongAnswer = wrongAnswer, wrongAnswer2 = wrongAnswer2
        };

        CreateQuizeEntryTransform(newEntry, entryContainer, quizEntryTransformList);
    }

    private void EditQuizEntry(int id, string question, string correctAnswer, string wrongAnswer, string wrongAnswer2)
    {
        float templateHeight = 30f;

        Transform entryTransform = quizEntryTransformList[id - 1];
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * (id - 1));
        entryTransform.gameObject.SetActive(true);

        entryTransform.Find("id").GetComponent<Text>().text = id.ToString();
        entryTransform.Find("Question").GetComponent<Text>().text = question;
        entryTransform.Find("CorrectAnswer").GetComponent<Text>().text = correctAnswer;
        entryTransform.Find("WrongAnswer1").GetComponent<Text>().text = wrongAnswer;
        entryTransform.Find("WrongAnswer2").GetComponent<Text>().text = wrongAnswer2;

        quizEntryTransformList.RemoveAt(id - 1);
        quizEntryTransformList.Insert(id - 1, entryTransform);
    }

    public void SaveData()
    {
        Quizes quizes = new Quizes { quizEntryList = quizEntryList };
        for(int i = 0; i < quizes.quizEntryList.Count; ++i)
        {
            string jsonOfData = JsonUtility.ToJson(quizes.quizEntryList[i]);
            PlayerPrefs.DeleteKey("QuizzesDataTable" + i);
            PlayerPrefs.SetString("QuizzesDataTable" + i, jsonOfData);
            PlayerPrefs.Save();
        }
    }

    public void ClearData()
    {
        for (int i = 0; i < 20; ++i)
        {
            PlayerPrefs.DeleteKey("QuizzesDataTable" + i);
            PlayerPrefs.Save();
        }

        if(quizEntryList != null)
            quizEntryList.Clear();
        if (quizEntryTransformList != null)
            quizEntryTransformList.Clear();
    }

    public void ResetData()
    {
        // Get data from Player Prefs
        string jsonOfData;
        Quizes quizes = new Quizes();
        for (int i = 0; i < 20; ++i)
        {
            jsonOfData = PlayerPrefs.GetString("QuizzesDataTable" + i);

            if (jsonOfData.Length < 1)
                break;

            QuizEntry quiz = JsonUtility.FromJson<QuizEntry>(jsonOfData);
            quizes.quizEntryList.Insert(i, quiz);
        }

        for(int i = 0; i < quizEntryTransformList.Count; ++i)
        {
            Destroy(quizEntryTransformList[i].gameObject);
        }
        quizEntryTransformList.Clear();
        quizEntryList.Clear();

        quizEntryList = quizes.quizEntryList;

        quizEntryTransformList = new List<Transform>();
        foreach(QuizEntry quizEntry in quizEntryList)
        {
            CreateQuizeEntryTransform(quizEntry, entryContainer, quizEntryTransformList);
        }
    }

    // New quiz functions
    public void CleanQuizInputs()
    {
        idsDropdown.value = 0;
        questionInput.text = "";
        correctAnswerInput.text = "";
        wrongAnswerInput.text = "";
        wrongAnswer2Input.text = "";
    }

    public void AddNewQuiz()
    {
        QuizEntry newQuiz = new QuizEntry
        {
            id = idsDropdown.value + 1,
            question = questionInput.text,
            correctAnswer = correctAnswerInput.text,
            wrongAnswer = wrongAnswerInput.text,
            wrongAnswer2 = wrongAnswer2Input.text
        };

        if (newQuiz.id > quizEntryList.Count)
        {
            quizEntryList.Add(newQuiz);
            CreateQuizeEntryTransform(newQuiz, entryContainer, quizEntryTransformList);

            if (newQuiz.id + 1 < 21)
            {
                Dropdown.OptionData data = new Dropdown.OptionData { text = (newQuiz.id + 1).ToString() };
                idsDropdown.options.Add(data);
            }
        }
        else
        {
            quizEntryList.RemoveAt(newQuiz.id - 1);
            quizEntryList.Insert(newQuiz.id - 1, newQuiz);
            Destroy(quizEntryTransformList[newQuiz.id - 1].gameObject);
            quizEntryTransformList.RemoveAt(newQuiz.id - 1);
            CreateQuizeEntryTransform(newQuiz, entryContainer, quizEntryTransformList);
        }

        idsDropdown.value = 0;
        questionInput.text = "";
        correctAnswerInput.text = "";
        wrongAnswerInput.text = "";
        wrongAnswer2Input.text = "";
    }

    private class Quizes
    {
        public List<QuizEntry> quizEntryList = new List<QuizEntry> { };
    }

    [System.Serializable]
    private class QuizEntry
    {
        public int id = 0;
        public string question = "";
        public string correctAnswer = "";
        public string wrongAnswer = "";
        public string wrongAnswer2 = "";
    }
}
