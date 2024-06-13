using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public Material daySkybox;
    public Material eveningSkybox;
    public Material nightSkybox;

    public Color dayAmbientColor;
    public Color eveningAmbientColor;
    public Color nightAmbientColor;

    public Color dayTintColor = Color.white;
    public Color eveningTintColor = Color.white;
    public Color nightTintColor = Color.white;

    public Light sunLight;
    public Light moonLight;

    public float cycleDuration = 120f; // Duração total do ciclo em segundos

    public Text timeText;

    private float cycleTimer;
    private bool isDaytime = true;

    void Start()
    {
        cycleTimer = 0f;
        SetSkybox(daySkybox, dayTintColor);
        RenderSettings.ambientLight = dayAmbientColor;
        DynamicGI.UpdateEnvironment();
        sunLight.gameObject.SetActive(true);
        moonLight.gameObject.SetActive(false);
    }

    void Update()
    {
        cycleTimer += Time.deltaTime;
        float cycleProgress = cycleTimer / cycleDuration;

        int hour = Mathf.FloorToInt(cycleTimer / cycleDuration * 24f);
        if (hour >= 24)
            hour = 0;

        if (hour >= 7 && hour < 18)
        {
            // Dia
            SetSkybox(daySkybox, dayTintColor);
            RenderSettings.ambientLight = dayAmbientColor;
            sunLight.gameObject.SetActive(true);
            moonLight.gameObject.SetActive(false);
            isDaytime = true;
        }
        else if (hour >= 18 && hour < 20)
        {
            // Fim de Tarde
            float t = (hour - 18f) / 2f;
            SetSkyboxTint(daySkybox, eveningSkybox, dayTintColor, eveningTintColor, t);
            RenderSettings.ambientLight = Color.Lerp(dayAmbientColor, eveningAmbientColor, t);
            sunLight.intensity = Mathf.Lerp(1f, 0f, t);
            moonLight.intensity = Mathf.Lerp(0f, 1f, t);
            isDaytime = false;
        }
        else
        {
            // Noite
            float t = (hour >= 20) ? (hour - 20f) / 4f : (hour + 4f) / 4f;
            SetSkyboxTint(eveningSkybox, nightSkybox, eveningTintColor, nightTintColor, t);
            RenderSettings.ambientLight = Color.Lerp(eveningAmbientColor, nightAmbientColor, t);
            sunLight.gameObject.SetActive(false);
            moonLight.gameObject.SetActive(true);
            isDaytime = false;
        }

        // Atualiza o shader da skybox para aplicar a interpolação
        DynamicGI.UpdateEnvironment();

        // Mostra o horário em um formato de 24 horas na UI
        ShowTime();
    }

    private void ShowTime()
    {
        int hour = Mathf.FloorToInt(cycleTimer / cycleDuration * 24f);

        if (hour >= 24)
            hour = 0;

        string hourString = hour.ToString().PadLeft(2, '0');

        string timeString = hourString + ":00";

        timeText.text = timeString;
    }

    private void SetSkybox(Material skybox, Color tintColor)
    {
        RenderSettings.skybox = skybox;
        skybox.SetColor("_Tint", tintColor);
    }

    private void SetSkyboxTint(Material fromSkybox, Material toSkybox, Color fromTintColor, Color toTintColor, float t)
    {
        Color fromColor = Color.Lerp(fromTintColor, Color.black, t);
        Color toColor = Color.Lerp(Color.black, toTintColor, t);

        fromSkybox.SetColor("_Tint", fromColor);
        toSkybox.SetColor("_Tint", toColor);

        RenderSettings.skybox = t < 0.5f ? fromSkybox : toSkybox;
    }
}
