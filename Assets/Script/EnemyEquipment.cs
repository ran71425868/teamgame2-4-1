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

    [Header("アイテム（防具）モデル")]
    public GameObject chestArmorModel; // インスペクターで胴体のアーマーモデルをセット
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

    public void OnAttackStart(int active)
    {
        bool isActive = (active == 1);

        // 現在アクティブ（表示中）な武器を探して判定を切り替える
        GameObject currentWeapon = GetActiveWeapon();

        if (currentWeapon != null)
        {
            EnemyWeapon weaponScript = currentWeapon.GetComponent<EnemyWeapon>();
            if (weaponScript != null)
            {
                weaponScript.SetAttackActive(isActive);
            }
            else
            {
                Debug.LogWarning(currentWeapon.name + " に EnemyWeapon スクリプトが付いていません！");
            }
        }
    }

    public void OnAttackEnd(int active)
    {
        bool isActive = (active == 1);

        // すべての武器に対して安全に判定をオフにする
        DeactivateWeaponHit(swordModel, isActive);
        DeactivateWeaponHit(axeModel, isActive);
        DeactivateWeaponHit(hammerModel, isActive);
        DeactivateWeaponHit(defaultWeapon, isActive);
    }

    // --- 便利機能：現在表示されている武器を返す ---
    private GameObject GetActiveWeapon()
    {
        if (swordModel != null && swordModel.activeSelf) return swordModel;
        if (axeModel != null && axeModel.activeSelf) return axeModel;
        if (hammerModel != null && hammerModel.activeSelf) return hammerModel;
        if (defaultWeapon != null && defaultWeapon.activeSelf) return defaultWeapon;
        return null;
    }

    // --- 便利機能：安全にスクリプトを叩く ---
    private void DeactivateWeaponHit(GameObject model, bool active)
    {
        if (model != null)
        {
            EnemyWeapon script = model.GetComponent<EnemyWeapon>();
            if (script != null) script.SetAttackActive(active);
        }
    }


    public void EquipItemVisual(string itemName)
    {
        // 全てのアイテムを一度非表示にする（必要に応じて）
        if (chestArmorModel) chestArmorModel.SetActive(false);

        // アイテム名に "Armor" が含まれていたら表示
        if (itemName.Contains("Armor"))
        {
            if (chestArmorModel)
            {
                chestArmorModel.SetActive(true);
                Debug.Log("アーマーを装備しました！");
            }
        }
    }
}