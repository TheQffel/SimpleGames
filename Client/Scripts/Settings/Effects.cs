using UnityEngine;
using UnityEngine.UI;

public class Effects : MonoBehaviour
{
    public GameObject EffectsSlider;

    private void Start()
    {
        RefreshEffectsSlider();
    }

    public int Get()
    {
        string Value = Settings.Get("Effects");
        if (Value == null)
        {
            return 100;
        }
        else
        {
            return int.Parse(Value);
        }
    }

    public void Set(int Value)
    {
        Settings.Set("Effects", Value.ToString());
        RefreshEffectsSlider();
    }

    public void Change()
    {
        Set((int)EffectsSlider.GetComponent<Slider>().value * 25);
    }

    void RefreshEffectsSlider()
    {
        EffectsSlider.GetComponent<Slider>().value = (int)(Get() / 25.0f);
    }
}
