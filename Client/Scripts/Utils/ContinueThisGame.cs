using UnityEngine;

public class ContinueThisGame : MonoBehaviour
{
    public static ContinueThisGame Script;

    void Awake()
    {
        Script = GetComponent<ContinueThisGame>();
    }

    public void Continue()
    {
        Game.Script.Continue(name);
    }
}
