using Godot;
using System;

public partial class Inventory : Node
{
    private GameManager GameManager = null;
    private int CurrentItemIndex = -1;
    private int InventoryBitArray = 0;

    public override void _Ready() {

        GameManager = GameManager.Instance;
        GameManager.Instance.Inventory = this;

    }

    public bool IsItemInInventory(int ID) {

        int IsID = InventoryBitArray & ID;

        if (IsID == 0) {

            return false;
        
        }

        return true;
    
    }

    public void AddItemToInventory(int ID) {

        InventoryBitArray |= ID;
        EquipItem(ID);

    }

    public void EquipItem(int ID) {

        CurrentItemIndex = ID;
    
    }
         
}
