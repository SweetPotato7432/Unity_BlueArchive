using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct TMPMaterials
{
    public string ID;// ���׸��� �̸�
    public Material Mat; // TMP ���׸���
}

public class DamageUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text damageText;

    [SerializeField]
    private TMP_Text typeText;

    [SerializeField]
    private GameObject criticalUI;

    [SerializeField]
    private List<TMPMaterials> _materialAssets;

    Camera mainCamera;
    public float offset = 0.5f;

    private void Start()
    {
        transform.position += new Vector3(0, 1, 0);
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.1f, 0.5f), 0);
        transform.position += randomOffset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // ������ �ٶ󺸴� UI
        Quaternion cameraRotation = Camera.main.transform.rotation;

        transform.rotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
    }

    public void SetData(string Damage, bool isCritical, DamageCode damageEffective)
    {
        // ������ ��ġ
        damageText.text = Damage;

        // ġ��Ÿ ����
        if(isCritical)
        {
            if (!criticalUI.activeSelf)
            {
                criticalUI.SetActive(true);
            }
            
        }
        else
        {
            if(criticalUI.activeSelf)
            {
                criticalUI.SetActive(false);
            }
            
        }

        // ������ Ÿ�Կ� ���� ������, ũ�� ����
        switch(damageEffective)
        {
            case DamageCode.None:
                damageText.fontMaterial = _materialAssets.Find(x => x.ID.Equals("Normal")).Mat;
                this.transform.localScale = new Vector3(0.5f, 0.5f);
                if (typeText.gameObject.activeSelf)
                {
                    typeText.gameObject.SetActive(false);
                }
                break;
            case DamageCode.Resist:
                damageText.fontMaterial = _materialAssets.Find(x => x.ID.Equals("ResistNum")).Mat;
                typeText.text = "Resist";
                typeText.fontSize = 0.75f;
                typeText.fontMaterial = _materialAssets.Find(x => x.ID.Equals("Resist")).Mat;
                this.transform.localScale = new Vector3(0.5f, 0.5f);
                if (!typeText.gameObject.activeSelf)
                {
                    typeText.gameObject.SetActive(true);
                }
                break;
            case DamageCode.Normal:
                damageText.fontMaterial = _materialAssets.Find(x => x.ID.Equals("Normal")).Mat;
                this.transform.localScale = new Vector3(0.5f, 0.5f);
                if (typeText.gameObject.activeSelf)
                {
                    typeText.gameObject.SetActive(false);
                }
                break;
            case DamageCode.Effective:
                damageText.fontMaterial = _materialAssets.Find(x => x.ID.Equals("EffecitveNum")).Mat;
                this.transform.localScale = new Vector3(0.5f, 0.5f);
                if (typeText.gameObject.activeSelf)
                {
                    typeText.gameObject.SetActive(false);
                }
                break;
            case DamageCode.Weak:
                damageText.fontMaterial = _materialAssets.Find(x => x.ID.Equals("WeakNum")).Mat;
                typeText.fontMaterial = _materialAssets.Find(x => x.ID.Equals("Weak")).Mat;
                typeText.text = "Weak";
                this.transform.localScale = new Vector3(0.75f, 0.75f);
                if (!typeText.gameObject.activeSelf)
                {
                    typeText.gameObject.SetActive(true);
                }
                break;
        }


    }
}
