using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfinityGridTest : MonoBehaviour {
    public InfinityGrid grid;

    private void Awake() {
        grid.onCellChanged = onCellChanged;
        grid.onCellCreated = onCellCreated;
    }

    private void onCellCreated(InfinityGridCell cell) {
    }
    
    private void onCellChanged(InfinityGridCell cell, int index) {
        Text text = cell.bindGo.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            this.grid.CellCount = 20;
            this.grid.ForceRefreshActiveCell();
        }
        else if (Input.GetKeyDown(KeyCode.B)) {
            this.grid.CellCount = 500;
            this.grid.ForceRefreshActiveCell();
        }
        else if (Input.GetKeyDown(KeyCode.C)) {
            this.grid.CellCount = 3;
            this.grid.ForceRefreshActiveCell();
        }
    }
}
