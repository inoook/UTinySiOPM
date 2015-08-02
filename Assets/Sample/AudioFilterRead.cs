using UnityEngine;
using System.Collections;
//using System;

public class AudioFilterRead : MonoBehaviour {

	public UTinySiOPM module;

	public GameObject ballPrefab;

	private string[] bshc = new string[]{"6060003305300031","0000620000006001","3110011121110010","0000000000000000"};
	private int[,] tone = new int[,] {{640,3,11,24,-48}, {960,4,3,20,0}, {1280,2,3,32,0}, {1024,4,3,8,0}};
	private int[] pong = new int[]{60,62,64,67,69};
	private int pointer, frame, frameCounter;
	public int bouns;
	
	private int p = 0;

	public int mouseDown = 0;

	// Use this for initialization
	void Start () {
		frame = 3*2;
		frameCounter = 0;
	}

	public Camera cam;

	// Update is called once per frame
	void Update () {
		if (mouseDown > 1) {
			Vector3 screenPos = Input.mousePosition;
			screenPos.z = 10;
			Vector3 pos = cam.ScreenToWorldPoint(screenPos);

			GameObject gObj = (GameObject)GameObject.Instantiate(ballPrefab);
			gObj.transform.position = pos;
			gObj.GetComponent<Ball>().Init(mouseDown-2, this);

			mouseDown = 0;
		}
		if(Input.GetMouseButtonDown(0)){
			mouseDown = 1;
			p = (int)(Random.value*5.0f);
		}
	}


	void OnAudioFilterRead(float[] data, int channels)
	{
		if (++frameCounter == frame) {
			for (int i=0; i<4; i++) {
				int v = int.Parse(bshc[i][pointer].ToString());
				if (v != 0) { 
					//module.noteOn(tone[i, 0], (int)(v << (tone[i, 1])), tone[i, 2], tone[i, 3], tone[i, 4]);
					module.noteOn(tone[i, 0], (int)(v << (tone[i, 1])), 1.0f, tone[i, 2], tone[i, 3], tone[i, 4]);
				}
			}
			if (mouseDown == 1) {
				module.noteOn(pong[p] << 4, 64, 1.0f, 2, 8);
				mouseDown = p+2;
			}

			if (bouns > 0) {
				module.noteOn(pong[bouns>>5]<<4, bouns&31, 1.0f, 2, 8);
				bouns = 0;
			}

			pointer = (pointer + 1) & 15;
			frameCounter = 0;
		}
//
		module.buffer(data, channels);

	}

	string url = "http://soundimpulse.sakura.ne.jp/bound-ball-synthesizer/";
	public string label;
	void OnGUI()
	{

		if( GUILayout.Button("S+: "+url) ){
			Application.OpenURL(url);
		}
		GUILayout.Label(label);
	}
}
