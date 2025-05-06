using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.MeshOperations;

public class LoopingMusic : MonoBehaviour
{
    public int DeadStart, DeadEnd;
    public int GeneralStart, GeneralEnd;
    public int CombatStart, CombatEnd;
    public int LowHealthStart, LowHealthEnd;


    private int LoopStart,LoopEnd = 1;
    private int Currbeat;
    private int CurrState, OldState;
    public float BPM;
    float bpmfloat;
    float time;
    public AudioSource aud;

    public HealthScript healthscr;
    public MechTest mechscr;



    void Start()
    {
        bpmfloat = (60f / BPM) * 8f;
    }

    // Update is called once per frame
    void Update()
    {   
        if(aud.time >= Currbeat * bpmfloat)
        {  
            Debug.Log("gyat");
            Currbeat += 1;
            if(healthscr.CurrHealth <= 0)
            {
                if(OldState == 3)
                    return;
                
                CurrState = 3;
                LoopStart = DeadStart;
                LoopEnd = DeadEnd; 
                Currbeat = LoopStart +1;
                aud.time = LoopStart * bpmfloat;
            }
            else if(healthscr.CurrHealth < healthscr.MaxHealth / 3)
            {
                if(OldState == 2)
                    return;
                
                CurrState = 2;
                LoopStart = LowHealthStart;
                LoopEnd = LowHealthEnd;
                Currbeat = LoopStart +1;
                aud.time = LoopStart * bpmfloat;
            }
            else if(mechscr.shooting)
            {
                if(OldState == 1)
                    return;
                
                CurrState = 1;
                LoopStart = CombatStart;
                LoopEnd = CombatEnd;
                Currbeat = LoopStart +1;
                aud.time = LoopStart * bpmfloat;
            }
            else
            {
                if(OldState == 0)
                    return;
                
                CurrState = 0;
                LoopStart = GeneralStart;
                LoopEnd = GeneralEnd;
                Currbeat = LoopStart +1;
                aud.time = LoopStart * bpmfloat;
            }

            OldState = CurrState;
        }

        if(CurrState == OldState)
        {
            if(aud.time >= LoopEnd * bpmfloat)
            {
                Currbeat = LoopStart +1;
                aud.time = LoopStart * bpmfloat;
            }
        }
    }
}
