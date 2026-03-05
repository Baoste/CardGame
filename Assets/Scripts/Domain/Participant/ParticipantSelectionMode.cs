using Game.Domain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticipantSelectionMode
{
    public virtual List<int> Execute(GameState state, List<int> pool, int count, List<int> selected) { return null; }

    public virtual bool ValidatePool(List<int> pool, int count)
    { 
        return false; 
    }

    public virtual bool ValidateSelected(List<int> pool, List<int> selected) 
    {
        foreach (int id in selected)
        {
            if (!pool.Contains(id))
                return false;
        }
        return true;
    }
}

public class SelectionModeNone : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected) => new List<int>();
    public override bool ValidatePool(List<int> pool, int count) => true;
    public override bool ValidateSelected(List<int> pool, List<int> selected) => true;
}

public class SelectionModeAll : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected) => pool;
    public override bool ValidatePool(List<int> pool, int count) => pool.Count >= count;
    public override bool ValidateSelected(List<int> pool, List<int> selected)
    {
        return base.ValidateSelected(pool, selected);
    }
}


public class SelectionModeChoose : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected) => selected;
    public override bool ValidatePool(List<int> pool, int count) => pool.Count >= count;
    public override bool ValidateSelected(List<int> pool, List<int> selected)
    {
        return base.ValidateSelected(pool, selected);
    }
}

public class SelectionModeFirst : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected)
    {
        List<int> res = new List<int>();
        if (pool.Count > 0)
            res.Add(pool[0]);
        return res;
    }
    public override bool ValidatePool(List<int> pool, int count) => pool.Count >= count;
    public override bool ValidateSelected(List<int> pool, List<int> selected)
    {
        return base.ValidateSelected(pool, selected);
    }
}

public class SelectionModeLast : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected)
    {
        List<int> res = new List<int>();
        if (pool.Count > 0)
            res.Add(pool[pool.Count - 1]);
        return res;
    }
    public override bool ValidatePool(List<int> pool, int count) => pool.Count >= count;
    public override bool ValidateSelected(List<int> pool, List<int> selected)
    {
        return base.ValidateSelected(pool, selected);
    }
}

public class SelectionModeRandom : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected)
    {
        List<int> res = new List<int>();

        StaticFunction.Shuffle(pool, state.rng);
        for (int i = 0; i < count; i++)
        {
            if (pool.Count >= count - i)
                res.Add(pool[i]);
        }
        return res;
    }
    public override bool ValidatePool(List<int> pool, int count) => pool.Count >= count;
    public override bool ValidateSelected(List<int> pool, List<int> selected)
    {
        return base.ValidateSelected(pool, selected);
    }
}

public class SelectionModeMin : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected)
    {
        List<int> res = new List<int>();
        if (pool.Count > 0)
            res.Add(pool.Min());
        return res;
    }
    public override bool ValidatePool(List<int> pool, int count) => pool.Count >= count;
    public override bool ValidateSelected(List<int> pool, List<int> selected)
    {
        return base.ValidateSelected(pool, selected);
    }
}

public class SelectionModeMax : ParticipantSelectionMode
{
    public override List<int> Execute(GameState state, List<int> pool, int count, List<int> selected)
    {
        List<int> res = new List<int>();
        if (pool.Count > 0)
            res.Add(pool.Max());
        return res;
    }
    public override bool ValidatePool(List<int> pool, int count) => pool.Count >= count;
    public override bool ValidateSelected(List<int> pool, List<int> selected)
    {
        return base.ValidateSelected(pool, selected);
    }
}