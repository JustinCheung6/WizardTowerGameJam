using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySpells : MonoBehaviour
{
    private Dictionary<System.Type, Sprite> spellReferences = new Dictionary<System.Type, Sprite>();

    private Image[] spells = null;
    private RectTransform[] masks = null;

    private Vector2[] maskOrigins = null;

    private float[] maskSize = null;

    private void OnEnable()
    {
        UpdateManager.um.UpdateEvent += VisualizeSpells;
    }
    private void OnDisable()
    {
        UpdateManager.um.UpdateEvent -= VisualizeSpells;
    }
    private void Start()
    {
        for (int i = 0; i < SpellManager.sm.SpellAmount; i++)
        {
            GameObject spell = SpellManager.sm.GetSpellPrefab(i);

            Debug.Log("Checking: " + spell.name);

            foreach(SpriteRenderer s in spell.GetComponentsInChildren<SpriteRenderer>())
                if (s.CompareTag("Icon"))
                {
                    Debug.Log("Entered Type: " + spell.GetComponent<Spell>().GetType());
                    spellReferences.Add(spell.GetComponent<Spell>().GetType(), s.sprite);
                    break;
                }
            
        }

        spells = new Image[SpellManager.sm.SpellAmount];
        masks = new RectTransform[SpellManager.sm.SpellAmount];
        maskOrigins = new Vector2[SpellManager.sm.SpellAmount];
        maskSize = new float[SpellManager.sm.SpellAmount];

        foreach (Mask m in GetComponentsInChildren<Mask>())
        {
            int i = 0;
            if (m.CompareTag("1stSpell"))
                i = 0;
            else if (m.CompareTag("2ndSpell"))
                i = 1;
            else if (m.CompareTag("3rdSpell"))
                i = 2;

            spells[i] = m.GetComponentsInChildren<Image>()[1];
            masks[i] = m.rectTransform;
            maskOrigins[i] = masks[i].anchoredPosition;
            maskSize[i] = masks[i].rect.height * masks[i].localScale.y;
        }

    }
    private void VisualizeSpells()
    {
        for (int i = 0; i < spellReferences.Count; i++)
        {
            if(SpellManager.sm.InventorySize < spellReferences.Count)
            {
                spells[i].sprite = null;
                masks[i].anchoredPosition = maskOrigins[i];
                spells[i].rectTransform.anchoredPosition = new Vector2(0f, -100f);
                spells[i].color = Color.black;
            }
            else
            {
                //Change sprite of spell
                Spell spell = SpellManager.sm.GetInventory(i);

                Debug.Log("Looking for Type: " + spell.GetType());
                spells[i].sprite = spellReferences[spell.GetType()];

                //Change position of spell
                float scale = (SpellManager.sm.GetCooldown(i) / spell.Cooldown);
                masks[i].anchoredPosition = new Vector2(maskOrigins[i].x, maskOrigins[i].y + scale * maskSize[i]);
                spells[i].rectTransform.anchoredPosition = new Vector2(0f, -scale * 100);

                //Change black or white
                if (SpellManager.sm.GetCooldown(i) == 0)
                    spells[i].color = Color.white;
                else
                    spells[i].color = Color.black;
            }
        }
    }
}
