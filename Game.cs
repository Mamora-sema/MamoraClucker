
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Security.Principal;

public class Game : MonoBehaviour
{
    [Header("Текст, отвечающий за отображение денег")]
    public Text scoreText;
    [Header("Текст, отвечающий за отображение денег в магазине")]
    public Text scoreTextmaga;
    public Text scoreTextmaga2;
    public Text scoreTextmaga3;
    [Header("Текст, отвечающий за ошибкив магазине")]
    public Text erormaga;
    public Text erormaga2;
    public Text erormaga3;
    [Header("Картинки")]
    public GameObject h1;
    public GameObject h2;
    public GameObject h3;
    




    [Header("Магазин")]
    public List<Item> shopItems = new List<Item>();
    [Header("Текст счета в ифно")]
    public Text scoreinfo;
    [Header("Текст бонусов в инфо")]
    public Text bonusinfo;
    [Header("Текст рабочих в игфо")]
    public Text werinfo;
    [Header("Текст мастеров в игфо")]
    public Text masterinfo;
    [Header("Текст боссов игфо")]
    public Text bossinfo;
    [Header("Текст богов в игфо")]
    public Text boginfo;
    [Header("Версии кнопки")]
    public GameObject v1;
    public GameObject v2;
    public GameObject v3;
    [Header("Тест")]
    public Text test1;
    public Text test2;
    [Header("Текст на кнопках товаров")]
    public Text[] shopItemsText;
    [Header("Кнопки товаров")]
    public Button[] shopBttns;
    [Header("Панелька магазина")]
    public GameObject shopPan;
    [Header("Панелька магазина улучшений")]
    public GameObject shopPan2;
    [Header("Панелька магазина рабов")]
    public GameObject shopPan3;
    [Header("Скоро")]
    public GameObject skoro;
    [Header("Инфо")]
    public GameObject infopan;
    [Header("Обновления")]
    public GameObject infomaxx;
    [Header("Кнопка выключена")]
    public Button er;
    
    private int score;
    private int ty;
    private int ver = 0;
    private int tyty = 1;

    private int score2;//Игровая валюта
    private int scoreIncrease = 1; //Бонус при клике
    private Save sv = new Save();

    private void Awake()
    {
        if (PlayerPrefs.HasKey("SV"))
        {
            int totalBonusPS = 0;
            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("SV"));
            score = sv.score;
            tyty= sv.scoreIncrease;
            scoreIncrease = sv.scoreIncrease;

            for (int i = 0; i < shopItems.Count; i++)
            {
                print((int)Mathf.Pow(shopItems[i].bonusIncrease, shopItems[i].levelOfItem));
                shopItems[i].levelOfItem = sv.levelOfItem[i];
                shopItems[i].bonusCounter = sv.bonusCounter[i];
                if (shopItems[i].needCostMultiplier) shopItems[i].cost *= (int)Mathf.Pow(shopItems[i].costMultiplier, shopItems[i].levelOfItem);
                if (shopItems[i].bonusIncrease != 0 && shopItems[i].levelOfItem != 0)
                totalBonusPS += shopItems[i].bonusPerSec * shopItems[i].bonusCounter;
            }
            
            print("-+++++++++++++++++");
            print(scoreIncrease);
            print("-+++++++++++++++++");
            DateTime dt = new DateTime(sv.date[0], sv.date[1], sv.date[2], sv.date[3], sv.date[4], sv.date[5]);
            TimeSpan ts = DateTime.Now - dt;
            int offlineBonus = (int)ts.TotalSeconds * totalBonusPS;
            score += offlineBonus;
            score2 += offlineBonus;

            
            if(tyty == 0)
            {
                tyty = 1;
            }
            if (scoreIncrease == 0)
            {
                scoreIncrease = 1;
            }
            



        }
    }
  

    private void Start()
    {
        er.interactable = false;
        updateCosts(); //Обновить текст с ценами
        StartCoroutine(BonusPerSec()); //Запустить просчёт бонуса в секунду
        if (ver == 1)
        {
            v2.SetActive(!v3.activeSelf);
            v3.SetActive(!v3.activeSelf);


        }
        if (ver ==2)
        {
            v1.SetActive(!v1.activeSelf);
            v3.SetActive(!v3.activeSelf);


        }
        if (ver == 3)
        {
            v2.SetActive(!v3.activeSelf);
            v1.SetActive(!v1.activeSelf);


        }
        scoreIncrease = tyty;
    }

    private void Update()
    {
        test1.text = $"{tyty}"; ;
        test2.text = $"{scoreIncrease}";
        scoreText.text = score + "$";
        scoreTextmaga.text = score + "$";
        scoreTextmaga2.text = score + "$";
        scoreTextmaga3.text = score + "$";
        scoreinfo.text = $"{score}$";//Отображаем деньги
        bonusinfo.text = $"Бонусов: {scoreIncrease}";
        werinfo.text = $"Работников: {shopItems[4].bonusCounter}";
        masterinfo.text = $"Мастеров: {shopItems[5].bonusCounter}";
        bossinfo.text = $"Боссов: {shopItems[6].bonusCounter}";
        boginfo.text = $"Богов: {shopItems[7].bonusCounter}";
        

    }

    public void BuyBttn(int index) //Метод при нажатии на кнопку покупки товара (индекс товара)
    {
        int cost = shopItems[index].cost * shopItems[shopItems[index].itemIndex].bonusCounter; //Рассчитываем цену в зависимости от кол-ва рабочих (к примеру)
        if (shopItems[index].itsBonus && score >= cost) //Если товар нажатой кнопки - это бонус, и денег >= цены(е)
        {
            if (cost > 0) // Если цена больше чем 0, то:
            {
                erormaga.text = "  ";
                erormaga2.text = "  ";
                erormaga3.text = "  ";
                score -= cost; // Вычитаем цену из денег
                StartCoroutine(BonusTimer(shopItems[index].timeOfBonus, index)); //Запускаем бонусный таймер
            }
            else print("Нечего улучшать то!"); // Иначе выводим в консоль текст.
        }
        else if (score >= shopItems[index].cost) // Иначе, если товар нажатой кнопки - это не бонус, и денег >= цена
        {
            erormaga.text = "  ";
            if (shopItems[index].itsItemPerSec) shopItems[index].bonusCounter++; // Если нанимаем рабочего (к примеру), то прибавляем кол-во рабочих
            else scoreIncrease += shopItems[index].bonusIncrease;
            tyty += shopItems[index].bonusIncrease; ;// Иначе бонусу при клике добавляем бонус товара
            score -= shopItems[index].cost; // Вычитаем цену из денег
            if (shopItems[index].needCostMultiplier) shopItems[index].cost *= shopItems[index].costMultiplier; // Если товару нужно умножить цену, то умножаем на множитель
            shopItems[index].levelOfItem++; // Поднимаем уровень предмета на 1
            
        }
        else
        {
            print("Не хватает денег!");
            erormaga.text = "Не хватает денег!";
            erormaga2.text = "Не хватает денег!";
            erormaga3.text = "Не хватает денег!";
            

        }
        // Иначе если 2 проверки равны false, то выводим в консоль текст.
        updateCosts(); //Обновить текст с ценами
    }
    private void updateCosts() // Метод для обновления текста с ценами
    {
        for (int i = 0; i < shopItems.Count; i++) // Цикл выполняется, пока переменная i < кол-ва товаров
        {
            if (shopItems[i].itsBonus) // Если товар является бонусом, то:
            {
                int cost = shopItems[i].cost * shopItems[shopItems[i].itemIndex].bonusCounter; // Рассчитываем цену в зависимости от кол-ва рабочих (к примеру)
                shopItemsText[i].text = shopItems[i].name + "\n" + cost + "$"; // Обновляем текст кнопки с рассчитанной ценой
            }
            else shopItemsText[i].text = shopItems[i].name + "\n" + shopItems[i].cost + "$"; // Иначе если товар не является бонусом, то обновляем текст кнопки с обычной ценой
        }
    }

    IEnumerator BonusPerSec() // Бонус в секунду
    {
        while (true) // Бесконечный цикл
        {
            for (int i = 0; i < shopItems.Count; i++) score += (shopItems[i].bonusCounter * shopItems[i].bonusPerSec); // Добавляем к игровой валюте бонус рабочих (к примеру)
            yield return new WaitForSeconds(1); // Делаем задержку в 1 секунду
        }
    }
    IEnumerator BonusTimer(float time, int index) // Бонусный таймер (длительность бонуса (в сек.),индекс товара)
    {
        shopBttns[index].interactable = false; // Выключаем кликабельность кнопки бонуса
        shopItems[shopItems[index].itemIndex].bonusPerSec *= 2; // Удваиваем бонус рабочих в секунду (к примеру)
        yield return new WaitForSeconds(time); // Делаем задержку на столько секунд, сколько указали в параметре
        shopItems[shopItems[index].itemIndex].bonusPerSec /= 2; // Возвращаем бонус в нормальное состояние
        shopBttns[index].interactable = true; // Включаем кликабельность кнопки бонуса
    }
#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            sv.score = score;
            sv.levelOfItem = new int[shopItems.Count];
            sv.bonusCounter = new int[shopItems.Count];
            for (int i = 0; i < shopItems.Count; i++)
            {
                sv.levelOfItem[i] = shopItems[i].levelOfItem;
                sv.bonusCounter[i] = shopItems[i].bonusCounter;
            }
            sv.date[0] = DateTime.Now.Year; sv.date[1] = DateTime.Now.Month; sv.date[2] = DateTime.Now.Day; sv.date[3] = DateTime.Now.Hour; sv.date[4] = DateTime.Now.Minute; sv.date[5] = DateTime.Now.Second;
            PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
        }
    }
#else
    public void info2()
    {
        sv.score = score;
        sv.scoreIncrease = scoreIncrease;
        sv.tart = scoreIncrease;
        sv.levelOfItem = new int[shopItems.Count];
        sv.bonusCounter = new int[shopItems.Count];
        for (int i = 0; i < shopItems.Count; i++)
        {
            sv.levelOfItem[i] = shopItems[i].levelOfItem;
            sv.bonusCounter[i] = shopItems[i].bonusCounter;
        }
        sv.date[0] = DateTime.Now.Year; sv.date[1] = DateTime.Now.Month; sv.date[2] = DateTime.Now.Day; sv.date[3] = DateTime.Now.Hour; sv.date[4] = DateTime.Now.Minute; sv.date[5] = DateTime.Now.Second;
        PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
    }
#endif

    public void showShopPan() 
    {
        shopPan.SetActive(!shopPan.activeSelf);
        

        erormaga.text = "  ";
    }
    public void skoro2() 
    {
        skoro.SetActive(!skoro.activeSelf);
        

    }
   
    public void info()
    {
        
        infopan.SetActive(!infopan.activeSelf);
    }
    public void MAXinfo()
    {
        infomaxx.SetActive(!infomaxx.activeSelf);
        h1.SetActive(!h1.activeSelf);
        h2.SetActive(!h2.activeSelf);
        h3.SetActive(!h3.activeSelf);
       
    }
    public void showShopPan2() 
    {
        shopPan2.SetActive(!shopPan2.activeSelf);
        shopPan.SetActive(!shopPan.activeSelf);
        h1.SetActive(!h1.activeSelf);
        h2.SetActive(!h2.activeSelf);
        h3.SetActive(!h3.activeSelf);
        
        erormaga2.text = "  ";
    }
    public void showShopPan3() 
    {
        shopPan3.SetActive(!shopPan3.activeSelf);
        shopPan.SetActive(!shopPan.activeSelf);
        h1.SetActive(!h1.activeSelf);
        h2.SetActive(!h2.activeSelf);
        h3.SetActive(!h3.activeSelf);
        
        erormaga3.text = "  ";
    }
    public void home() 
    {
        shopPan2.SetActive(!shopPan2.activeSelf);
        h1.SetActive(!h1.activeSelf);
        h2.SetActive(!h2.activeSelf);
        h3.SetActive(!h3.activeSelf);
        
        erormaga2.text = "  ";
    }
    public void home2() 
    {
        shopPan3.SetActive(!shopPan3.activeSelf);
        h1.SetActive(!h1.activeSelf);
        h2.SetActive(!h2.activeSelf);
        h3.SetActive(!h3.activeSelf);
        
        erormaga3.text = "  ";
    }

    public void OnClick() 
    {
        
        score += scoreIncrease;
        
    }

}

[Serializable]
public class Item // Класс товара
{
    [Tooltip("Название используется на кнопках")]
    public string name;
    [Tooltip("Цена товара")]
    public int cost;
    [Tooltip("Бонус, который добавляется к бонусу при клике")]
    public int bonusIncrease;
    [HideInInspector]
    public int levelOfItem; // Уровень товара
    [Space]
    [Tooltip("Нужен ли множитель для цены?")]
    public bool needCostMultiplier;
    [Tooltip("Множитель для цены")]
    public int costMultiplier;
    [Space]
    [Tooltip("Этот товар даёт бонус в секунду?")]
    public bool itsItemPerSec;
    [Tooltip("Бонус, который даётся в секунду")]
    public int bonusPerSec;
    [HideInInspector]
    public int bonusCounter; // Кол-во рабочих (к примеру)
    [Space]
    [Tooltip("Это временный бонус?")]
    public bool itsBonus;
    [Tooltip("Множитель товара, который управляется бонусом (Умножается переменная bonusPerSec)")]
    public int itemMultiplier;
    [Tooltip("Индекс товара, который будет управляться бонусом (Умножается переменная bonusPerSec этого товара)")]
    public int itemIndex;
    [Tooltip("Длительность бонуса")]
    public float timeOfBonus;
}

[Serializable]
public class Save
{
    public int score;
    public int scoret;
    public int scoreIncrease;
    public int tart;
    public int[] levelOfItem;
    public int[] bonusCounter;
    public int[] date = new int[6];
}
