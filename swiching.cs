using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class swiching : MonoBehaviour
{
    public Vector3 targetPosition; // �ړ���̍��W���w�肵�܂�

    void Update()
    {
        // �I�u�W�F�N�g�̌��݂̍��W�ƖڕW���W�̋������v�Z���܂�
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // ���������ȉ��̏ꍇ�A�V�����V�[���Ɉړ����܂�
        if (distanceToTarget < 0.1f)
        {
            // �ʂ̃V�[���Ɉړ����܂�
            SceneManager.LoadScene("GameModeA");
        }
    }
}
