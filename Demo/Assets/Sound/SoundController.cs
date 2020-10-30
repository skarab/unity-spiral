using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using UnityEngine.VFX;

public class SoundController : MonoBehaviour
{
    private const float _MidiOffset = -0.0f;
    private const int _BassChannel = 11;

    public AudioSource Music;
    public float Bass;
    public bool MidiBass;
    public bool MidiBassTwo;
    public bool MidiBassThree;
    public float Treble;
    public Volume[] PPVolumes;
    public RawImage Render;
    public AudioClip Clip;
    public TextAsset MidiFile;

    private static SoundController _Instance;
    private float _Timer = 0.0f;
    
    private Kaitai.StandardMidiFile Midi;

    private Dictionary<int, List<float>> _Samples;
    private Dictionary<int, int> _SamplesID;
    private Dictionary<int, bool> _SamplesValue;

    private float _Scale;
    private bool _Playing = false;

    private float[] _Spectrum = new float[256];
        
    public bool Sample(int id)
    {
        int i=0;
        foreach (var v in _SamplesValue.Values)
        {
            if (i==id)
                return v;
            ++i;
        }
        return false;
    }

    public bool SampleChannel(int channel)
    {
        return _SamplesValue[channel];
    }

    public static SoundController Get()
    {
        return _Instance;
    }

    void Awake()
    {
        _Instance = this;

#if !UNITY_EDITOR
        Cursor.visible = false;
#endif   

        Midi = new Kaitai.StandardMidiFile(new Kaitai.KaitaiStream(MidiFile.bytes));
        _Samples = new Dictionary<int, List<float>>();
        _SamplesID = new Dictionary<int, int>();
        _SamplesValue = new Dictionary<int, bool>();

        for (int channel=0 ; channel<256 ; ++channel)
        {
            List<float> timers = new List<float>();

            foreach(var track in Midi.Tracks)
            {
                int time = 0;
                foreach(var ev in track.Events.Event)
                {
                    if (ev.Channel==channel)
                    {
                        if (ev.EventType==144)
                        {
                            time += ev.VTime.Value;
                            float t = time/(float)(Midi.Hdr.Division*2.0f);
                            timers.Add(t);
                        }

                        if (ev.EventType==128)
                            time += ev.VTime.Value;
                    }
                }
            }

            if (timers.Count>4)
            {
                _Samples[channel] = timers;
                _SamplesID[channel] = 0;
                _SamplesValue[channel] = false;
            }
        }

        _Scale = 300.0f/273.0f; //289.5f/273.0f;
    }

    void Start()
    {
        Render.material.SetFloat("_Scanline", 0.0f);
        Render.material.SetFloat("_Pixelate", 0.0f);
    }

    public void Pixelate()
    {
        Render.material.SetFloat("_Pixelate", 1.0f);
    }

    void Update()
    {    
        if (_Timer>=278.0f || Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (_Timer>=265.0f)
            Render.material.SetFloat("_FadeOut", Mathf.Lerp(Render.material.GetFloat("_FadeOut"), 1.0f, Time.deltaTime*0.6f));

        _Timer += Time.deltaTime;
        if (_Timer>=2.0f && !Music.isPlaying && !_Playing)
        {
            Music.Play();
            _Playing = true;
        }

        AudioListener.GetSpectrumData(_Spectrum, 0, FFTWindow.Rectangular);
        float v = 0.0f;
        for (int i=0 ; i<10 ; ++i)
            v += _Spectrum[i];

        Bass = Mathf.Clamp01(v*0.7f);

        foreach (var k in _Samples.Keys)
            _SamplesValue[k] = false;

        foreach (var k in _Samples.Keys)
        {
            if (_SamplesID[k]<_Samples[k].Count && Music.time*_Scale>=_Samples[k][_SamplesID[k]]+_MidiOffset)
            {
                _SamplesID[k] = _SamplesID[k]+1;
                _SamplesValue[k] = true;
            }
        }

        MidiBass = _SamplesValue[_BassChannel];
        MidiBassTwo = MidiBass && (_SamplesID[_BassChannel]%2==0);
        MidiBassThree = MidiBass && (_SamplesID[_BassChannel]%3==0);

        ChromaticAberration ca;
        
        foreach (Volume pp in PPVolumes)
        {
            pp.sharedProfile.TryGet<ChromaticAberration>(out ca);
            ca.intensity.value = v;        
        }

        v = 0.0f;
        for (int i=128 ; i<256 ; ++i)
            v += _Spectrum[i];

        Treble = Mathf.Clamp01(v);

        Render.material.SetFloat("_Pixelate", Mathf.MoveTowards(Render.material.GetFloat("_Pixelate"), 0.0f, Time.deltaTime));
        Render.material.SetFloat("_Scanline", Mathf.MoveTowards(Render.material.GetFloat("_Scanline"), 0.0f, Time.deltaTime*4.0f));
    
        if (MidiBass)
            Render.material.SetFloat("_Scanline", 1.0f);
    }
}
