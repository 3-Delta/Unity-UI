using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Rct {
    public int w;
    public int h;
    public Color color;

    public bool used;

    public Rct(int w, int h, Color color) {
        this.w = w;
        this.h = h;
        this.color = color;
    }
}

public class MondrianBlocks : MonoBehaviour {
    public static readonly int LINE = 8;
    public Block[] blocks = new Block[LINE * LINE];
    public Rct[] rcts = new Rct[LINE];
    public BlockSetter setter;

    [ContextMenu(nameof(CollectBlcoks))]
    public void CollectBlcoks() {
        int i = 0;
        int length = transform.childCount;
        for (i = 0; i < length; ++i) {
            var child = transform.GetChild(i);
            if (child.TryGetComponent<Block>(out var block)) {
                blocks[i] = block;
            }
            else {
                blocks[i] = child.gameObject.AddComponent<Block>();
            }

            blocks[i].setter = setter;

            blocks[i].x = i / 8;
            blocks[i].y = i % 8;
        }
        
        rcts[0] = new Rct(1, 4, Color.blue);
        rcts[1] = new Rct(1, 5, Color.blue);
        
        rcts[2] = new Rct(2, 2, Color.cyan);
        rcts[3] = new Rct(2, 3, Color.magenta);
        rcts[4] = new Rct(2, 4, Color.magenta);
        rcts[5] = new Rct(2, 5, Color.magenta);
        
        rcts[6] = new Rct(3, 3, Color.cyan);
        rcts[7] = new Rct(3, 4, Color.green);
    }

    public bool IsComplete() {
        foreach (var block in this.blocks) {
            if (!block.used) {
                return false;
            }
        }

        return true;
    }

    public bool CanSelect(Rct rct, int i, int j) {
        if (rct.used) {
            return false;
        }

        int maxI = i + rct.h;
        int maxJ = j + rct.w;

        if (maxI >= LINE || maxJ >= LINE) {
            return false;
        }

        for (int ii = i; ii < maxI; ++ii) {
            for (int jj = j; jj < maxJ; ++jj) {
                int totalIndex = ii * LINE + jj;
                if (this.blocks[totalIndex].used) {
                    return false;
                }
            }
        }

        return true;
    }

    public void SetSecect(Rct rct, int i, int j, bool toSelect) {
        int maxI = i + rct.h;
        int maxJ = j + rct.w;
        for (int ii = i; ii < maxI; ++ii) {
            for (int jj = j; jj < maxJ; ++jj) {
                int totalIndex = ii * LINE + jj;
                this.blocks[totalIndex].SetSelect(toSelect, ESelectType.Normal, rct.color);
            }
        }

        rct.used = true;
    }
    
    public void Find(int i, int j) {
        if (this.IsComplete()) {
            //
        }
        else {
            bool can = TryFind(i, j, out Rct rct);
            if (can) {
                SetSecect(rct, i, j, true);
            }

            Find(i, j + 1);
            Find(i + 1, j);
            Find(i + 1, j + 1);
            
            if (can) {
                SetSecect(rct, i, j, false);
            }
        }
    }
    
    public bool TryFind(int i, int j, out Rct block)
    {
        for (int k = 0; k < this.rcts.Length; k++)
        {
            block = rcts[k];
            if (CanSelect(block, i, j))
            {
                return true;
            }
        }
        block = default;
        return false;
    }
}
