using UnityEngine;

public class InventorySlotButton : ButtonBase
{
    public void ChangeIcon(Sprite sprite)
    {
        _buttonImage.sprite = sprite;
    }

    public void ChangeCount(int count)
    {
        string countString = count.ToString();
        _buttonText.text = countString;
    }
}
