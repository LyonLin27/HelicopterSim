using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOnSystem : MonoBehaviour
{
    public float lockOnDist = 500f;
    public int lockOnMax = 4;
    public LayerMask noPlayerLayer;
    private List<RawImage> lockOnIcons;
    private List<Transform> targetsOnScreen;

    // Start is called before the first frame update
    void Start()
    {
        targetsOnScreen = new List<Transform>();
        lockOnIcons = new List<RawImage>();
        foreach (RawImage ri in GetComponentsInChildren<RawImage>()) {
            lockOnIcons.Add(ri);
        }

        StartCoroutine("ScanForTargetOnScreen");
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        for (i = 0; i < targetsOnScreen.Count; i++) {
            if (i >= lockOnIcons.Count) {
                break;
            }
            if (targetsOnScreen[i]) {
                lockOnIcons[i].enabled = true;
                lockOnIcons[i].transform.position = Camera.main.WorldToScreenPoint(targetsOnScreen[i].transform.position);
                Vector3 temp = lockOnIcons[i].rectTransform.localPosition;
                temp.z = 0f;
                lockOnIcons[i].rectTransform.localPosition = temp;
            }
            else {
                lockOnIcons[i].enabled = false;
            }
        }

        while (i < lockOnIcons.Count-1) {
            lockOnIcons[i].enabled = false;
            i++;
        }

    }

    IEnumerator ScanForTargetOnScreen() {
        while (true) {
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForEndOfFrame();

            targetsOnScreen.Clear();

            if (GameManager.instance) {
                foreach (Transform target in GameManager.instance.enemyList) {
                    if(targetsOnScreen.Count > lockOnMax){
                        break;
                    }
                    if (!target)
                        continue;
                    Vector3 playerPos = GameManager.instance.player.transform.position;
                    if(Vector3.Distance(target.position, playerPos) > 500f){
                        continue;
                    }
                    RaycastHit hit;
                    Vector3 dir = (target.position - playerPos).normalized;
                    Physics.Raycast(playerPos, dir, out hit, 500, noPlayerLayer);
                    if(hit.collider && hit.collider.transform != target){
                        continue;
                    }
                
                    Vector3 relativePos = Camera.main.WorldToViewportPoint(target.transform.position);
                    if (relativePos.z > 0f && relativePos.x > 0f && relativePos.x < 1f && relativePos.y > 0f && relativePos.y < 1) {
                        targetsOnScreen.Add(target);
                    }
                }
            }
        }
    }

    public Transform GetLockedTarget(int index){
        if(targetsOnScreen.Count ==0){
            return null;
        }
        while(index >= targetsOnScreen.Count){
            index -= targetsOnScreen.Count;
        }
        return targetsOnScreen[index];
    }
}
