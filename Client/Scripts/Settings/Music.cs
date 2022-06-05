using UnityEngine;
using UnityEngine.UI;

public class Music : MonoBehaviour
{
    public GameObject MusicSlider;

    private void Start()
    {
        RefreshMusicSlider();
    }

    public int Get()
    {
        string Value = Settings.Get("Music");
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
        Settings.Set("Music", Value.ToString());
        RefreshMusicSlider();
    }

    public void Change()
    {
        Set((int)MusicSlider.GetComponent<Slider>().value * 25);
    }

    void RefreshMusicSlider()
    {
        MusicSlider.GetComponent<Slider>().value = (int)(Get() / 25.0f);
    }
}
