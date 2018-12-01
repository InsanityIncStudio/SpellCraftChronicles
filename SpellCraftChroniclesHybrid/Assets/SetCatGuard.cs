using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetCatGuard : MonoBehaviour
{
    public CharacterStateMachine bossState;
    public PrimalMind bossMind;
    public bool triggered;
    public float levelChangeTimer = 10;
    public Text text;
    public GameObject soundSources;
    public void Update()
    {
        if (triggered)
        {
            if (bossState.dead)
            {
                text.text = ((int) levelChangeTimer).ToString();
                levelChangeTimer -= Time.deltaTime;
                if(levelChangeTimer<=0)
                    SceneManager.LoadScene(1, LoadSceneMode.Single);
                soundSources.GetComponents<AudioSource>().ToList().ForEach(e => e.Stop());
            }
        }
    }
}
