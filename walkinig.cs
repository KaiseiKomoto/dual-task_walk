using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NuitrackSDK;
using NuitrackSDK.Calibration;

public class walkinig : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Rigged model")]
    [SerializeField]
    ModelJoint[] modelJoints;
    [SerializeField]
    nuitrack.JointType rootJoint = nuitrack.JointType.LeftCollar;

    public float speed = 1000.0f;
    Rigidbody rb;

    bool HitFlg;
    //bool Switching;

    public GameObject Omi;

    /// <summary> Model bones </summary>
    Dictionary<nuitrack.JointType, ModelJoint> jointsRigged = new Dictionary<nuitrack.JointType, ModelJoint>();

    void Start()
    {
        for (int i = 0; i < modelJoints.Length; i++)
        {
            modelJoints[i].baseRotOffset = modelJoints[i].bone.rotation;
            jointsRigged.Add(modelJoints[i].jointType.TryGetMirrored(), modelJoints[i]);
        }

        rb = GetComponent<Rigidbody>();
    }

    public void OnCollisionEnter()
    {
        HitFlg = true;
        Debug.Log("Hit");
    }

    public void OnCollisionExit()
    {
        HitFlg = false;
        Debug.Log("Exit");
    }

    public void OnCollisionStay()
    {
        HitFlg = false;
    }

    float Angles = 0;

    public float i = 0.0f;

    void Update()
    {
        if (NuitrackManager.Users.Current != null && NuitrackManager.Users.Current.Skeleton != null)
            ProcessSkeleton(NuitrackManager.Users.Current.Skeleton);

        //transform.rotation = Vector3(0, camera.main.y, 0);

        //if (0 < transform.localEnlurAngles.y <= 1)
        //{
        //transform.Rotate(new Vector3(0, 1f, 0));
        //}

        Vector3 pos = transform.position;
        pos.x = 0.0f;
        pos.y = 0.0f;
        pos.z = i;

        transform.position = pos;

        if (HitFlg)
        {
            //Transform cameraTrans = GameObject.Find("Camera").transform;
            //cameraTrans.Translate(Vector3.forward * 0.5f);
            i += 1.0f;
            transform.position = pos;
        }
        return;
    }

    void FixedUpdate()
    {

    }

    void ProcessSkeleton(UserData.SkeletonData skeleton)
    {
        //Calculate the model position: take the root position and invert movement along the Z axis
        Vector3 rootPos = Quaternion.Euler(0f, 180f, 0f) * skeleton.GetJoint(rootJoint).Position;
        transform.position = rootPos;

        foreach (var riggedJoint in jointsRigged)
        {
            //Get joint from the Nuitrack
            UserData.SkeletonData.Joint joint = skeleton.GetJoint(riggedJoint.Key);

            ModelJoint modelJoint = riggedJoint.Value;

            //Calculate the model bone rotation: take the mirrored joint orientation, add a basic rotation of the model bone, invert movement along the Z axis
            Quaternion jointOrient = Quaternion.Inverse(CalibrationInfo.SensorOrientation) * joint.RotationMirrored * modelJoint.baseRotOffset;

            modelJoint.bone.rotation = jointOrient;
        }
    }
}
