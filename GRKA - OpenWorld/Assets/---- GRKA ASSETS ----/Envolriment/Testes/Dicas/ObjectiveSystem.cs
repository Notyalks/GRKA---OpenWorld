using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectiveSystem : MonoBehaviour
{
    public static ObjectiveSystem Instance { get; private set; }

    public GameObject objectivePanel; // Painel onde os objetivos ser�o exibidos
    public GameObject objectiveTemplate; // Template para criar objetivos
    public TextMeshProUGUI missionTitleText; // Texto para exibir o t�tulo das miss�es

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
        // Definir t�tulo inicial da miss�o
        SetMissionTitle("Miss�o Inicial");

        // Exibir objetivos iniciais
        AddObjective("Pressione E para interagir");
        AddObjective("Pressione Espa�o para pular");
    }

    void Update()
    {
        // Verificar se o jogador pressionou a tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Marcar o objetivo como completo
            CompleteObjective("Pressione E para interagir");

            // Realizar a a��o do objetivo
            Interact();
        }

        // Verificar se o jogador pressionou a tecla Espa�o
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Marcar o objetivo como completo
            CompleteObjective("Pressione Espa�o para pular");

            // Realizar a a��o do objetivo
            Jump();
        }
    }

    // Fun��o para definir o t�tulo da miss�o
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

    // Fun��o para simular a intera��o do jogador
    void Interact()
    {
        Debug.Log("A��o de intera��o realizada!");
    }

    // Fun��o para simular o pulo do jogador
    void Jump()
    {
        Debug.Log("A��o de pulo realizada!");
    }

    public void AddObjective(string objectiveDescription)
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

        Objective newObjective = new Objective(objectiveDescription, objectiveTextComponent, checkmarkComponent);
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
            CheckAllObjectivesCompleted();
        }
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

        // Se todos os objetivos est�o completos, tachar o t�tulo
        missionTitleText.text = $"<s><color=#{ColorUtility.ToHtmlStringRGBA(completedColor)}>{missionTitleText.text}</color></s>";
    }

    private class Objective
    {
        public string Description { get; private set; }
        public bool IsCompleted { get; private set; }
        public TextMeshProUGUI TextComponent { get; private set; }
        public Image CheckmarkComponent { get; private set; }

        public Objective(string description, TextMeshProUGUI textComponent, Image checkmarkComponent)
        {
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
