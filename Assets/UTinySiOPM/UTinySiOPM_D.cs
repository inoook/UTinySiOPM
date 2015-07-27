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

public class UTinySiOPM_D : MonoBehaviour{

	private double[] _output;
	private int[] _zero;
	private int[] _pipe;
	private int[] _pitchTable = new int[2048];
	private int[] _logTable = new int[6144];
	private double[] _panTable = new double[129];

	private int _callbackFrams;

	public int _bufferSize;
	//_onSoundFrame:Function;


	void Awake()
	{
		Init(2048, 2048);
	}

	private void Init(int bufferSize=2048, int callbackFrams=1024) {
		int i, j;
		double p, v;
		int[] t;
		int[] ft = new int[]{0,1,2,3,4,5,6,7,7,6,5,4,3,2,1,0};
		for (i=0, p=0; i<192; i++, p+=0.00520833333)                            // create pitchTable[128*16]
			for(v=System.Math.Pow(2, p)*12441.464342886, j=i; j<2048; v*=2, j+=192) _pitchTable[j] = (int)(v);
		for (i=0; i<32; i++) _pitchTable[i] = (i+1)<<6;                         // [0:31] for white noize
		for (i=0, p=0.0078125; i<256; i+=2, p+=0.0078125)                       // create logTable[12*256*2]
			for(v=System.Math.Pow(2, 13-p), j=i; j<3328; v*=0.5, j+=256) _logTable[j+1] = -(_logTable[j]=(int)(v));
		for (i=3328; i<6144; i++) _logTable[i] = 0;                             // [3328:6144] is 0-fill area
		for (i=0, p=0; i<129; i++, p+=0.01217671571) _panTable[i]=System.Math.Sin(p)*0.5;  // pan table;
		for (t=Osc.createTable(10), i=0, p=0; i<1024; i++, p+=0.00613592315) t[i] = log(System.Math.Sin(p)); // sin=0
		for (t=Osc.createTable(10), i=0, p=0.75f; i<1024; i++, p-=0.00146484375) t[i] = log(p);        // saw=1
		for (t=Osc.createTable(5),  i=0; i<16; i++) t[i+16] = (t[i] = log(ft[i]*0.0625)) + 1;         // famtri=2
		for (t=Osc.createTable(15), i=0; i<32768; i++) t[i] = log(Random.value-0.5);                 // wnoize=3
		for (i=0; i<8; i++) for (t=Osc.createTable(4), j=0; j<16; j++) t[j] = (j<=i) ? 192 : 193;     // pulse=4-11
		_zero = new int[bufferSize];                             // allocate zero buffer
		_pipe = new int[bufferSize];                             // allocate fm pipe buffer
		_output = new double[bufferSize*2];                      // allocate stereo out
		_bufferSize = bufferSize;
		_callbackFrams = callbackFrams;

		//_onSoundFrame = onSoundFrame;                                           // set parameters
		for (i=0; i<bufferSize; i++) { _pipe[i]=_zero[i]=0; }                   // clear buffers
	}

	// calculate index of logTable
	static public int log(double n) {
		return (n<0) ? ((n<-0.00390625) ? ((((int)(System.Math.Log(-n) * -184.66496523 + 0.5) + 1) << 1) + 1) : 2047)
					 : ((n> 0.00390625) ? (( (int)(System.Math.Log( n) * -184.66496523 + 0.5) + 1) << 1)      : 2046);
	}

	// reset all oscillators
	public void reset() {
		for (Osc o = Osc._tm.n; o != Osc._tm; o = o.inactivate().n) { o.fl = Osc._fl; }
	}

	// Returns stereo output as Vector.<Number>(bufferSize*2).
	public double[] render()  {
		int i, j, ph, dph, mod, sh, tl, lout, v, imax; 
		Osc osc, tm;
		double l, r;
		int[] wv, fm, _base;
		int[] _out =_pipe, lt=_logTable;
		double[] stereoOut = _output;
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
					l = _panTable[64-osc.pan] * 0.0001220703125;
					r = _panTable[64+osc.pan] * 0.0001220703125;
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
	public Osc noteOn(int pitch, int length=0, double vol=0.5, int wave=0, int decay=6, int sweep=0, int pan=0) {
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
		double[] pipe = render();
		
		for (int i=0; i < _bufferSize; i++) { 
			float p = (float)pipe[i];
			data[i] = p;
		}
	}
}






