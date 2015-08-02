using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 rnd = Random.onUnitSphere;
		Color c = new Color(Mathf.Abs(rnd.x), Mathf.Abs(rnd.y), Mathf.Abs(rnd.z), 1.0f);
		this.gameObject.GetComponent<Renderer>().material.color = c;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(this.transform.position.y < -5){
			Destroy(this.gameObject);
		}
	}

	void OnCollisionEnter(Collision collision) {
		bound();
	}

	public void Init(int p, AudioFilterRead af)
	{
		pitch = p;
		audioFRead = af;
	}

	private int pitch = 0;
	private AudioFilterRead audioFRead;

	void bound(float v = 0.9f)
	{
		audioFRead.bouns = (pitch<<5) | (int)(v*32);
	}
}
