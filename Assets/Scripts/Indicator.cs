using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public float height = 100f;
    public float bobModifier = 20f;
    public float bobTime = 1.0f;
    private bool goingUp = true;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        float scale = (1.5f/ (transform.parent.localScale.x / 4));
        float dist2Player = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);
        scale *= dist2Player / 1000f;
        scale = Mathf.Clamp(scale - 1f, 0f, 5f);
        transform.localScale = new Vector3(scale, scale, scale);
        transform.LookAt(Camera.main.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, -90);
        if(goingUp)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, height, transform.position.z), new Vector3(transform.position.x, height + bobModifier, transform.position.z), timer / bobTime);
            if(timer / bobTime >= 1.0f)
            {
                goingUp = false;
                timer = 0;
            }
        }
        else
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, height + bobModifier, transform.position.z), new Vector3(transform.position.x, height, transform.position.z), timer / bobTime);
            if (timer / bobTime >= 1.0f)
            {
                goingUp = true;
                timer = 0;
            }
        }
    }
}
