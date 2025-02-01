using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public enum WeatherType { Clear, Storm, Fog }
    public WeatherType currentWeather;

    public void ChangeWeather()
    {
        currentWeather = (WeatherType)Random.Range(0, 3);
        ApplyWeatherEffects();
    }

    void ApplyWeatherEffects()
    {
        switch(currentWeather)
        {
            case WeatherType.Storm:
                // 이동 속도 30% 감소
                break;
            case WeatherType.Fog:
                // 시야 거리 제한
                break;
        }
    }
}
