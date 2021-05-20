using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    //Manager that handles which spells the player can cast next, and casting spells

    [SerializeField] private GameObject[] spellPrefab;
    private Spell[] spells;

    private List<Spell> inventory = new List<Spell>();
    private List<float> cooldowns = new List<float>();

    private void OnEnable()
    {
        UpdateManager.um.UpdateEvent += ManageInventory;
        UpdateManager.um.UpdateEvent += ManageSpells;
    }
    private void OnDisable()
    {
        UpdateManager.um.UpdateEvent -= ManageInventory;
        UpdateManager.um.UpdateEvent -= ManageSpells;
    }
    private void Start()
    {
        spells = new Spell[spellPrefab.Length];
        for (int i = 0; i < spellPrefab.Length; i++)
            spells[i] = spellPrefab[i].GetComponent<Spell>();
    }
    private void ManageSpells()
    {
        if (spells == null)
            return;

        //Fire spell
        if (Input.GetButtonDown("Fire") || inventory[0].CastingSpell)
            inventory[0].CastSpell(Input.GetButtonDown("Fire"), Input.GetButton("Fire"), Input.GetButtonUp("Fire"));

        //Remove Spell if casted
        if (inventory[0].SpellCasted)
        {
            inventory.RemoveAt(0);
            cooldowns.RemoveAt(0);
        }
    }
    private void ManageInventory()
    {
        if (spells == null)
            return;

        //Case: add more spells
        if (inventory.Count <= spells.Length)
        {
            ShuffleSpells();
            foreach (Spell p in spells)
            {
                inventory.Add(p);
                cooldowns.Add(p.Cooldown);
            }
        }

        for(int i = 0; i < spells.Length; i++)
        {
            if (cooldowns[i] > Time.deltaTime)
                cooldowns[i] -= Time.deltaTime;
            else if (cooldowns[i] > 0)
                cooldowns[i] = 0;
        }
    }
    private void ShuffleSpells()
    {
        for(int i = 0; i < 2; i++)
            for(int j = 0; j < spells.Length; j++)
            {
                Spell tmp = spells[j];
                int randIndex = Random.Range(0, spells.Length-1);
                
                spells[j] = spells[randIndex];
                spells[randIndex] = tmp;
            }
    }
}
