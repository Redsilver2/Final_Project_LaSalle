using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Redsilver2.Array
{
    public class ItemDisplayer : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI informationDisplayer;
        [SerializeField] private Image levelupFillbar;

        [Space]
        [SerializeField] private float rotationSpeed     = 5f;
        [SerializeField] private float bounceSpeed       = 5f;
        [SerializeField] private float positionLerpSpeed = 3f;

        [Space]
        [SerializeField] private float maxItemHeight = 5f;
        [SerializeField] private float itemSpacing   = 10f;

        private float currentItemPositionY;
        private int currentIndex = 0;

        private bool canUpdate = true;
        private Item[] items = null;

        private void Awake()
        {
            items = FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        private async void Update()
        {
            if (!canUpdate)
            {
                SetInformationText(string.Empty);
                return;
            }

            if (Input.GetKeyDown(KeyCode.A))
              SetCurrentIndex(items, false);
         
            if (Input.GetKeyDown(KeyCode.D))
              SetCurrentIndex(items, true);

            if (Input.GetKeyDown(KeyCode.R))
                GenerateRandomIDForItems();


            if (Input.GetKeyDown(KeyCode.Alpha1))
               items = await SortArray(items);


            currentItemPositionY = Mathf.Lerp(-maxItemHeight, maxItemHeight, Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed)));
        }

        private void LateUpdate()
        {
            if (!canUpdate) return;
            DisplayItems(items);
        }

        private void GenerateRandomIDForItems()
        {
            foreach (Item item in items) item.GenerateRandomID();
        }

        private void DisplayItems(Item[] items)
        {
            if (items == null) return;

            if(items.Length > 0)
            {
                Item currentItem = items[currentIndex];
                SetInformationText(currentItem);
                currentItem.GainExperience(10f * Time.deltaTime);
                                
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

        private async void SetInformationText(Item item)
        {
            if (item != null)
            {
                string details = await item.GetDetails();
                SetInformationText(details);    
            }
        }

        private void SetInformationText(string text)
        {
            if (informationDisplayer != null)
                informationDisplayer.text = text;
        }

        private void SetLevelupFillBar(Item item)
        {
            if (levelupFillbar != null && item != null)
            {
                levelupFillbar.fillAmount = item.GetLevelUpProgress();
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
            else if (itemIndex >= items.Length) itemIndex = 0;
        }

        private async Task<Item[]>SortArray(Item[] items)
        {
            canUpdate = false;

            for (int i = 0; i < items.Length - 1; i++)
                for (int j = 0; j < items.Length - i - 1; j++)
                    if (items[j].ID > items[j + 1].ID)
                    {
                        Item tempVar = items[j];
                        items[j] = items[j + 1];
                        items[j + 1] = tempVar;

                        VisualizeSorting(items[j], tempVar, 1f);
                        await Awaitable.WaitForSecondsAsync(1f);
                    }

            canUpdate = true;
            return items;
        }

        private async void VisualizeSorting(Item item01, Item item02, float duration)
        {

            if(item01 != null && item02 != null)
            {
                duration /= 3f;
                await Visualize(item01.transform.position + Vector3.up * 5, item02.transform.position + Vector3.up * 5, duration);
                await Visualize(item02.transform.position                 , item01.transform.position                                  , duration);
                await Visualize(item01.transform.position - Vector3.up * 5, item02.transform.position - Vector3.up * 5, duration);
            }

            async Task Visualize(Vector3 endPosition01, Vector3 endPosition02, float duration)
            {
                float t = 0f;
                Vector3 startPosition01, startPosition02;

                startPosition01 = item01.transform.position;
                startPosition02 = item02.transform.position;

                while (t < duration)
                {
                    float progress = t / duration;
                    LerpPosition(item01.transform, startPosition01, endPosition01, progress);
                    LerpPosition(item02.transform, startPosition02, endPosition02, progress);

                    t += Time.deltaTime;
                    await Task.Yield();
                }

                item01.transform.position = endPosition01;
                item02.transform.position = endPosition02;

                void LerpPosition(Transform transform, Vector3 startPosition, Vector3 endPosition, float progression)
                {
                    progression = Mathf.Clamp01(progression);
                    transform.position = Vector3.Lerp(startPosition, endPosition, progression);
                }
            }
        }
    }
}
