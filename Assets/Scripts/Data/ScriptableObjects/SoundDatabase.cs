using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Data/SoundDataBase")]
public class SoundDatabase : ScriptableObject
{
    [SerializeField]
    [Tooltip("‰¹Œ¹‚ÌƒŠƒXƒg")]
    private List<SoundData> _soundDataList;

    public List<SoundData> SoundDataList => _soundDataList;
}


[Serializable]
public class SoundData
{
    [SerializeField] private AudioClip _clip;
    public AudioClip Clip => _clip;
    [SerializeField, Range(0f, 1f)] private float _volume = 0.5f;
    public float Volume => _volume;
    [SerializeField] private bool _isLoop = false;
    public bool IsLoop => _isLoop;
}
