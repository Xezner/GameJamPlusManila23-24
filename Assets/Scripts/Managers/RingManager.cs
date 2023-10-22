using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RingManager : Singleton<RingManager>
{
    [SerializeField] private List<RingBlock> _ringBlocks;
    [SerializeField] private GoalBlock _goalBlock;
    private int _ringBlocksCount;
    // Start is called before the first frame update
    void Start()
    {
        _ringBlocksCount = _ringBlocks.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetRingBlocks();
        }
    }
    public void ResetRingBlocks()
    {
        foreach (var ringBlock in _ringBlocks)
        {
            ringBlock.Reset();
        }
        _goalBlock.LockGoal();
        _ringBlocksCount = _ringBlocks.Count;
    }

    public void UpdateRingBlockCount()
    {
        int ringBlockCountHolder = 0;
        foreach(var ringBlock in _ringBlocks)
        {
            if(!ringBlock.IsEntered)
            {
                ringBlockCountHolder++;
            }
        }
        _ringBlocksCount = ringBlockCountHolder;

        if(_ringBlocksCount == 0)
        {
            _goalBlock.UnlockGoal();
        }
    }

    public int GetBlockCount()
    {
            return _ringBlocksCount;
    }
}
