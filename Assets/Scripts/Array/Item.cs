using UnityEngine;


namespace Redsilver2.Array
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private string itemName;
        [SerializeField] private string itemDescription;
        public uint ID { get; private set; }

        private void Start()
        {
            GenerateRandomID();
        }

        public void GenerateRandomID()
        {
            ID = (uint)Random.Range(0, uint.MaxValue);
            itemDescription = $"Too Lazy to put description for item #{ID}";
        }
        public override string ToString() => $"<b>Name: {itemName} | ID: {ID}</b>\n\n{itemDescription}";

    }
}
