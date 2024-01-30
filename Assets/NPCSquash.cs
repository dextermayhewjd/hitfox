using System.Collections;
using UnityEngine;
using Photon.Pun;

public class NPCSquash : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isSquashed;
    public NPC_Woodcutter woodcutter;
    public float originalSpeed;

    private void Start()
    {
        originalScale = transform.localScale;
        isSquashed = false;
        originalSpeed = woodcutter.speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("lumb coll");
        if (collision.gameObject.CompareTag("Forklift") && collision.gameObject.GetComponent<NewCarUserControl>().driving && !isSquashed)
        {
            if (PhotonNetwork.IsMasterClient) {
                Debug.Log("start squashing ");
                GameObject objectives = GameObject.Find("Timer+point");
                Debug.Log("5 points for squashing");
                objectives.GetComponent<Timer>().IncreaseScore(5);
                GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
                if (pointsDisplay != null)
                {
                    pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(5);
                }
            }
            // The NPC has been hit by the forklift and is not already squashed
            StartCoroutine(SquashAndRestore());
        }
    }

    IEnumerator SquashAndRestore()
    {
        // Squash the NPC and set isSquashed to true
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.2f, transform.localScale.z);
        isSquashed = true;
        this.GetComponent<CapsuleCollider>().enabled = false;
        woodcutter.agent.speed *= 0.33f;
        // Wait for n seconds
        yield return new WaitForSeconds(5f);

        // Restore the NPC to its original size and set isSquashed to false
        this.GetComponent<CapsuleCollider>().enabled = true;
        transform.localScale = originalScale;
        woodcutter.agent.speed = originalSpeed;
        isSquashed = false;
    }
}


