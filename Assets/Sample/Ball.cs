using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
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
