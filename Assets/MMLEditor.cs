using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// http://wonderfl.net/c/moSo

public class MMLEditor : MonoBehaviour {

	public string mml = "";
	public UTinySiOPM _module;

	Sequencer _sequencer;

	// Use this for initialization
	void Start () {
		
		mml+="#BS=r28$s10l4[3e8[22e]er<s2c12>s10brg]s1a36g12f+16s12e8[5e]l2egabl4ers2e20\n";
		mml+="s10[[3e8[22e]er<s2c12>s10brg]e8[15e]gabf+8gae8eeegab\n";
		mml+="[[<c8[6c]>b8[6b]a8[6a]|g8ggggab]|a8aaaabr]a8[6a]<d8[14d]>]\n";
		mml+="erer20[8[erer20|erer20]|er28]erer20\n";
		mml+="[[[<c8[6c]>b8[6b]a8[6a]|g8ggggab]|a8aaaabr]a8[6a]]<d8[14d]>;\n";
		mml+="@0@o1o2BS; v7@0@i3o4BS;\n";
		mml+="\n";
		mml+="#BD=r8l4v14s32cc2<v5e2c2>g2v14crl4c$[rcrr[15ccrr]] [[rcrr[15ccrr]]\n";
		mml+="[rcrr[15ccrr]] [2ccrr]cl2<v6ggggeeeecccc>l4v14c]\n";
		mml+="rcr20[6[crcr20|crcr20]|cr28][8crcrrccr]crcrrcccr\n";
		mml+="[4rcrr[15ccrr]] [2ccrr]cl2<v6ggggeeeecccc>l4v14c;\n";
		mml+="#SN=r8l4v14s12k8cc2k4s32c2c2c6k8s12l8c$k8[14rc]l4ck4ccrk8ccl8c[7rc]r4c4c[5rc]rl2k4ccc8r4k8c8c4c4r8\n";
		mml+="l8[[[15rc]r4c4c] [k8[13rc]|r4c2c2c4k4l4crccrccl8r] rc[rcr4c4c]c4k4l2s32[3c1c1ccc]r4l8k8s12]\n";
		mml+="l16[[2rc]|rc4s24k4c4c8s12k8rc4c12]rc4c12k4s32l2[ccc4]k8s12c4c4c8 l8[6[7rc]r4c4c]r16c4c4c8\n";
		mml+="[4k8[13rc]|r4c2c2c4k4l4crccrccl8r] rc[rcr4c4c]c4k4l2s32[3c1c1ccc]r4l8k8s12;\n";
		mml+="#CY=r8l8s4v8c+20$l8[3s6p3v16d12s10p5v6[14d]d4]s6v16d12p5v6[3d]p2v12d12p5d16p3v16d44p4d20\n";
		mml+="[l4[4p5s6v16d12s24p6v6[29d]]l8p5[4s6v16d16s10v8[14d]][4d]p3s4v12c+32]\n";
		mml+="s64v3l2[512d]v12s6c+8v15c+2r22\n";
		mml+="l8p5[8s6v16d16s10v8[14d]][4d]p3s4v12c+32;\n";
		mml+="@0w32o4BD; @3o0SN; @3o0CY;\n";
		mml+="#GT=r28$l4[3[3[s2e8s12e]ee]ers2b2<c10>brg]s1a36g12f+16s2e40re20\n";
		mml+="[l4[3[3[s2e8s12e]ee]ers2b2<c10>brg][[s2e8s12e]ee]es1g16f+12e20s12eer\n";
		mml+="s1l8[<c24c>b24b<d24d>g24g<c24c>b24b|a56b]a64<d64>]\n";
		mml+="r256s24l4[192e]s12erer20\n";
		mml+="s1l8[[<c24c>b24b<d24d>g24g<c24c>b24b|a56b]a64]<d64>;\n";
		mml+="@2@o1o6    GT; @0@i3@o1o4k112GT; p1v5@1@i4o3    GT;\n";
		mml+="@2@o1o6k112GT; @0@i3@o1o4k224GT; p7v4@1@i4o3k112GT;\n";
		mml+="\n";
		mml+="#S1=v5r28$o6s6l4e8[4e|r36drdrer]r28[3dr]e8[er36drdr|er]s1c36>b12a12r<erer32d20\n";
		mml+="s2[[[e32d32|d24crc20d8r]rd28dr|c12>b<rd]g12f+rd\n";
		mml+="[[er20erdr20dr|dr20drdr12d16]|d24drd24d8]d64d56dr]\n";
		mml+="v7drdrv1drdr[8[v6drdrv1dv6c+12|v6drdrv1drdr]|drrrv1drrr]drdr20\n";
		mml+="[[[er20erdr20dr|dr20drdr12d16]|d24drd24d8]d64]d56dr;\n";
		mml+="#S2=v6r28$o5s6l4g8[4g|r36f+rf+rgr]r28[3f+r]g8[gr36f+rf+r|gr]s1e36e12dr12erer32f+20\n";
		mml+="s2[[[g32f+32|f+24f+re20f+8r]rf+28f+r|e12drf+]b12brf+\n";
		mml+="[[gr20grgr20gr|f+r20f+rgr12fgb8]|g24grf+24f+8]g64f+56f+r]\n";
		mml+="v8grgrv1grgr[8[v6grgrv1gv6g12|v6grgrv1grgr]|grrrv1grrr]grgr20\n";
		mml+="[[[gr20grgr20gr|f+r20f+rgr12fgb8]|g24grf+24f+8]g64]f+56f+r;\n";
		mml+="#S3=v6r28$o5s6l4b8[4b|r36ararbr]r28[3ar]b8[br36arar|br]s1a36g12f+12rbrbr32a20\n";
		mml+="s2>[[[b32a32|a24grg20a8r]ra28ar|g12ara]<<d12d>ra\n";
		mml+="[[cr20cr>br20br|ar20arbr12b16<]|a56a8<]a64a56ar]<\n";
		mml+="v8ererv1erer[8[v6ererv1ev6e12|v6ererv1erer]|errrv1errr]erer20\n";
		mml+="[[[cr20cr>br20br|ar20arbr12b16<]|a56a8<]a64]a56ar<;\n";
		mml+="p6@1S1; p2@1S2; @1S3;\n";
		mml+="\n";
		mml+="#MA=r28$@1s3[l16f+1g19f+d>a12b48l4|f+gargre8r112<]r8<cde36g8el8aea+2b2l4rer32d20\n";
		mml+="@10[s6l4[>[b8bbbbab<c+2d2r>a24|a8aaaagab8<c>ba8eg]r|<f+16g8f+e24r8]<g16f+12e20>\n";
		mml+="s3b<d>b<e20re8d16>brgra24gab16 fgb<de24f+gd16cr>b8a52<ef+r\n";
		mml+="g28f+g8d16grf+12gargrarb16<c+1d3rdr>g16<d+1e7dr>g12<cr>ba64f+64];\n";
		mml+="#MB=r20@2s1w12o9c64s4w-32o4[4c8]r16s2w6o8c64s4w24o5[5c6]r2s1w-12o4c64s0w-2o5c224\n";
		mml+="@10s3w0o6l4[8rf+de>gb<d>f+abegaf+de<];\n";
		mml+="#MB2=r772l4<[4raf+g>b<df+>a<de>ab<d>af+g<]>;\n";
		mml+="#MC=r16>[s3b<d>b<e20re8d16>brgra24gab16 fgb<de24f+gd16cr>b8a52<ef+r\n";
		mml+="g28f+g8d16grf+12gargrarb16<c+1d3rdr>g16<d+1e7dr>g12<cr>b|a52>]a64<d64>;\n";
		mml+="v8o6k-1MAv6MBv8MC; v10o5k1MAv3MB2v10MC; v3p1o6r4MAv4MBv3MC;\n";

		
		_module.OnSoundFrame = onSoundFrame;

		_sequencer = new Sequencer();
		_sequencer.pos = 0;
		_sequencer.speed = 2;
	}
	
	// Update is called once per frame
	void Update () {

	}
	public bool isPlay = false;
	void OnAudioFilterRead(float[] data, int channels)
	{
		if(isPlay){
			_module.buffer(data, channels);
		}
	}
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10,10,800,800));
		List<Track> tracks = _sequencer.GetTracks();
		GUILayout.Label("tracks: "+tracks.Count);
		for(int i = 0; i < tracks.Count; i++){
			GUILayout.Label(i + " : "+tracks[i].cnt);
		}
		if( GUILayout.Button("Play") ){
			Play();
		}
		mml = GUILayout.TextField(mml);

		GUILayout.EndArea();
	}

	void Play()
	{
		_module.reset();
		string[] mmlstrs = _expandMML(mml);
		_sequencer.mml = mmlstrs;

		isPlay = true;
	}

	void onSoundFrame()
	{
		_sequencer.onSoundFrame();
	}

	public string[] _expandMML(string mml) {
		string str = System.Text.RegularExpressions.Regex.Replace(mml, "\\s+", "");
		System.Text.RegularExpressions.Regex splitReqex = new  System.Text.RegularExpressions.Regex(";");
		string[] split = splitReqex.Split(str);

		List<string> list = new List<string>();
		Dictionary<string, string> macro = new Dictionary<string, string>();
		
		string currentKey = "";
		foreach (string seq in split) {
			System.Text.RegularExpressions.Match res = System.Text.RegularExpressions.Regex.Match(seq, "^#([_A-Z][_A-Z0-9]*)=?(.*)");
			if (res.Success) {
				currentKey = res.Groups[1].Value;
				macro[currentKey] = res.Groups[2].Value;
			} else {
				System.Text.RegularExpressions.Match resR = System.Text.RegularExpressions.Regex.Match(seq, "[_A-Z][_A-Z0-9]*");
				string s = "";
				if (resR.Success) {
					s = System.Text.RegularExpressions.Regex.Replace(seq, "[_A-Z][_A-Z0-9]*", GetReplaceMent(resR.Groups[0].Value, macro));
				}
				list.Add(s);
			}
		}
		return list.ToArray();
	}

	string GetReplaceMent(string key, Dictionary<string, string> macro)
	{
		if(!macro.ContainsKey(key)){ return "null"; }
		return macro[key];
	}

}
