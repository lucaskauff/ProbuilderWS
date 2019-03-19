/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Joycon))]
public class JoyconDemo : MonoBehaviour {
	
	private List<Joycon> joycons;

    // Values made available via Unity
    public float[] stick;
    public Vector3 gyro;
    public Vector3 accel;
    public int jc_ind = 0;
    public Quaternion orientation;

	public float minMagnitude;

	// private Rigidbody selfRigidbody;

    void Start ()
    {
		// selfRigidbody = GetComponent<Rigidbody>();
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.joyConList;

		if(joycons == null){
			Debug.LogError("No Joycon List");
		}
		else{
			if (joycons.Count < jc_ind+1){
				Destroy(gameObject);
			}
		}
	}

    // Update is called once per frame
    void Update () {
		if(joycons == null){
			return;
		}

		// make sure the Joycon only gets checked if attached
		if (joycons.Count > 0)
        {
			Joycon j = joycons [jc_ind];


            // Accel values:  x, y, z axis values (in Gs)
            accel = j.GetAccel();
			gyro = j.GetGyro();

			transform.rotation = j.GetVector();

			if(accel.magnitude > 5){
				Debug.DrawRay(transform.position, transform.forward * 5, Color.red, 5);
			}
        }
    }
}*/