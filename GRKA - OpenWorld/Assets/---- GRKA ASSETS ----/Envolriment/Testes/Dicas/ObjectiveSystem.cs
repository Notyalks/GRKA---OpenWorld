using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectiveSystem : MonoBehaviour
{
    public static ObjectiveSystem Instance { get; private set; }

    public GameObject objectivePanel; // Painel onde os objetivos serão exibidos
    public GameObject objectiveTemplate; // Template para criar objetivos
    public TextMeshProUGUI missionTitleText; // Texto para exibir o título das missões

    public Color completedColor = Color.gray; // Cor do texto tachado

    private List<Objective> objectives = new List<Objective>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // Função para definir o título da missão
    public void SetMissionTitle(string title)
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = title;
        }
        else
        {
            Debug.LogError("MissionTitleText is not set in the inspector.");
        }
    }

    public void AddObjective(string objectiveTitle, string objectiveDescription)
    {
        if (objectiveTemplate == null || objectivePanel == null)
        {
            Debug.LogError("ObjectiveTemplate or ObjectivePanel is not set in the inspector.");
            return;
        }

        GameObject newObjectiveObject = Instantiate(objectiveTemplate, objectivePanel.transform);
        newObjectiveObject.SetActive(true);

        TextMeshProUGUI objectiveTextComponent = newObjectiveObject.transform.Find("ObjectiveText").GetComponent<TextMeshProUGUI>();
        Image checkmarkComponent = newObjectiveObject.transform.Find("Checkmark").GetComponent<Image>();

        if (objectiveTextComponent == null || checkmarkComponent == null)
        {
            Debug.LogError("ObjectiveText or Checkmark component not found in ObjectiveTemplate.");
            return;
        }

        Objective newObjective = new Objective(objectiveTitle, objectiveDescription, objectiveTextComponent, checkmarkComponent);
        objectives.Add(newObjective);
        UpdateObjectiveText(newObjective);
    }

    public void CompleteObjective(string objectiveDescription)
    {
        Objective obj = objectives.Find(o => o.Description == objectiveDescription);
        if (obj != null)
        {
            obj.Complete();
            UpdateObjectiveText(obj);

            CheckAllObjectivesCompleted(); // Adiciona chamada ao CheckAllObjectivesCompleted

            // Notificar o TutorialManager
            FindObjectOfType<TutorialManager>()?.OnObjectiveCompleted(objectiveDescription);
        }
    }

    public void ClearObjectives()
    {
        foreach (var obj in objectives)
        {
            Destroy(obj.TextComponent.transform.parent.gameObject); // Destroi o GameObject do objetivo
        }
        objectives.Clear();
    }

    private void UpdateObjectiveText(Objective obj)
    {
        if (obj.IsCompleted)
        {
            obj.TextComponent.text = $"<s><color=#{ColorUtility.ToHtmlStringRGBA(completedColor)}>{obj.Description}</color></s>";
            obj.CheckmarkComponent.enabled = true;
        }
        else
        {
            obj.TextComponent.text = obj.Description;
            obj.CheckmarkComponent.enabled = false;
        }
    }

    private void CheckAllObjectivesCompleted()
    {
        foreach (var obj in objectives)
        {
            if (!obj.IsCompleted)
            {
                return;
            }
        }

        // Se todos os objetivos estão completos, tachar o título
        missionTitleText.text = $"<s><color=#{ColorUtility.ToHtmlStringRGBA(completedColor)}>{missionTitleText.text}</color></s>";
    }

    public bool HasObjective(string objectiveTitle)
    {
        return objectives.Exists(o => o.Title == objectiveTitle);
    }

    private class Objective
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsCompleted { get; private set; }
        public TextMeshProUGUI TextComponent { get; private set; }
        public Image CheckmarkComponent { get; private set; }

        public Objective(string title, string description, TextMeshProUGUI textComponent, Image checkmarkComponent)
        {
            Title = title;
            Description = description;
            IsCompleted = false;
            TextComponent = textComponent;
            CheckmarkComponent = checkmarkComponent;
        }

        public void Complete()
        {
            IsCompleted = true;
        }
    }
}
