using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    //Manager that handles which spells the player can cast next, and casting spells
    public static SpellManager sm = null;

    [Header("Available Spells")]
    [SerializeField] private GameObject[] spellPrefabs;

    //[Header("Debug")]
    private List<Spell> inventory = new List<Spell>();
    private List<float> cooldowns = new List<float>();

    private List<Spell> castedSpells = new List<Spell>();

    private Dictionary<int, List<Spell>> spellPools = new Dictionary<int, List<Spell>>();

    public bool IsCurrentSpell(Spell spell) { return spell == inventory[0]; }
    public void ExtendCooldown(float amount) { cooldowns[0] += amount; }

    private void OnEnable()
    {
        if (sm == null)
            sm = this;

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
        //spells = new Spell[spellPrefabs.Length];

        for (int i = 0; i < spellPrefabs.Length; i++)
        {
            List<Spell> spellPool = new List<Spell>();
            for(int j = 0; j < 3; j++)
            {
                Spell spell = Instantiate(spellPrefabs[i], transform).GetComponent<Spell>();
                spell.enabled = false;
                spellPool.Add(spell);
            }
            spellPools.Add(i, spellPool);
        }

            
    }
    private void ManageSpells()
    {
        if (spellPools.Count == 0)
            return;

        //Fire spell
        if ((Input.GetButtonDown("Fire") && inventory[0].CanCast() ) || inventory[0].CastingSpell)
            if(cooldowns[0] <= 0)
                inventory[0].CastSpell(Input.GetButtonDown("Fire"), 
                    Input.GetButton("Fire"), Input.GetButtonUp("Fire"));

        //Remove Spell if casted
        if (inventory[0].SpellCasted)
        {
            Spell tmp = inventory[0];
            inventory.RemoveAt(0);
            cooldowns.RemoveAt(0);

            inventory[0].enabled = true;

            castedSpells.Add(tmp);
        }

        //Remove castedSpells if they're done being used
        if(castedSpells.Count > 0)
            for(int i = 0; i < castedSpells.Count; i++)
                if (!castedSpells[i].SpellCasted)
                {
                    Spell tmp = castedSpells[i];
                    castedSpells.RemoveAt(i);

                    //Move spell to end of list
                    foreach (List<Spell> spellPool in spellPools.Values)
                    {
                        if (spellPool.Contains(tmp))
                        {
                            spellPool.Remove(tmp);
                            spellPool.Add(tmp);
                            break;
                        }
                    }

                    tmp.ResetSpell();
                    tmp.enabled = false;
                    i--;
                }
    }
    private void ManageInventory()
    {
        if (spellPools.Count == 0)
            return;

        //Case: add more spells
        if (inventory.Count <= spellPrefabs.Length)
        {
            Spell[] spells = GetNextSet();

            for (int i = 0; i < spells.Length; i++)
            {
                inventory.Add(spells[i]);
                cooldowns.Add(spells[i].Cooldown);

                if(inventory.Count == 1)
                    inventory[0].enabled = true;
            }
        }

        for(int i = 0; i < spellPrefabs.Length; i++)
        {
            if (cooldowns[i] > Time.deltaTime)
                cooldowns[i] -= Time.deltaTime;
            else if (cooldowns[i] > 0)
                cooldowns[i] = 0;
        }
    }
    private Spell[] GetNextSet()
    {
        Spell[] spells = new Spell[spellPrefabs.Length];
        for (int i = 0; i < spells.Length; i++)
            spells[i] = PoolSpell(i);

        for (int i = 0; i < 2; i++)
            for(int j = 0; j < spells.Length; j++)
            {
                Spell tmp = spells[j];
                int randIndex = Random.Range(0, spells.Length-1);
                
                spells[j] = spells[randIndex];
                spells[randIndex] = tmp;
            }
        return spells;
    }

    private Spell PoolSpell(int index)
    {
        List<Spell> spellPool = spellPools[index];

        for(int i = 0; i < spellPool.Count; i++)
            if (!spellPool[i].enabled)
            {
                spellPool[i].enabled = false;
                return spellPool[i];
            }

        //Create new spell if pool runs out
        Spell spell = Instantiate(spellPrefabs[index], transform).GetComponent<Spell>();
        spell.enabled = false;
        spellPool.Add(spell);

        return spell;
    }
}
