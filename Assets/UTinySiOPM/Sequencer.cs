/**
 * Copyright keim_at_Si ( http://wonderfl.net/user/keim_at_Si )
 * MIT License ( http://www.opensource.org/licenses/mit-license.php )
 * Downloaded from: http://wonderfl.net/c/viWw
 * 
 *  as3 -> Unity3d  
 * Copyright dizgid ( https://twitter.com/inok )
 * 
 * http://wonderfl.net/tag/TinySiOPM
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text.RegularExpressions;

// MML Sequencer
public class Sequencer {
	private List<Track> _tracks;
	private int _count = Track.speed+1;

	public Sequencer(int speed = 2, string[] mml=null) {
		this.speed = speed;
		this.mml = mml;
	}
	public bool onSoundFrame() {
		if (++_count == Track.speed) {
			foreach (Track tr in _tracks) tr.execute();
			_count = 0;
			return true;
		}
		return false;
	}
	public int speed {
		set{
			Track.speed = value;
			if (_count >= value) _count = 0;
		}
	}
	public int drSpeed{
		set{
			if (value<0 || value>2) return;
			Track.drs = value;
		}
	}
	public int pos{
		set{
			int p = value;
			for (int i=0; i<p; i++) {
				foreach (Track tr in _tracks) tr.execute();
			}
		}
	}
	public string[] mml {
		set{
			string[] list = value;
			_tracks = new List<Track>();
			if (list != null) {
				foreach (string seq in list) _tracks.Add(new Track(seq));
			}
			_count = 0;
		}
	}

	// add
	public void SetMML(string[] mml)
	{
		this.mml = mml;
	}
	public void SetMML(string mml)
	{
		this.mml = new string[]{ mml };
	}
	public List<Track> GetTracks()
	{
		return _tracks;
	}
}

public class StacObj
{
	public int p;
	public int c;
	public int j;

	public StacObj(int p, int c, int j)
	{
		this.p = p;
		this.c = c;
		this.j = j;
	}
}

public class Track {
	static public int codeA;
	static public int[] nt=new int[]{9,11,0,2,4,5,7};
	static public int speed = 3, drs=2;

	public int oct, len, tl, dt, cnt;
	public string seq;
	public int sgn;
	public List<StacObj> stac;
	public Osc osc;

	private Regex _rex = new Regex( "(@i|@o|[a-gkloprsvw<>[|\\]$@])([#+])?(-?\\d+)?" );
	private int _rex_lastIndex;

	public static int charCodeAt(string str)
	{
		byte[] stringBytes = System.Text.Encoding.Unicode.GetBytes(str);
		return stringBytes[0];
	}

	public Track(string seq) {

		codeA = charCodeAt("a");

		osc = Osc.alloc().reset().activate(false);
		reset(seq);
	}
	public void reset(string seq_) {
		seq=seq_; oct=5; len=4; tl=256; dt=0; cnt=0; sgn=0; 
		_rex_lastIndex=0; 
		stac = new List<StacObj>();
	}
	public void execute() {
		if (--cnt <= 0) {
			for (int i=0; i<100; i++) {
				System.Text.RegularExpressions.Match res = _rex.Match(seq, _rex_lastIndex);
				_rex_lastIndex = res.Index +res.Length;

				if (!res.Success) {
					if (sgn == 1) { 
						_rex_lastIndex = sgn; continue;
					}else{ 
						cnt = int.MaxValue; break;
					}
				}

				int cmd = charCodeAt(res.Groups[1].Value);
				if (cmd>=codeA && cmd<=codeA+6) {
					cnt = (res.Groups[3].Value != "") ? toInt(res.Groups[3].Value) : len;
					osc.len = cnt * speed;
					osc.pt = ((nt[cmd-codeA]+oct*12+((res.Groups[2].Value != "") ? 1 : 0))<<4) + dt;
					osc.tl = tl;
					break;
				} else if (res.Groups[1].Value == "r") {
					cnt = (res.Groups[3].Value != "") ?  toInt(res.Groups[3].Value) : len;
					break;
				} else {
					switch(res.Groups[1].Value){
					case "k": dt  =  toInt(res.Groups[3].Value); break;
					case "l": len = toInt(res.Groups[3].Value); break;
					case "o": oct =  toInt(res.Groups[3].Value); break;
					case "v": tl  = UTinySiOPM.log( toInt(res.Groups[3].Value)*0.0625f); break;
					case "<": oct++; break;
					case ">": oct--; break;
					case "@":  osc.ws =  toInt(res.Groups[3].Value);    break;
					case "s":  osc.dr = ( toInt(res.Groups[3].Value)<<drs)&~1; break;
					case "w":  osc.sw = -( toInt(res.Groups[3].Value)>>(2-drs));   break;
					case "p":  osc.pan = ( toInt(res.Groups[3].Value)<<4)-64; break;
					case "@i": osc.mod =  toInt(res.Groups[3].Value);   break;
					case "@o": osc._out =  toInt(res.Groups[3].Value);   break;
					case "$": sgn = _rex_lastIndex; break;
					case "[": 
						StacObj s = new StacObj( _rex_lastIndex, ((res.Groups[3].Value != "") ?  toInt(res.Groups[3].Value):2), 0 );
						stac.Insert(0, (s));
						break;
					case "|": if (stac[0].c == 1) { _rex_lastIndex = stac[0].j; stac.RemoveAt(0); } break;
					case "]": 
						stac[0].j = _rex_lastIndex;
						if (--stac[0].c == 0) { stac.RemoveAt(0); 
						} else{ 
							_rex_lastIndex = stac[0].p;
						}
						break;
					}
				}
			}
		}
	}

	int toInt(string str)
	{
		int res = 0;
		bool success = int.TryParse(str, out res);
		return success ? res : 0;
	}
}