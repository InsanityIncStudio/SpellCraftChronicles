using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public AudioSource audio;
    public Sprite doorClosedsprite;
    public float levelChangeTimer = 2;

    public bool exited = false;
    // Update is called once per frame
    void Update ()
	{
	    if (exited)
	    {
	        levelChangeTimer -= Time.deltaTime;
	        if (levelChangeTimer <= 0)
	            SceneManager.LoadScene("New Scenes Later", LoadSceneMode.Single);
	    }
	}

    public void OnTriggerEnter2D(Collider2D colider)
    {
        colider.gameObject.GetComponent<PlayerMind>()?.Hpbar.transform.parent.gameObject.SetActive(false);
        colider.gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().sprite = doorClosedsprite;
        exited = true;
        audio.Play();
    }
}
