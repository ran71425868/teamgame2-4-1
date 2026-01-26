using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEquipment : MonoBehaviour
{
    [Header("手元の武器モデル")]
    public GameObject swordModel;
    public GameObject axeModel;
    public GameObject hammerModel;
    public GameObject defaultWeapon;

    public void ChangeWeaponVisual(string weaponName)
    {
        // 全ての武器を一度非表示にする（念のため）
        if (swordModel) swordModel.SetActive(false);
        if (axeModel) axeModel.SetActive(false);
        if (hammerModel) hammerModel.SetActive(false);

        // 拾った武器の名前に特定の文字が含まれているかチェックして、手元のモデルを表示
        if (weaponName.Contains("Sword"))
        {
            if (swordModel) swordModel.SetActive(true);
        }
        else if (weaponName.Contains("Axe"))
        {
            if (axeModel) axeModel.SetActive(true);
        }
        else if (weaponName.Contains("Hammer"))
        {
            if (hammerModel) hammerModel.SetActive(true);
        }
        else
        {
            // どれにも該当しない場合はデフォルト（weaponOnHand）を表示
            if (defaultWeapon) defaultWeapon.SetActive(true);
        }
    }
}