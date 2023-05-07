using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PointsPopupDisplay : MonoBehaviour
{
    private PhotonView pv;

    [SerializeField] private GameObject displayParent;
    [SerializeField] private GameObject pointsDisplay;

    [SerializeField] private string pointsPrefix;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void PointsPopup(float points)
    {
        pv.RPC("RPC_PointsPopup", RpcTarget.All, points);
    }

    [PunRPC]
    public void RPC_PointsPopup(float points)
    {
        GameObject pointsPopup = Instantiate(pointsDisplay, RandomPoint(), Quaternion.identity, displayParent.transform);

        TMP_Text pointsText = pointsPopup.GetComponent<TMP_Text>();

        pointsText.text = pointsPrefix + points.ToString();
    }

    private Vector2 RandomPoint()
    {
        Rect rect = displayParent.GetComponent<RectTransform>().rect;
        float xMin = displayParent.transform.position.x - (rect.width / 2);
        float xMax = displayParent.transform.position.x + (rect.width / 2);
        float yMin = displayParent.transform.position.y - (rect.height / 2);
        float yMax = displayParent.transform.position.y + (rect.height / 2);

        Vector2 point = new Vector2(
            Random.Range( xMin, xMax),
            Random.Range( yMin, yMax )
        );
        return point;
    }
}
