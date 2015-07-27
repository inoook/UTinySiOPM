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

public class UTinySiOPM : MonoBehaviour{

	private float[] _output;
	private int[] _zero;
	private int[] _pipe;
	private int[] _pitchTable = new int[2048];
	private int[] _logTable = new int[6144];
	private float[] _panTable = new float[129];

	private int _callbackFrams;

	public int _bufferSize;
	//_onSoundFrame:Function;


	void Awake()
	{
		Init(2048, 2048);
	}

	private void Init(int bufferSize=2048, int callbackFrams=1024) {
		int i, j;
		float p, v;
		int[] t;
		int[] ft = new int[]{0,1,2,3,4,5,6,7,7,6,5,4,3,2,1,0};
		for (i=0, p=0; i<192; i++, p+=0.00520833333f)                            // create pitchTable[128*16]
			for(v=Mathf.Pow(2, p)*12441.464342886f, j=i; j<2048; v*=2, j+=192) _pitchTable[j] = (int)(v);
		for (i=0; i<32; i++) _pitchTable[i] = (i+1)<<6;                         // [0:31] for white noize
		for (i=0, p=0.0078125f; i<256; i+=2, p+=0.0078125f)                       // create logTable[12*256*2]
			for(v=Mathf.Pow(2, 13-p), j=i; j<3328; v*=0.5f, j+=256) _logTable[j+1] = -(_logTable[j]=(int)(v));
		for (i=3328; i<6144; i++) _logTable[i] = 0;                             // [3328:6144] is 0-fill area
		for (i=0, p=0; i<129; i++, p+=0.01217671571f) _panTable[i]=Mathf.Sin(p)*0.5f;  // pan table;
		for (t=Osc.createTable(10), i=0, p=0; i<1024; i++, p+=0.00613592315f) t[i] = log(Mathf.Sin(p)); // sin=0
		for (t=Osc.createTable(10), i=0, p=0.75f; i<1024; i++, p-=0.00146484375f) t[i] = log(p);        // saw=1
		for (t=Osc.createTable(5),  i=0; i<16; i++) t[i+16] = (t[i] = log(ft[i]*0.0625f)) + 1;         // famtri=2
		for (t=Osc.createTable(15), i=0; i<32768; i++) t[i] = log(Random.value-0.5f);                 // wnoize=3
		for (i=0; i<8; i++) for (t=Osc.createTable(4), j=0; j<16; j++) t[j] = (j<=i) ? 192 : 193;     // pulse=4-11
		_zero = new int[bufferSize];                             // allocate zero buffer
		_pipe = new int[bufferSize];                             // allocate fm pipe buffer
		_output = new float[bufferSize*2];                      // allocate stereo out
		_bufferSize = bufferSize;
		_callbackFrams = callbackFrams;

		//_onSoundFrame = onSoundFrame;                                           // set parameters
		for (i=0; i<bufferSize; i++) { _pipe[i]=_zero[i]=0; }                   // clear buffers
	}

	// calculate index of logTable
	static public int log(float n) {
		return (n<0) ? ((n<-0.00390625f) ? ((((int)(Mathf.Log(-n) * -184.66496523f + 0.5f) + 1) << 1) + 1) : 2047)
					 : ((n> 0.00390625f) ? (( (int)(Mathf.Log( n) * -184.66496523f + 0.5f) + 1) << 1)      : 2046);
	}

	// reset all oscillators
	public void reset() {
		for (Osc o = Osc._tm.n; o != Osc._tm; o = o.inactivate().n) { o.fl = Osc._fl; }
	}

	// Returns stereo output as Vector.<Number>(bufferSize*2).
	public float[] render()  {
		int i, j, ph, dph, mod, sh, tl, lout, v, imax; 
		Osc osc, tm;
		float l, r;
		int[] wv, fm, _base;
		int[] _out =_pipe, lt=_logTable;
		float[] stereoOut = _output;
		imax = _bufferSize<<1;
		for (i=0; i<imax; i++) stereoOut[i] = 0;
		for (imax=_callbackFrams; imax<=_bufferSize; imax+=_callbackFrams) {
//			if (_onSoundFrame!=null) {
//				_onSoundFrame();
//			}
			tm = Osc._tm;
			for (osc=tm.n; osc!=tm; osc=osc.update()) {
				dph=_pitchTable[osc.pt];
				ph=osc.ph;
				mod=osc.mod+10; 
				sh=osc.sh; 
				tl=osc.tl; 
				wv=osc.wv;
				fm = (osc.mod==0) ? _zero:_pipe;
				_base = (osc._out!=2) ? _zero:_pipe;
				for (i = imax-_callbackFrams; i < imax; i++) {
					v = ((ph + (fm[i] << mod))& 0x3ffffff) >> sh;
					lout = wv[v] + tl;
					_out[i] = lt[lout] + _base[i];
					ph = (ph + dph) & 0x3ffffff;
				}
				osc.ph = ph;
				if (osc._out==0) {
					l = _panTable[64-osc.pan] * 0.0001220703125f;
					r = _panTable[64+osc.pan] * 0.0001220703125f;
					for (i=imax-_callbackFrams, j=i*2; i<imax; i++) {
						stereoOut[j] += _out[i]*l; j++;
						stereoOut[j] += _out[i]*r; j++;
					}
				}
			}
		}
		return stereoOut;
	}

	// note on
	public Osc noteOn(int pitch, int length=0, float vol=0.5f, int wave=0, int decay=6, int sweep=0, int pan=0) {
		Osc osc = Osc.alloc().reset();
		osc.pt = pitch;
		osc.len = length;
		osc.tl = log(vol);
		osc.ws = wave;
		osc.dr = decay<<2;
		osc.sw = sweep; 
		osc.pan = pan;
		return osc.activate(true);
	}

	public void buffer(float[] data, int channels)
	{
		float[] pipe = render();
		
		for (int i=0; i < _bufferSize; i++) { 
			float p = pipe[i];
			data[i] = p;
		}
	}
}

public class Osc {

	// create new wave table and you can refer the table by '@' command.
	static public int[] createTable(int b) {
		_w.Add(new int[1<<b]); _s.Add(26-b);
		return _w[_w.Count-1];
	}
	static public List<int[]> _w= new List<int[]>();
	static public List<int> _s= new List<int>();
	static public Osc _fl=new Osc();
	static public Osc _tm=new Osc();
	static public Osc alloc(){ if(_fl.p==_fl)return new Osc(); Osc r=_fl.p;_fl.p=r.p;r.p.n=_fl;return r; }
	public Osc into(Osc x){ p=x.p;n=x;p.n=this;n.p=this;return this; }
	public Osc p, n, fl;
	public int pt, len, ph;
	public int tl, sw, dr;
	public int[] wv;
	public int sh, mod, _out, pan;

	public int ws{ 
		set {
			wv=_w[value];
			sh=_s[value];
		}
	}
	public Osc() { p = n = this; }
	public Osc update() { tl+=dr; pt+=sw; pt&=2047; return (--len==0||tl>3328) ? (inactivate().n) : n; }
	public Osc reset() { ph=0; pt=0; len=0; tl=3328; sw=0; dr=24; pan=0; ws=0; mod=0; _out=0; return this; }
	public Osc activate(bool autoFree=false) { into(_tm); fl=(autoFree)?_fl:null; return this; }
	public Osc inactivate() { 
		tl=3328;
		//if(!fl)return this;
		if(fl != null) { return this; }

		Osc r=p; p.n=n; n.p=p; into(fl);
		return r;
	}
	public bool isActive() { return (tl<3328); }
}





