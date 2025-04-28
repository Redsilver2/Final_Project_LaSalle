using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


namespace Redsilver2.Array
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private string itemName;
        [SerializeField] private string itemDescription;

        [SerializeField] private string itemType;

        [Space]
        [SerializeField] private AbilityData[] abilityDatas;

        private ItemAbility mainAbility;


        private uint  currentLevel;
        private float exprienceNeededToLevelup = 5;
        private float currentExperience;

        public uint ID { get; private set; }

        private async void Start()
        {
            if(abilityDatas.Length > 0f)
            {
                abilityDatas = abilityDatas.DistinctBy(x => x.UnlockedLevel).ToArray();
                mainAbility = new ItemAbility(abilityDatas[0].AbilityName, abilityDatas[0].AbilityDescription, abilityDatas[0].UnlockedLevel, abilityDatas[0].OnActivate);

                for (int i = 1; i < abilityDatas.Length; i++)
                {
                    AbilityData data = abilityDatas[i];
                    mainAbility = await InsertItemAbility(mainAbility, new ItemAbility(data.AbilityName, data.AbilityDescription, data.UnlockedLevel, data.OnActivate));
                }
            }
     
           
            
            GenerateRandomID();
        }

        public void GenerateRandomID()
        {
            ID = (uint)Random.Range(0, uint.MaxValue);
            itemDescription = $"Too Lazy to put description for item #{ID}";
        }

        public float GetLevelUpProgress() => Mathf.Clamp01(currentExperience / exprienceNeededToLevelup); 


        public async Task<string> GetDetails()
        {
            StringBuilder builder     = new StringBuilder();
            ItemAbility   nextAbility = await FindNextAbility(mainAbility);

            builder.Append($"<b>Name: {itemName} | ID: {ID}</b> | Level: {currentLevel} ({(int)currentExperience}/{exprienceNeededToLevelup})");

            if (nextAbility != null)
                builder.Append($"\n{nextAbility.UnlockRequierement()}");
            else
                builder.Append("\nYou have unlocked all abilities :D");

                builder.Append($"\n\n{itemDescription}");
            return builder.ToString();
        }

        public void GainExperience(float amount)
        {
            currentExperience += amount;

            if(currentExperience > exprienceNeededToLevelup)
                LevelUp();
        }

        private void LevelUp()
        {
            exprienceNeededToLevelup *= 2f;
            currentExperience = 0f;
            currentLevel++;
        }

        private async Task<ItemAbility> InsertItemAbility(ItemAbility mainAbility, ItemAbility newAbility)
        {
            ItemAbility current;

            if (mainAbility == null || newAbility == null) return mainAbility;
            current =  mainAbility;

            await Task.Run(async () =>
            {
                while (current.next != null)
                {
                    current = current.next;
                    await Task.Yield();
                }
            });

            current.next = newAbility;
            Debug.Log(current.next.ToString());
            return current; 
        }

        private async Task<ItemAbility> FindNextAbility(ItemAbility mainAbility)
        {
            ItemAbility current = mainAbility;

            await Task.Run(async () =>
            {
                bool foundNextAbility = false;

                while(current != null)
                {
                    if (currentLevel < current.UnlockedLevel) {
                        foundNextAbility = true;
                        break; 
                    }

                    current = current.next;
                    await Task.Yield();
                }

                if (!foundNextAbility) { current = null; }
            });

            return current;
        }

        public async Task<ItemAbility[]> GetAbilitiesUnlocked(ItemAbility mainAbility)
        {
            ItemAbility       current;
            List<ItemAbility> abilities;
            
            if (mainAbility == null) { return null; }
          
            current   = mainAbility;
            abilities = new List<ItemAbility>();

            await Task.Run(async () =>
            {
                while (current.next != null)
                {
                    if (currentLevel <= current.UnlockedLevel)
                        abilities.Add(current);
                    else
                        break;

                    current = current.next;
                    await Task.Yield();
                }
            });

            return  abilities.ToArray();
        }

        public async Task<string> GetAbilitiesUnlockDetails(ItemAbility abilityUnlock)
        { 
            ItemAbility[] abilities;
            StringBuilder builder;
            
            if (abilityUnlock == null) { return string.Empty; }

            abilities = await GetAbilitiesUnlocked(abilityUnlock);
            builder   = new StringBuilder();

            foreach (ItemAbility ability in abilities) { builder.Append(ability.AbilityName).Append("\n"); }
            return builder.ToString();
        }
    }

    public class ItemAbility
    {

        private string abilityName;

        private string abilityDescription;

        private uint unlockedLevel;


        private UnityEvent onActivate;
        public  ItemAbility  next;

        public string AbilityName => abilityName;
        public string AbilityDescription => abilityDescription;
        public uint UnlockedLevel => unlockedLevel;


        public ItemAbility(string abilityName, string abilityDescription, uint unlockedLevel)
        {
            this.abilityName        = abilityName;
            this.abilityDescription = abilityDescription;
            this.unlockedLevel      = unlockedLevel;
        }
        public ItemAbility(string abilityName, string abilityDescription, uint unlockedLevel, UnityEvent onActivate) : this(abilityName, abilityDescription, unlockedLevel)
        {
            this.onActivate = onActivate;
        }

        public void Activate()
        {
            if(onActivate != null) onActivate.Invoke();
        }
        public void AddOnActivateEvent(UnityAction onActivate)
        {
            if(onActivate != null && this.onActivate != null) this.onActivate.AddListener(onActivate);
        }
        public void RemoveOnActivateEvent(UnityAction onActivate)
        {
            if (onActivate != null && this.onActivate != null) this.onActivate.RemoveListener(onActivate);
        }

        public string UnlockRequierement() => $"Requires <color=yellow>Level {unlockedLevel}</color> To Unlock Next Ability (Unlocks {abilityName})";
        public override string ToString()  => $"Ability Name: {abilityName} | Description: {abilityDescription} | Unlock Level: {unlockedLevel}";
 
    }

    [System.Serializable]
    public struct AbilityData
    {
        [SerializeField] private string abilityName;
        [SerializeField] private string abilityDescription;
        [SerializeField] private uint   unlockedLevel;

        [Space]
        [SerializeField] private UnityEvent onActivate;

        public string AbilityName => abilityName;
        public string AbilityDescription => abilityDescription;

        public uint UnlockedLevel => unlockedLevel;

        public UnityEvent OnActivate => onActivate;
    }
}
