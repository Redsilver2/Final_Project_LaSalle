using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redsilver2.Array
{
    public class ItemDisplayer : MonoBehaviour
    {

        [SerializeField] private float rotationSpeed     = 5f;
        [SerializeField] private float bounceSpeed       = 5f;
        [SerializeField] private float positionLerpSpeed = 3f;

        [Space]
        [SerializeField] private float maxItemHeight = 5f;
        [SerializeField] private float itemSpacing   = 10f;

        private float currentItemPositionY;
        private int currentIndex = 0;

        private Item[] items = null;

        private void Awake()
        {
            items = FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
              SetCurrentIndex(items, true);
            else if (Input.GetKeyDown(KeyCode.D))
              SetCurrentIndex(items, false);

            currentItemPositionY = Mathf.Lerp(-maxItemHeight, maxItemHeight, Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed)));
        }

        private void LateUpdate()
        {
            DisplayItems(items);
        }

        private void DisplayItems(Item[] items)
        {
            if (items == null) return;

            if(items.Length > 0)
            {
                Item currentItem = items[currentIndex];     
                DisplayItem(items, currentIndex, Vector3.up * currentItemPositionY);
                DisplayItems(items, currentItem);
            }
        }

        private void DisplayItems(Item[] items, Item currentItem)
        {
            bool isOppositeDisplay = false;
            List<Item> leftSideItems;
            List<Item> rightSideItems;

            if (items == null) return;

            leftSideItems  = new List<Item>();
            rightSideItems = new List<Item>();


            for (int i = 0; i < items.Length; i++)
            {
                Item    item            = items[i];
             
                if (item == currentItem)
                {
                    isOppositeDisplay         = !isOppositeDisplay;
                    continue;
                }

                if (!isOppositeDisplay)
                    leftSideItems.Add(item);
                else
                    rightSideItems.Add(item);
            }

            leftSideItems.Reverse();

            Display(leftSideItems.ToArray(),  Vector3.left  * itemSpacing);
            Display(rightSideItems.ToArray(), Vector3.right * itemSpacing);

            void Display(Item[] items, Vector3 desiredPosition)
            {
                for (int i = 0; i < items.Length; i++)
                    DisplayItem(items[i], desiredPosition * (i + 1));
            }

        }

        private void DisplayItem(Item[] items, int selectableIndex, Vector3 desiredPosition)
        {
            ClampItemIndex(items, ref selectableIndex);
            DisplayItem(items[selectableIndex], desiredPosition);
        }

        private void DisplayItem(Item item, Vector3 desiredPosition)
        {
            Transform transform;
            if (item == null) return;

            transform = item.transform;
            transform.localEulerAngles += Time.deltaTime * Vector3.up * rotationSpeed;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionLerpSpeed);
        }

        private void SetCurrentIndex(Item[] items, bool isIncrementing)
        {
            if (isIncrementing) { currentIndex++; }
            else                { currentIndex--; }

            ClampItemIndex(items, ref currentIndex);

        }

        private void ClampItemIndex(Item[] items,ref int itemIndex)
        {
            if(items == null) return;
            if(itemIndex < 0)                   itemIndex = items.Length - 1;
            else if (itemIndex >= items.Length) itemIndex = items.Length - 1;
        }
    }
}
