using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderInitializer : MonoBehaviour
{
    [SerializeField] private string playerPrefKey;

    void Start()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat(playerPrefKey, 1f);
    }
}