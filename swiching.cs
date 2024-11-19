using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class swiching : MonoBehaviour
{
    public Vector3 targetPosition; // 移動先の座標を指定します

    void Update()
    {
        // オブジェクトの現在の座標と目標座標の距離を計算します
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 距離が一定以下の場合、新しいシーンに移動します
        if (distanceToTarget < 0.1f)
        {
            // 別のシーンに移動します
            SceneManager.LoadScene("GameModeA");
        }
    }
}
