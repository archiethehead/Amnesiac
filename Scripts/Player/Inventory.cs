using Godot;
using System;

public partial class Inventory : Node
{
    private GameManager GameManager = null;
    private const int MaxBits = 32 - 1;
    private int CurrentItemIndex = -1;
    private int InventoryBitArray = 0;

    public override void _Ready() {

        GameManager = GameManager.Instance;
        GameManager.Instance.Inventory = this;

    }

    public bool IsItemInInventory(int ID) {

        int IsID = InventoryBitArray & (1 << (ID & MaxBits));

        if (IsID == 0) {

            return false;
        
        }

        return true;
    
    }

    public void AddItemToInventory(int ID) {

        InventoryBitArray |= (1 << (ID & MaxBits));
        EquipItem(ID);

    }

    public void EquipItem(int ID) {

        CurrentItemIndex = ID;
    
    }
         
}
