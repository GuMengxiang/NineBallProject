using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Prime31;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;

public sealed class GameManager_script : MonoBehaviour
{
    public static GameManager_script instance;
    
    // cpu, fps and ram related stuff
    [System.NonSerialized]
    public int targetFrameRate = 60; // dajiang hack, should be 300 or 60 or 30???
    [System.NonSerialized]
    public float targetPingHigherLimit = 0.0f; // dajiang hack, this number is a variable depending on network speed
    [System.NonSerialized]
    public float networkConnectionTimer = 0.0f;

    // game table placement and stuff....
    public static float Table_Screen_Ratio = 0.85f;
    public static float Table_Self_Ratio = 1.83f;
    public static float Table_Bot_Top_Ratio = 8.0f; // top 8 pixels for every bot pixel
    public static float Top_Width_Ratio = 8.5f; // top side banner height cannot exceed 1/7.5 of total screen width 
    public float Top_Padding = 0.0f;
    public float Top_Gap = 0.0f;
    public float Mid_Gap = 0.0f;
    public float Bot_Gap = 0.0f;
    public float Side_Gap = 0.0f;

    public string connectionStatus = "         ";

    // player related stuff
    [System.NonSerialized]
    public string First_Name = "";
    [System.NonSerialized]
    public int Max_Name_Length = 12; //change this in GameManger gameobject

    // the other input related stuff
    [System.NonSerialized]
    public string SelectorFrdKey = "";
    [System.NonSerialized]
    public int Max_Key_Length = 30;

    [System.NonSerialized]
    public int Install_Day_Since_Beginning = 0;

    // input and popup focus stuff and hackz
    [System.NonSerialized]
    public bool isFrdsSelector = false; // dajiang hack, this whole isFrdsSelector thing is realllllly unity and realllllly fucked up
    [System.NonSerialized]
    public bool isEverythingFocusedOnFrdsSelector = false; // dajiang hack, this serves the double purpose of InputClick focus as well as differentiate between frd and online games
    [System.NonSerialized]
    public bool HackButtonShow = true; // dajiang hack, show hack button when true

    [System.NonSerialized]
    public bool InMenuAndAtMainPage = false;

    [System.NonSerialized]
    public bool currentlyPaused = false;

    [System.NonSerialized]
    public bool iOSPurchaseProductInfoReceived = false;
    [System.NonSerialized]
    public float iOSPurchaseProductInfoCounter = 0.0f;
    [System.NonSerialized]
    public float AndroidPurchaseProductInfoCounter = 0.0f;

    [System.NonSerialized]
    public float TotalWaitForRoomTime = 0.0f;
    [System.NonSerialized]
    public float TotalWaitForRoomTimeLimit = 0.0f; // dajiang hack, we will set this value later, either 18.0f or 24.0f
    [System.NonSerialized]
    public float TimeSpendPausingGame = 0.0f; // well ya
    [System.NonSerialized]
    public float TimeSpendPausingGameLimit = 11.25f; // if game paused for more than 10, gg no re
    [System.NonSerialized]
    public float TotalWaitForResponseTime = 11.25f; // if we wait these many seconds without getting any responses, we quit the game
    [System.NonSerialized]
    public float PutUpBannerAndWarnPlayerTime = 3.75f; // if we don't receive anything for 3 seconds or send anything for 3 seconds, tell them we are connecting
    [System.NonSerialized]
    public float TimerCap = 30.0f; // total 30 seconds a round
    [System.NonSerialized]
    public float ClockCap = 22.5f; // after 22.5 seconds, start playing clock music
    [System.NonSerialized]
    public float SkippableTimer = 5.0f; // fancy shiite dealing with lag, forgot what it does...

    // player experience related
    [System.NonSerialized]
	public float NineBallExperience; // total experience 总经验
    [System.NonSerialized]
    public float CurrentLevelExperience; // the left over experience for the current level
    [System.NonSerialized]
	public float CurrentLevel; // the current level starting at 1, not 0  当前级别从1开始，不为0
    public static float[] ExperienceIncreaseWin = { 20.0f, 30.0f, 50.0f, 75.0f, 100.0f, 150.0f, 200.0f, 250.0f, 500.0f, 1000.0f };
    public static float[] ExperienceIncreaseLoss = { 8.0f, 12.0f, 20.0f, 30.0f, 40.0f, 60.0f, 80.0f, 100.0f, 200.0f, 400.0f };
    public static float[] LevelExperienceCounts = { 
                                                      120.0f, 140.0f, 160.0f, 180.0f, 200.0f, 220.0f, 240.0f, 260.0f, 280.0f, 300.0f, // 20++
                                                      320.0f, 340.0f, 360.0f, 380.0f, 400.0f, 420.0f, 440.0f, 460.0f, 480.0f, 500.0f, // 20++
                                                      550.0f, 600.0f, 650.0f, 700.0f, 750.0f, 800.0f, 850.0f, 900.0f, 950.0f, 1000.0f, // 50++
                                                      1100.0f, 1200.0f, 1300.0f, 1400.0f, 1500.0f, 1600.0f, 1700.0f, 1800.0f, 1900.0f, 2000.0f, // 100++
                                                      2200.0f, 2400.0f, 2600.0f, 2800.0f, 3000.0f, 3200.0f, 3400.0f, 3600.0f, 3800.0f, 4000.0f, // 200++
                                                      4400.0f, 4800.0f, 5200.0f, 5600.0f, 6000.0f, 6400.0f, 6800.0f, 7200.0f, 7600.0f, 8000.0f, // 400++
                                                      8650.0f, 9300.0f, 9950.0f, 10600.0f, 11250.0f, 11900.0f, 12550.0f, 13200.0f, 13850.0f, 14500.0f, // 650++
                                                      15400.0f, 16300.0f, 17200.0f, 18100.0f, 19000.0f, 19900.0f, 20800.0f, 21700.0f, 22600.0f, 23500.0f, // 900++
                                                      24700.0f, 25900.0f, 27100.0f, 28300.0f, 29500.0f, 30700.0f, 31900.0f, 33100.0f, 34300.0f, 35500.0f, // 1200++
                                                      37000.0f, 38500.0f, 40000.0f, 41500.0f, 43000.0f, 44500.0f, 46000.0f, 47500.0f, 49000.0f, 52000.0f // 1500++
                                                  };

    // tracking levels, bronze, silver, gold, plat, diamond
    public static float[] SkillsLevel = { 0.40f, 0.60f, 0.80f };

    // all game tracking vars
    public float Total_Games_Played; // GameWonList.count for recent games
    public float Total_Games_Won; // GameWonList true counts
    public float Total_Chips_Lost; // ChipsWonList negatives
    public float Total_Chips_Won; // ChipsWonList positives
    public List<float> GameWonList; // 1 is win 0 is loss (not used in V1)
    public List<float> ChipsWonList;

    // streak
    public float Current_Win_Streak; // total's friend
    public float Current_Lose_Streak; // total's friend

    // tpa related stuff on all levels
    public List<float> BallsPottedList;
    public List<float> SnookeredSelfList;
    public List<float> MissShotsList;
    public List<float> ScratchList;
    public float Total_Balls_Potted; // this is the ONLY success condition (numerator)
    public float Total_Snookered_Self; // fail 1
    public float Total_Miss_Shots; // fail 2
    public float Total_Scratch; // fail 3

    public float P_One_Balls_Potted; // this is the ONLY success condition (numerator)
    public float P_One_Snookered_Self; // fail 1 still has control but fucked up the next shot
    public float P_One_Miss_Shots; // fail 2 not foul but has to transfer control
    public float P_One_Scratch; // fail 3 general foul

    public float P_Two_Balls_Potted; // this is the ONLY success condition (numerator)
    public float P_Two_Snookered_Self; // fail 1 still has control but fucked up the next shot
    public float P_Two_Miss_Shots; // fail 2 not foul but has to transfer control
    public float P_Two_Scratch; // fail 3 general foul

    [System.NonSerialized]
    public float LevelUpScreenTime = 3.0f;
    [System.NonSerialized]
    public float EndOfGameScreenTime = 15.0f; // dajiang hack, 15 seconds of this...
    [System.NonSerialized]
    public float EndOfFtueScreenTime = 0.0f;
    [System.NonSerialized]
    public float StartOfGameScreenTime = 0.0f;
    [System.NonSerialized]
    public float StartOfFtueScreenTime = 0.0f;
    [System.NonSerialized]
    public float EndOfGameWaitAndChangeTime = 0.0f;
    [System.NonSerialized]
    public float EndOfGameExitRematchHideTime = 1.5f;
    [System.NonSerialized]
    public float EndOfGameSmartBotDecideTime = 3.0f;
    [System.NonSerialized]
    public float EndOfGameSmartBotDecideTimeLong = 5.0f;

    public static string[][] statsButtonTextArray = 
    {
        new string [] { "", "WindowStatsPastGames", "WindowStatsAllTimes" },
        new string [] { "WindowStatsTPAScore", "", "" },
        new string [] { "WindowStatsBallsPotted", "", "" },
        new string [] { "WindowStatsBallsMissed", "", "" },
        new string [] { "WindowStatsOutOfPosition", "", "" },
        new string [] { "WindowStatsScratched", "", "" },
        new string [] { "", "WindowStatsPastGames", "WindowStatsAllTimes" },
        new string [] { "WindowStatsGamesWonLoss", "", "" },
        new string [] { "WindowStatsChipsWonLoss", "", "" },
        new string [] { "WindowStatsWinStreak", "", "" } // only thing not clearly defined in the arrayz
    };

    // smart bot flag that tries to determine if we should beat this player (given their mixed record with smart bots as well as human players)
    public static bool SmartBotIntendToWinGame = false;
    public static bool SmartBotIntendToMakeThisLongShot = false;
    public static float SmartBotMinDistanceForThisLongShot = 0.0f;
    public static float SmartBotDiagDistanceFactor = 0.0f;
    public List<float> SmartBotGameLoseList; // 1 is win 0 is loss
    public List<float> SmartBotLongShotTakenList; // we will randomly give a shot a "long shot" status (for all shots NOT given long shot status, we do a random and small tolerate level and make sure they go in)
    public List<float> SmartBotLongShotMadeList;

    // smart bot whatever
    [System.NonSerialized]
    public int SmartBotArrayLength = 12;

    // smart bot's overall mood on angry power shots
    public static float SmartBotAngryPowerShotPercentage = 0.0f; // chances we go all out straight shooting
    public static float SmartBotNormalPowerShotPercentage = 0.0f; // chances we go from required to all out shooting

    // smart bot's overall mood on quick shots
    public static bool SmartBotFastDrawShotMood = false;

    // smart bot single game stuff
    [System.NonSerialized]
    public float Smart_Bot_Single_Long_Shot_Taken = 0.0f;
    [System.NonSerialized]
    public float Smart_Bot_Single_Long_Shot_Made = 0.0f;

    // smart bot profiles
    public static float SmartBotTPAScore = 0.0f;
    public static int SmartBotLevel = 0;
    public static int SmartBotTotalGamesPlayed = 0;
    public static int SmartBotTotalGamesWon = 0;
    public static int SmartBotTotalStreak = 0;
    public static string[] SmartBotPlayerNameArray = 
    {
        "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name",
        "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name", "Player Name",
        "Player Name", "Galaxy", "Lost in city", "karry王", "Funker", "Fishbone", "Erstickt", "RoLL FoR Me", "J、ane丨you丶", "醋意浓浓", "~HeYの鉄鎷", "Crown prince", "伱是硪的のangel", "不三心°sunset", "女王Maxine彡", "Lillaby", "Pugss0912", "Cheveux tirés", 
        "我爱你", "Angel丨雅致", "cEncore LaLa", "Pobby", "Pinellia", "安辰兮", "Zenobia_1", "Tearl", "Kiwis", "じ☆veLOVE", "┾Ｓmile℡", "K.O.P", "9 Ball God", "SaKItama", "EXO-M╮咆哮", "┢┦aΡｐy✿baby", "FIy", "WoodenMonk", "Ginger Ginger", "⑦。", "Make❤NotWar",
        "H.H.H.", "Jacko", "Zelda", "Ironman 2", "Y.O.L.O.", "Pie n Mash", "Han Solo", "thewrongbro", "spid3yp3t3r2", "absman", "cool_cc_ro", "^(O_O)^", "Christorm", "CountryBall", "nofilter_99", "YoungNLethal", "jottaz1", "dorthancore", "thrasheyboy", "hammarby199", "Balls2walls", 
        "michiel0111", "Anonymous", "Yohnr", "stevenojobs", "Imabrownguy", "elnananov", "Da Eun", "Daeun", "Dahye", "Dong Hyun", "Eun Ji", "Heejin", "Hye Jin", "hye won", "Hyun Ji", "Jaehyun" , "Jiyoung", "Jongmin", "Seung Hyun", "Seungmin", "SO YEON", "So young", "SOHEE", "Sung min", "Yuna", 
        "Alyosha", "Anastas", "Anastasiy", "anastasiya", "Boleslava", "Borya", "Daniil", "Darya", "demyan", "Desya", "Dimitri", "Féodor", "Feodora", "Gennadiy", "gennadiya", "Kirill", "Léonid", "Lyubochka", "Lyubov", "Masha", "nastasia", "Nastya", "Nataliya", "Oleg", "Olya", "Rostislav", "Sergej",
        "Sergey", "Tatyana", "vadik", "Vadim", "Vasili", "Vasilisa", "Vasily", "Vitali", "Vlad", "Yegor", "Yevdokiya", "Yevgeniy", "yulia", "Nirmala", "Bagaskoro", "Bagus", "Bambang", "Budiarto", "Budiman", "Budiwati", "Bujana", "geni", "Gesang", "Gita", "Graha", "Gunadi", "Hamengku", "Harta", 
        "Harto", "harum", "kamboja", "Karna", "Karno", "Kartika", "Katon", "Kemala", "Kemuning", "Melati", "Mlathi", "Mulia", "Nakula", "Natalia", "Ndari", "Netro", "Niken", "Pertiwi", "Perwira", "Perwiro", "Ponco", "prakoso", "Pramana", "Aaron", "Adam", "Adam", "Adam", "Alan", "Albert", "Albert", "allen", "Andrew", 
        "Andy", "anthony", "Arthur", "August", "Barry", "Bart", "Ben", "Bill", "Bill", "billy", "Berto", "Bobby", "Brett", "brandon", "Bucky","Calvin", "Charles", "Charles", "christian", "Christopher", "Christopher", "Daniel", "Danny", "Dave", "dave", "Dennis", "Douglas", "Duncan", "Dwight", "Edward", "eddie", "Elliot", "Fabian", 
        "Felix", "Frank", "Gabriel", "Gabe", "gary", "George", "Gerald", "Glenn", "Gordon", "Greg", "Hamilton", "Harley", "Harold", "Harry", "Harvey", "Hayden", "Heather", "Heather", "Henry", "humphrey", "Hunter", "Ian", "Ivan", "Jack", "Jacob", "James", "jared", "Jason", "Jay", "Jeff", "Jeremy", "Jerome",
        "Jerry", "Jesse", "Jim", "john", "john", "John", "Johnny", "Johnny", "Jonathan", "Jonathan", "Joseph", "Joshua", "Josh", "josh", "Joyce", "justin", "Keith", "Kenneth", "Kelly", "Kent", "Kevin", "Kevin", "Kim", "kirk", "Kyle", "Larry", "Lawrence", "Len", "Leo", "lester", "Louis", "Lynn", "Malcolm", "Martin", "Meredith", "michael", 
        "Michael", "Michael", "Michael", "Mike", "Mikey", "Nathan", "Nathaniel", "Neil", "Nicholas", "Nick", "Nigel", "Omar", "Oscar", "patrick", "Patrick", "Patrick", "Paul", "Pete", "Peter", "Phil", "Philip", "Quincy", "DeShawn", "Raymond", "Richard", "Robert", "Robin", "Rodney", "Ryan", "sebastian", "Sidney", "Silvester", "Simon", 
        "Spencer", "Stan", "Stanley", "Steve", "Steve", "Steven", "steven", "Taylor", "Teddy", "Theo", "Tiffany", "Timmy", "Toby", "todd", "Tommy", "Tony", "Tracy", "Tyler", "Adeline", "Aimé", "Alain", "alexandre", "Amer", "Amon", "André", "Anna", "Antoine", "antony", "Aurélien", "Bastian", "Bernard", "Berry", 
        "Bertrand", "blois", "Bordeu", "Boulle", "Bouloun", "Brown", "Camille", "Cécile", "Cédric", "Christophe", "Claire", "Claude", "colette", "Cousin", "Cugnot", "Curie", "Dauphin", "Denis", "Didier", "Dorat", "Douai", "Elie", "emile", "Eric", "Florian", "Francis", "Franck", "François", "Frédéric",
        "Gaël", "Geneviève", "Gérard", "Gill", "Grégory", "Guilloux", "Henri", "Isabelle", "Jacquard", "Jarry", "Jasmin", "jean-luc", "Jean-Marc", "Jean-Pierre", "Jean-Yves", "Jordan", "Joseph", "Jullian", "Lami", "lara", "Laure", "Laurent", "Laurie", "Léon", "Louisa", "Manuel", "Maral", "Marc", "Marie", "Mars", 
        "maurice", "Maxim", "Metz", "michelle", "Monique", "Maxim", "Metz", "michelle", "Monique", "Nancy", "Nicolas", "Olivier", "Pascal", "Patrice", "Patricia", "Philippe", "Pierre", "Rémi", "Rolland", "Roman", "Rossi", "Sema", "Serge", "Sophie", "Sylvain", "Tatiana", "Thierry", "Victor", "Vincent", "Yoanne", "Zakia",
        "Abigail", "Adelaide", "Agatha", "Alberta", "alexia", "Alexia", "Alice", "Amanda", "Amy", "Anastasia", "Andrea", "Angela", "Anna", "annabelle", "Antonia", "April", "Audrey", "Barbara", "Bella", "Belle", "Betsy", "Betty", "betty", "Beverly", "Blake", "Bonnie", "Bridget", "Candice", "Carol", "catherine", "Catherine", 
        "Cecilia", "Celeste", "Charlotte", "Cherry", "cherry", "Cheryl", "Chloe", "Christine", "Claire", "Constance", "Crystal", "Cynthia", "cynthia", "Daisy", "Daisy", "Dana", "Debby", "Deborah", "Deirdre", "Denise", "Dinah", "Dominic", "Dorothy", "Elaine", "Elizabeth", "Elizabeth", "Elsa", "Emily", "Erica", "Erin", "Esther", 
        "Evelyn", "Faithe", "Flora", "florence", "Frances", "Frederica", "Gabrielle", "Gail", "Georgia", "Gloria", "Grace", "Hannah", "hannah", "Helen", "Hilary", "Iris", "Isabel", "Jacqueline", "Janet", "Jennifer", "Jennifer", "jennifer", "Jenny", "Jessica", "Jessica", "Jessica", "Joanna", "Josephine", "Judith", "June", "Karen", "Katherine", "Kitty", 
        "Laura", "Leila", "Linda", "Lindsey", "Lindsey", "Lisa", "Lucy", "Madeline", "maggie", "Maria", "Martha", "Maxine", "Melissa", "Michelle", "michelle", "Miranda", "Molly", "Monica", "natalie", "Natalie", "Penelope", "Penny", "Phoebe", "Phyllis", "rachel", "Rachel", "Rachel", "Rebecca", "renee", "Roxanne", "Sabina", "Sally", "Sally", 
        "Sabrina", "Samantha", "Sandy", "Sebastiane", "sherry", "Sherry", "shirley", "Sophie", "Stacey", "Stephanie", "Suzanne", "Sylvia", "Silvia", "Silvia", "Teresa", "Tiffany", "valentina", "Valerie", "Vanessa", "Veronica", "victoria1", "Victoria", "Vicky", "viola", "Wendy", "Zara", "Zoe", "Zoey", "艾伦", "曹颜帅", "陈晨 Derek", "陈达", 
        "崔一博", "丁萌思", "董亮晨", "哆啦卡丘^_^", "樊翠之", "付江春", "郭达阳", "何楚宇", "胡圆方", "胡明", "蒋华楠", "蓝棋", "雷帅", "李方昱", "李博", "李滨姿", "李蕊彤_李", "李雨轩 이", "刘嘉", "刘奇", "刘宴妮", "汤汤", "宋宇", "马宁", "纳迪", "吴华", "杨嘉", "于婷Alice", "张艺婷", "〇于京〇", "大棍", "三哥", "缩根", "少年犯", "李飞飞", "旅长", 
        "委月小鬼", "W. H. Wagner", "johann", "Gerhard", "Helmut", "Ludwig W.", "Adrian", "yannic", "Maximilian", "Leon", "Leonie", "johanna", "Gustave", "Marge B.", "Natavidad", "Yvonne", "Zenobia_1", "Johansson", "T. M.", "Longfellow", "Turner Turner", "MacAdams", "Zimmer-E'jokew", "Pinkman", "Homer", "annamaria", "Antonella", "Adalberto", 
        "Annibale", "assunta", "Adolfo", "ambrogio", "Agostino", "Alessandro", "Bernardo", "Benedetta", "Bartolomeo", "Beniamino", "Benedetto", "Carmelo", "carolina", "caterina", "costantino", "Cristina", "Cristiano", "Daniela", "Domenico", "Donatella", "Davide", "Damiano", "Ermanno", "Emiliana", "Eugenia", "Elisabetta", "Eugenio", "Emanuele", "Ezio",
        "Filippo", "Fabrizio", "federica", "Floriana", "Ferdinando", "Franca", "Francesca", "Giovanni", "giovanna", "Gabriella", "Graziella", "Giuliano", "Gianluigi", "Irene", "Ignazio", "Immacolata", "Lorenza", "Lorena", "Leonardo", "Ludovico", "Melissa", "Marcello", "Mariano", "Massimo", "natalina", "Nicoletta", "Omar", "Nicolò", "Noemi", 
        "Patrizia", "Pia", "Piero", "Roberto", "Rachele", "Rita", "Rossana", "riccardo", "Ruggero", "Silvana", "Sonia", "Salvatore", "sebastiano", "Stefano", "Valentina", "Vincenzo", "umberto", "Viviana", "Zenone", "Abdías", "Adalbert", "Agripina", "José", "Beneharo", "Jésus", "Alonzo", "Beltran", "cookie", "Albiñe", 
        "Karmele", "alejandro", "Juan", "Pedro", "Nadia", "Altzagarate", "Eberlein", "Engelbert", "Julia", "Markel", "Xosé", "Martina", "Mohamed", "ibrahim", "Salma", "Friedhelm", "Abd al-Wahab", "Abdallah", "Abdelaziz", "Assad", "Arslan", "Abdul-Latif", "Bakr", "Barzani", "Emir", "Ebrahim", "fakhrel", "Fakhruddin", "Faraj", "Fatima", "Fawwaz", 
        "Hajjaj", "Hamd", "Harithi", "Husain", "Kamal", "Mahmud", "Mohammad", "Mustafa", "Ismail", "nahayan", "Osman", "Rahman", "Said", "Shākir", "Ta'i", "Yusuf", "Zubair", "يلعلا دبع", "شابع", "هيمأ", "ةريمأ", "تمساب", "تمسب", "تيده", "تليمج", "Sahlah", "ىولس", "تفيرش", "Shādiyah", "Sumayyah", "wajihah", "Zāfīrah", "ناَدْجِو", "AbūBakr", 
        "Bashshār", "Hoshiko", "ichitarō", "Ikuyo", "Isoroku", "Itsuo", "Junji", "Kana", "Kazuho", "Kazuo", "kinuko", "Kisanta", "Marumi", "Masanori", "Mayo", "Michiko", "Mitsuo", "Muramasa", "Nakamura", "Natsuo", "nobuko", "Nobuo", "Norihito", "Rokurō", "Sachiko", "Sadao", "Shigekatsu", "Shigeki", "Shinji", "Shuntarō", "takahito", "Takami", 
        "Teruki", "Tomoe", "Tōson", "Tsuruyo", "Umeko", "Waichirō", "Yasunobu", "Yasuo", "Yōzō", "Yūji", "Yukihira", "Yukio", "Yumiko", "秋絵", "秋郎", "知恵子", "千鶴", "長次郎", "団十郎", "恵美子", "悦次郎", "富貴子", "房代", "元二朗", "和一郎", "泰信", "靖男", "洋三", "裕二", "行平", "幸夫", "由美子", "百合子", "Charisse", "Cheskka", "ching", "Chrislann",
        "Conrad", "Crystal", "Dominique", "Donna", "Edenjevy", "Eduardon", "Evarish", "Exequiel", "Ian Benedict", "Jan Martin", "Jan-Claude", "Jonelver", "Joramae", "Mackenzie", "Marilyn", "pearl-angel", "Monty", "Mykeey", "Ritzer", "Rodrigo", "Sheila", "Virgilio", "doobiesco", "stpredmore", "jodege", "duckylovr", "KnowYourRole", "rikkenybo", "oxy__moron", "TaraK", 
        "Bananica", "nxttguy5", "X_calimour", "countjacula", "rjblack01", "henias", "mymustache", "8bitwonder", "9 Baller", "fazoid", "GoatLife", "McAdams", "MacDonald", "A. Mansfield", "Marlowe", "Middleton", "Milton", "Needham","Newman", "O'Connor", "Orlando", "Philemon", "Pitman", "Pullman", "Richardson", "Rossetti", "Sailsbury", "Saroyan", "Simpson B.", "Susanna", 
        "Aaliyah", "Aaminah", "Abbas", "Abd-Hamid", "Abdul", "Apu", "Adamantia", "ADEL", "ALA", "Alberte", "ALEC", "Aleksander", "Alax", "Alex", "Alexandra", "Ali", "Alia", "Alina", "Aliya", "Alma", "Almaa", "Àlvaro", "ALYA", "Amani", "Amelie", "Amin", "Ana", "Anastasia", "Angelica", "Angus", "Abel", "Anne", "Antonio", "Atallah", "Atifa", "Aurora", "AyA",
        "Ayaz", "Aysima", "Azhar", "Bae", "bailey", "baki", "Basma", "Bassem", "Aylee", "Beatrice", "Benjamin", "Blanca", "blayne", "bora", "borat", "Bradley", "brakynn", "brayden", "Brenton", "Cale", "Callum", "Campbell", "CANSU", "Carla", "Carlos", "Isa", "iassaac", "Isla", "iZac", "JABBAR", "Jade", "Jaden", "Jaffer", "jake", "Jamil", "Javier", "Jayden", "Jeon",
        "Caroline", "CASEY", "KC", "Chan", "Chase", "Cian", "Claudia", "Clement", "Connor", "Conor", "corey", "cruz", "Da Eun", "Daiki", "Dale", "Damian", "dante", "Declan", "Dinis", "Dionysos", "DJAMILA", "Logan", "Lola", "Lotte", "Loukas", "Louna", "LUCA", "Lucas", "Lucie", "Luis", "Louis", "Luker", "LULU", "MACKINLEY",
        "Dr. Dre", "Duarte", "Duka", "Ed", "Edward", "Eliou", "Elisa", "Ella", "Elliot", "Emilia", "Emma", "ENSAR", "ERAY", "Erin", "ESMAIL", "Ethan", "Eun", "Euthalia", "Evangelia", "Evie", "Maxwell", "Mazin", "Mia", "Miho", "Min", "Min Ji", "minji", "MinSu", "Mitch", "Mohamed", "MONA", "Moon", "Morrison",
        "Federico", "Femke", "Fidda", "Filipa", "Finn", "Finnegan", "firdos", "Fleur", "Flyn", "Francisco", "Fraser", "Freya", "Galal", "Galila", "Gamila", "Germain", "Georgia", "Giuseppe", "Gizem", "Gonzalo", "GUS", "Helios", "Hippo", "Hong", "Hugo", "HÜMEYRA", "Husayn", "Hyder", "Hyejin", "hyewon", "hyunji", "Ibrahim", "Igor", "Ikram", "Ilyas", "Imram",
        "Hadia", "Hadiya", "Hafsa", "Hafza", "Haifa", "Hang", "Hana", "Hanan", "Hansol", "harrison", "Hasan", "Hassan", "Havin", "Hawa", "Hayder", "Heath", "MAÏSSA", "Markham", "Malaika", "Marco", "Mariana", "Mark", "Markos", "Marta", "Maryam", "Mason", "Matheo", "Mathis", "Matilde", "Matteo", "Matthias", "Max", 
        "Ji", "Ji eun", "Ji Hyun", "ji yeon", "jieun", "jihoon", "jihye", "Jimin", "Jin", "JINAN", "Jisu", "Jiyeon", "Jonas", "Joo", "Joon", "Josefine", "Jules", "Julie", "Jung", "Kader", "Kaelan", "Kallias", "Kamil", "Kane", "Usman", "Valerian", "Vassa", "Viktor", "Viktoria", "Viper", "Vlada", "Vladmir", "Vlass", "Wafai", "WALDO",
        "Kate", "Katie", "KEN", "kiplin", "Konstantina", "Kwon", "Lamia", "Latifa", "El Barto", "Léa", "Leandros", "Lee", "Lena", "Léna", "Lewis", "Liam", "Lila", "Lilou", "Lina", "liva", "Saki", "Salah", "Samuel", "Samuel", "SANA", "Sang min", "SANI", "SANIYYA", "SANYA", "SARA", "Sarah", "SATOSHI", "SAYYIDA", "Sean", "SELIMA", "SEMIH", "Seo", "Sergio",
        "Mustapha", "Mutaism", "NadaNada", "Nader", "Nadir", "Nadya", "Nigel", "Naima", "Naji", "Nasira", "Nate", "Natsumi", "Nawra", "Nele", "NICOLAAAAS", "Niklas", "Suyeon", "Tait", "TAJ", "Tayyib", "TekanMan", "Tess", "Thomas", "Thompson", "Tim", "Tobias", "Tom", "Tommaso", "Travis", "Tristan", "Tugba", "Kumar",
        "Nizar", "Noa", "Noah", "Noam", "Nora", "Pacey", "Paula", "Rafael", "R.J.", "RamenDude", "Randa", "Ranya", "Raphael", "Rashed", "Rashid", "rashida", "rayan", "Reece", "Romane", "Ruba", "Ryan", "Ryo", "Ryu", "Sacha", "Sadia", "S*A*F*I*Y*A", "Sahar", "Zaid", "Zaina", "Zak", "Zaki", "Zakiyya", "Zayd", "Zayna", "zia", "Ziya", "Zubaida", "Zulekha", "Zulfaqar", "zulfiqar", 
        "Seth", "Severin", "Seyma", "Shaq", "Shakira", "SHERIFFz", "Shim", "Shukriya", "Shula", "Shun", "sims4", "Simone", "SOAN", "Sofia", "Sophia", "Sora", "soyoung", "Stephan", "Steven", "Su Jin", "Suha", "SUHAIL", "Suji", "yoon", "Younes", "yu", "Yulu", "Yuki", "yuri", "Yusef", "Yusra", "Zaahir", "Zachary", "Zahia", "Zahid", "zahira", "ZAHRAH",
        "Wasiri", "Wassim", "Will", "wong", "Woojin", "Xavier", "Yahya", "Yamato", "anis", "Yasemin", "Yasin", "YASMIN", "Yasser", "Yeji", "小铁", "谢宏伟", "翟帅", "肖和平", "李方华", "董大成", "赵亮亮", "方敬一", "开心果", "小红帽", "鱼雷", "章丫丫", "悠悠", "东雨晨", "孙畅", "江辉辉", "张光仪", "宋怀远"
    };
    
    // bots profiles.....
    public static int[] BotHeadImageArray = { 72, 73, 74, 75, 76, 77, 78, 79, 80, 81 };
    public static string[] BotPlayerNameArray = { "Robot #1", "Robot #2", "Robot #3", "Robot #4", "Robot #5", "Robot #6", "Robot #7", "Robot #8", "Robot #9", "Robot #10" };
    public static int[] BotCueEquippedArray = { 43, 20, 19, 11, 31, 47, 3, 26, 13, 24 };
    public static float[] BotTPAScoreArray = { 67.4f, 81.2f, 78.0f, 45.6f, 54.7f, 66.9f, 38.1f, 78.2f, 99.1f, 54.0f };
    public static int[] BotStarLevelArray = { 55, 75, 27, 14, 45, 23, 5, 33, 2, 57 };
    public static int[] StupidBotInActionGamesPlayedArray = { 1265, 4330, 342, 106, 450, 330, 22, 175, 6, 1898 };
    public static int[] StupidBotInActionGamesWonArray = { 658, 2524, 183, 52, 281, 172, 10, 117, 3, 996 };
    public static int[] StupidBotInActionGamesCoinsArray = { 54730, 340200, 2720, 880, 12400, 23900, 45, 7520, 360, 6000 };
    public static int[] StupidBotInActionGamesStreakArray = { 4, 12, 0, 2, 3, 10, 6, 5, 0, 17 };

    // disconnection related stuff, if we are currently in any sort of network gamez
    [System.NonSerialized]
    public bool NetworkGameSceneCurrentLoad = false;

    // game type
    [System.NonSerialized]
    public bool StartingOutAsANetWorkGame = false; // this includes an actual game with a network opponent and a "network" game with a robot
    [System.NonSerialized]
    public bool CurrentlyInANetWorkGame = false; // this is true when we are in a real network game as in there is an ACTUAL opponent playing on the other side
    [System.NonSerialized]
    public bool SmartBotInActionGame = false; // this is true when a smart robot attempts to play a game with an opponent
    [System.NonSerialized]
    public bool StupidBotInActionGame = false; // this can be true when we are in a real bot game
    [System.NonSerialized]
    public bool TrulySelfInActionGame = false; // this only happens when we are true solo
    [System.NonSerialized]
    public bool FTUEInActionGame = false; // this only happens when we are true ftue

    // language lolz
    [System.NonSerialized]
    public string Savelanguage = "";

    // coin vars
    [System.NonSerialized]
    public float CoinCount;
    [System.NonSerialized]
    public float CurrentWager;
    [System.NonSerialized]
    public float CurrentWagerLevel;

    // bot and solo coin stake
    [System.NonSerialized]
    public float StupidBotCoinWager = 40.0f;
    [System.NonSerialized]
    public float SoloCoinWager = 20.0f;

    // var that indicates if players ever loaded a v2 game before (first time will be a long wait).
    [System.NonSerialized]
    public float NeverLoadedGameBefore = 0.0f;

    // FTUE stages, -1 is wait a few seconds before 0, 0 is hide slider and swipe big, 1 is hide slider and swipe small, 2 is show slider and make sure they shoot out
    [System.NonSerialized]
    public int SwipeFtueStage = -1;

    // FTUE related stuffz
    [System.NonSerialized]
    public float SeenSwipeAndPullEver = 0.0f; // the actual mini game that player has to play
    [System.NonSerialized]
    public float SeenNetworkGameFlagEver = 0.0f; // the arrow pointing to "Play Online" button
    
    // all the other ever stuffz
    [System.NonSerialized]
    public float MadePurchaseEver = 0.0f; // 0 is no, 1 is yes
    [System.NonSerialized]
    public float SetGameCoinEver = 0.0f;
    [System.NonSerialized]
    public float ClaimedDailyBonusEver = 0.0f;

    // claim daily stuff
    public float CurrentBonusAmount = 0.0f;

    // a change of EV for Chinese specific market
    public static float[] DailyBonusArray = 
    {
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 
        800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 
        800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 
        1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 
        1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 
    };

    // world market EV
    /*
    public static float[] DailyBonusArray = 
    {
        100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 
        100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 
        100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 
        100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 
        100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 100.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 200.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 400.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 600.0f, 
        800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 
        800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 
        800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 800.0f, 
        1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 
        1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 1000.0f, 
        1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 1500.0f, 
        3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 3000.0f, 
        5000.0f, 5000.0f, 5000.0f, 5000.0f, 5000.0f, 5000.0f, 5000.0f, 5000.0f, 5000.0f, 5000.0f, 
        10000.0f, 10000.0f, 10000.0f, 10000.0f, 10000.0f, 
        20000.0f, 20000.0f, 20000.0f, 20000.0f, 
        50000.0f, 50000.0f, 50000.0f, 
        100000.0f, 100000.0f, 
        250000.0f
    };
    */

    // coin levels
    public static float[] WagerLevels = { 20.0f, 100.0f, 400.0f, 2000.0f, 10000.0f, 40000.0f, 100000.0f, 200000.0f, 400000.0f, 1000000.0f };

    // button managing states, dajiang hack, i really need to stop this....
    public bool DownOnRealButtons = false;
    public bool CanControlCue = false;

    // UI layer 
    public static int UIMenuLayerStart = 200; // this is set separately in the prefabs
    public static int UIPopupLayerStart = 600; // this is set separately in the prefabs
    public static int UIConfirmLayerStart = 1000; // this is set separately in the prefabs
    public static int UILayerIncrementStep = 10;

    // keep depth in order, pretty accurate
    public List<GameObject> MenuVisibilityStack; // well, menu visibility stack

    // keep depth in order, very cheap, we only allow one popup to open at any given time anywayz
    [System.NonSerialized]
    public bool PopupCurrentlyVisible = false;

    public AudioListener mListener;
    public AudioSource mAudioSource;
    public AudioClip[] clips;
    public GameObject AudioPrefab;

    [System.NonSerialized]
    public int TableTextureIndex = 1; // this thing starts at 1 just like pocketcontroller

    // 7 elements and 1st one is zero
    public static Vector3[] RobotHolePositions = { Vector3.zero, // this is just a filler, lolz
                                                     new Vector3(0.2f, 4.0f, -12.1f), // 1 mid pocket
                                                     new Vector3(19.0f, 4.0f, 6.45f),
                                                     new Vector3(19.0f, 4.0f, -11.65f), // 3
                                                     new Vector3(0.2f, 4.0f, 6.95f), // 4 mid pocket
                                                     new Vector3(-18.65f, 4.0f, -11.65f),
                                                     new Vector3(-18.65f, 4.0f, 6.45f) // 6
                                                 };
    [System.NonSerialized]
    public static float DiagDistance = 41.7748f; // (GameManager_script.RobotHolePositions[2] - GameManager_script.RobotHolePositions[5]).magnitude;

    [System.NonSerialized]
    public string Ginger_ID = "";

    [System.NonSerialized]
    public static string versionNumber = ""; // agree with Android.xml
    [System.NonSerialized]
    public static string marketNumber = "";
    [System.NonSerialized]
    public static string platformNumber = "";

    [System.NonSerialized]
    public float parsePeriodicPingCounter = -61.0f; // 0.0f, dajiang hack, this is to ensure we immediately fire off a signal

    [System.NonSerialized]
    public SettingInfo vSettingInfo = null;

    [System.NonSerialized]
    public UIinterstitialWindowInfo interstitialPageInfo = null;
    [System.NonSerialized]
    public ProfilePopupInfo selfGameProfileInfo = null;
    [System.NonSerialized]
    public ProfilePopupInfo otherGameProfileInfo = null;

    // confirmation updates, i am too lazy to actually pass in the actual prefab and keep track of multiple classes of prefabs
    public static ConfirmationType ConfirmationState = ConfirmationType.undecided;
    public static ConfirmationType PushOutConfirmState = ConfirmationType.undecided;

    // last time daily bonus is opened
    public float LastTimeDailyBonusOpened;

    // last time daily deal was calculated
    public float LastDayDailyDealCalculated;

    // info structs...
    public AvatarStructInfo AvatarInfo;
    public UserStructInfo UserInfo;
    public ProfileStructInfo ProfileInfo;
    public SettingStructInfo SettingInfo;
    public ShopMainStructInfo ShopMainInfo;
    public CoinStructInfo CoinInfo;
    public CueStructInfo CueInfo;
    public StatsStructInfo StatsInfo;

    // uicenter and uibottomright
    public UICenter uiCenter;
    public UIBottomRight uiBottomRight;
    public GameRoot gameRoot;

    // sound and vibration enabler
    [System.NonSerialized]
    public string SoundEnabled = "e"; // "d"
    [System.NonSerialized]
    public string VibeEnabled = "d"; // "e"

    // coin button stuff
    [System.NonSerialized]
    public static float[] ImageRatio = { 1.00f, 0.76f, 0.92f, 0.68f, 0.84f, 0.60f };
    [System.NonSerialized]
    public static float[] CoinCounts = { 1000000, 40000, 350000, 15000, 100000, 6000 };
    [System.NonSerialized]
    public static float[] MoneyCounts = { 99.99f, 9.99f, 49.99f, 4.99f, 19.99f, 1.99f };
    [System.NonSerialized]
    public static string[] AndroidPurchasableItems = { "chippack6", "chippack3", "chippack5", "chippack2", "chippack4", "chippack1", "newdeal1", "newdeal2", "newdeal3", "newdeal4", "newdeal5" }; // Items available for sale, array of your ProductID's
    [System.NonSerialized]
    public static string[] IPhonePurchasableItems = { "chippack6", "chippack3", "chippack5", "chippack2", "chippack4", "chippack1", "newdeal1", "newdeal2", "newdeal3", "newdeal4", "newdeal5" }; // Items available for sale, array of your ProductID's

    // flag for if we have a new daily deal, based on some complicated algorithms....
    [System.NonSerialized]
    public int newDailyDealPrStarterPackageSeen = 0; // 0 is false 1 is true
    [System.NonSerialized]
    public float[] DailyDealPopupInfoSave; // is starter(1, 2, 3, 4, 5 stands for the 5 packages), cue included, coins included, coins bonus included, actual price (0.99 ~ 4.99?)

    // some avatar keepers
    public int AvatarEquipped = 0;
    public bool[] AvatarOwned;
    public bool[] AvatarNew;

    public static float[][] AvatarAttributes = // level lock starts at 1, not 0
    {
        new float []{1, 0}, new float []{1, 0}, new float []{1, 0}, 
        new float []{2, 0}, new float []{2, 0}, new float []{2, 0}, 
        new float []{3, 100}, new float []{3, 100}, new float []{3, 100}, 
        new float []{5, 200}, new float []{5, 200}, new float []{5, 200}, 
        new float []{10, 400}, new float []{10, 400}, new float []{10, 400}, 
        new float []{15, 600}, new float []{15, 600}, new float []{15, 600}, 
        new float []{20, 800}, new float []{20, 800}, new float []{20, 800}, 
        new float []{25, 1000}, new float []{25, 1000}, new float []{25, 1000}, 
        new float []{30, 2000}, new float []{30, 2000}, new float []{30, 2000}, 
        new float []{35, 4000}, new float []{35, 4000}, new float []{35, 4000}, 
        new float []{40, 6000}, new float []{40, 6000}, new float []{40, 6000}, 
        new float []{45, 8000}, new float []{45, 8000}, new float []{45, 8000}, 
        new float []{50, 10000}, new float []{50, 10000}, new float []{50, 10000}, 
        new float []{55, 20000}, new float []{55, 20000}, new float []{55, 20000}, 
        new float []{60, 30000}, new float []{60, 30000}, new float []{60, 30000},
        new float []{65, 40000}, new float []{65, 40000}, new float []{65, 40000}, 
        new float []{70, 50000}, new float []{70, 50000}, new float []{70, 50000},
        new float []{75, 60000}, new float []{75, 60000}, new float []{75, 60000}, 
        new float []{80, 70000}, new float []{80, 70000}, new float []{80, 70000},
        new float []{85, 80000}, new float []{85, 80000}, new float []{85, 80000}, 
        new float []{90, 90000}, new float []{90, 90000}, new float []{90, 90000},
        new float []{95, 100000}, new float []{95, 100000}, new float []{95, 100000}, 
        new float []{100, 200000}, new float []{100, 200000}, new float []{100, 200000}, 
        new float []{100, 200000}, new float []{100, 200000}, new float []{100, 500000}
    };

    // all cue related stuff starts here?
    // some avatar keepers
    public int CueEquipped = 0;
    public bool[] CueOwned;
    public bool[] CueNew;

    // everything about cue
    // bronze, silver, gold, plat, diamond
    // Spins touched, 18% spin, 36% spin, 54% spin, 72%, 90% spin (maximum line at 71.4%)
    // Power, 23M (180.2), 25M (195.9), 27M (211.5), 30M (235.0), 32M (250.7), 35M (274.2)
    // Extension line, 1.0, 1.5, 2.0, 2.5, 3.0
    // -------------------------------------------
    // spins can go from around 50% to a full 100%
    // extension can go from 1.44 to 2.88 (2.75 inches to 5.5 inches)
    // speed can go from 117.5f to 235.0f (13 MPH to 26 MPH)

    public static float[][] CueAttributes = // level locks start at 1, not 0 (steps 0.0288, 2.35, 0.01)
    {
		new float []{1, 0, 1.44f, 117.5f, 0.500f}, // worst case, anchor overall
		new float []{1, 100, 1.53f, 120.5f, 0.521f}, // good extension
		new float []{1, 100, 1.49f, 124.6f, 0.512f}, // good speed
		new float []{2, 250, 1.48f, 121.1f, 0.540f}, // good spin 1.53, 124.6, 0.540
		new float []{2, 250, 1.61f, 125.8f, 0.554f}, // good extension
		new float []{3, 500, 1.55f, 131.6f, 0.560f}, // good speed
		new float []{3, 500, 1.57f, 128.3f, 0.570f}, // good spin 1.61, 131.6, 0.570
		new float []{4, 750, 1.69f, 133.5f, 0.582f}, // good extension
		new float []{4, 750, 1.65f, 136.2f, 0.600f}, // good spin
		new float []{5, 1000, 1.64f, 138.7f, 0.575f}, // good speed 1.69, 138.7, 0.600
		new float []{5, 1000, 1.72f, 140.3f, 0.630f}, // good spin
		new float []{10, 2500, 1.81f, 145.7f, 0.626f}, // good speed
		new float []{10, 2500, 1.80f, 147.1f, 0.610f}, // good extension 1.79, 145.7, 0.630
		new float []{15, 5000, 1.81f, 146.9f, 0.625f}, // 1 quarter case, anchor overall
		new float []{15, 5000, 1.87f, 148.5f, 0.633f}, // good extension
		new float []{20, 7500, 1.83f, 152.8f, 0.645f}, // good speed
		new float []{20, 7500, 1.85f, 150.2f, 0.660f}, // good spin 1.87, 152.8, 0.660
		new float []{25, 10000, 1.88f, 159.8f, 0.665f}, // good speed
		new float []{25, 10000, 1.96f, 153.2f, 0.666f}, // good extension
		new float []{30, 25000, 1.93f, 156.7f, 0.690f}, // good spin 1.96, 159.8, 0.690
		new float []{30, 25000, 2.04f, 163.1f, 0.711f}, // good extension
		new float []{35, 50000, 2.01f, 160.4f, 0.720f}, // good spin
		new float []{35, 50000, 2.00f, 166.9f, 0.702f}, // good speed 2.04, 166.9, 0.720
		new float []{40, 75000, 2.17f, 173.0f, 0.748f}, // good extension
		new float []{40, 75000, 2.09f, 176.4f, 0.728f}, // good speed 
		new float []{45, 100000, 2.16f, 176.3f, 0.730f}, // mid case, anchor overall
		new float []{45, 100000, 2.17f, 171.0f, 0.750f}, // good spin 2.13, 173.9, 0.750
		new float []{50, 200000, 2.18f, 181.0f, 0.760f}, // good speed
		new float []{50, 200000, 2.22f, 177.2f, 0.773f}, // good extension
		new float []{55, 300000, 2.17f, 174.9f, 0.780f}, // good spin 2.22, 181.0, 0.780
		new float []{55, 300000, 2.30f, 183.4f, 0.789f}, // good extension
		new float []{60, 400000, 2.27f, 188.0f, 0.800f}, // good speed
		new float []{60, 400000, 2.24f, 185.6f, 0.810f}, // good spin 2.30, 188.0, 0.810
		new float []{65, 500000, 2.39f, 192.0f, 0.830f}, // good extension
		new float []{65, 500000, 2.32f, 195.1f, 0.820f}, // good speed
		new float []{70, 600000, 2.36f, 194.0f, 0.840f}, // good spin 2.39, 195.1, 0.840
		new float []{70, 600000, 2.41f, 202.1f, 0.848f}, // good speed
		new float []{75, 700000, 2.42f, 203.7f, 0.865f}, // 3 quarter case, anchor overall, 2.52f, 205.7f, 0.880f
		new float []{75, 700000, 2.47f, 199.2f, 0.870f}, // good spin
		new float []{80, 800000, 2.48f, 203.2f, 0.863f}, // good extension 2.48, 202.1, 0.870
		new float []{80, 800000, 2.52f, 203.4f, 0.900f}, // good spin
		new float []{85, 900000, 2.57f, 204.9f, 0.878f}, // good extension
		new float []{85, 900000, 2.54f, 209.2f, 0.889f}, // good speed 2.57, 209.2, 0.900
		new float []{90, 1000000, 2.66f, 214.0f, 0.910f}, // good extension
		new float []{90, 1000000, 2.59f, 211.0f, 0.930f}, // good spin
		new float []{95, 2000000, 2.63f, 216.3f, 0.920f}, // good speed 2.66, 216.3, 0.930
		new float []{95, 2000000, 2.68f, 223.4f, 0.955f}, // good speed
		new float []{100, 3000000, 2.75f, 220.8f, 0.945f}, // good extension
		new float []{100, 3000000, 2.71f, 218.0f, 0.960f}, // good spin 2.75, 223.4, 0.960  100, 7500000,
		new float []{100, 3000000, 2.87f, 229.0f, 0.980f}, // good extension
		new float []{100, 5000000, 2.88f, 235.0f, 1.000f} // best case, anchor overall

//        new float []{1, 0, 1.44f, 117.5f, 0.500f}, // worst case, anchor overall
//        new float []{5, 100, 1.53f, 120.5f, 0.521f}, // good extension
//        new float []{10, 100, 1.49f, 124.6f, 0.512f}, // good speed
//        new float []{15, 250, 1.48f, 121.1f, 0.540f}, // good spin 1.53, 124.6, 0.540
//        new float []{20, 250, 1.61f, 125.8f, 0.554f}, // good extension
//        new float []{25, 500, 1.55f, 131.6f, 0.560f}, // good speed
//        new float []{30, 500, 1.57f, 128.3f, 0.570f}, // good spin 1.61, 131.6, 0.570
//        new float []{35, 750, 1.69f, 133.5f, 0.582f}, // good extension
//        new float []{40, 750, 1.65f, 136.2f, 0.600f}, // good spin
//        new float []{45, 1000, 1.64f, 138.7f, 0.575f}, // good speed 1.69, 138.7, 0.600
//        new float []{50, 1000, 1.72f, 140.3f, 0.630f}, // good spin
//        new float []{55, 2500, 1.81f, 145.7f, 0.626f}, // good speed
//        new float []{60, 2500, 1.80f, 147.1f, 0.610f}, // good extension 1.79, 145.7, 0.630
//        new float []{65, 5000, 1.81f, 146.9f, 0.625f}, // 1 quarter case, anchor overall
//        new float []{70, 5000, 1.87f, 148.5f, 0.633f}, // good extension
//        new float []{75, 7500, 1.83f, 152.8f, 0.645f}, // good speed
//        new float []{80, 7500, 1.85f, 150.2f, 0.660f}, // good spin 1.87, 152.8, 0.660
//        new float []{85, 10000, 1.88f, 159.8f, 0.665f}, // good speed
//        new float []{90, 10000, 1.96f, 153.2f, 0.666f}, // good extension
//        new float []{95, 25000, 1.93f, 156.7f, 0.690f}, // good spin 1.96, 159.8, 0.690
//        new float []{-1, 9.99f, 2.04f, 163.1f, 0.711f}, // good extension
//        new float []{-1, 9.99f, 2.01f, 160.4f, 0.720f}, // good spin
//        new float []{-1, 9.99f, 2.00f, 166.9f, 0.702f}, // good speed 2.04, 166.9, 0.720
//        new float []{-1, 9.99f, 2.17f, 173.0f, 0.748f}, // good extension
//        new float []{-1, 9.99f, 2.09f, 176.4f, 0.728f}, // good speed
//        new float []{-1, 9.99f, 2.16f, 176.3f, 0.730f}, // mid case, anchor overall
//        new float []{-1, 9.99f, 2.17f, 171.0f, 0.750f}, // good spin 2.13, 173.9, 0.750
//        new float []{-1, 9.99f, 2.18f, 181.0f, 0.760f}, // good speed
//        new float []{-1, 9.99f, 2.22f, 177.2f, 0.773f}, // good extension
//        new float []{-1, 9.99f, 2.17f, 174.9f, 0.780f}, // good spin 2.22, 181.0, 0.780
//        new float []{-1, 9.99f, 2.30f, 183.4f, 0.789f}, // good extension
//        new float []{-1, 9.99f, 2.27f, 188.0f, 0.800f}, // good speed
//        new float []{-1, 9.99f, 2.24f, 185.6f, 0.810f}, // good spin 2.30, 188.0, 0.810
//        new float []{-1, 19.99f, 2.39f, 192.0f, 0.830f}, // good extension
//        new float []{-1, 19.99f, 2.32f, 195.1f, 0.820f}, // good speed
//        new float []{-1, 19.99f, 2.36f, 194.0f, 0.840f}, // good spin 2.39, 195.1, 0.840
//        new float []{-1, 19.99f, 2.41f, 202.1f, 0.848f}, // good speed
//        new float []{-1, 19.99f, 2.42f, 203.7f, 0.865f}, // 3 quarter case, anchor overall, 2.52f, 205.7f, 0.880f
//        new float []{-1, 19.99f, 2.47f, 199.2f, 0.870f}, // good spin
//        new float []{-1, 19.99f, 2.48f, 203.2f, 0.863f}, // good extension 2.48, 202.1, 0.870
//        new float []{-1, 19.99f, 2.52f, 203.4f, 0.900f}, // good spin
//        new float []{-1, 29.99f, 2.57f, 204.9f, 0.878f}, // good extension
//        new float []{-1, 39.99f, 2.54f, 209.2f, 0.889f}, // good speed 2.57, 209.2, 0.900
//        new float []{-1, 49.99f, 2.66f, 214.0f, 0.910f}, // good extension
//        new float []{-1, 59.99f, 2.59f, 211.0f, 0.930f}, // good spin
//        new float []{-1, 69.99f, 2.63f, 216.3f, 0.920f}, // good speed 2.66, 216.3, 0.930
//        new float []{-1, 79.99f, 2.68f, 223.4f, 0.955f}, // good speed
//        new float []{-1, 89.99f, 2.75f, 220.8f, 0.945f}, // good extension
//        new float []{-1, 99.99f, 2.71f, 218.0f, 0.960f}, // good spin 2.75, 223.4, 0.960  100, 7500000,
//        new float []{-1, 888.88f, 2.87f, 229.0f, 0.980f}, // good extension
//        new float []{-1, 999.99f, 2.88f, 235.0f, 1.000f} // best case, anchor overall
    };

    public static string[] CueNames = 
    {
        "cuename0", "cuename1", "cuename2", "cuename3", "cuename4", "cuename5", "cuename6", "cuename7", "cuename8", "cuename9", 
        "cuename10", "cuename11", "cuename12", "cuename13", "cuename14", "cuename15", "cuename16", "cuename17", "cuename18", "cuename19", 
        "cuename20", "cuename21", "cuename22", "cuename23", "cuename24", "cuename25", "cuename26", "cuename27", "cuename28", "cuename29",
        "cuename30", "cuename31", "cuename32", "cuename33", "cuename34", "cuename35", "cuename36", "cuename37", "cuename38", "cuename39",
        "cuename40", "cuename41", "cuename42", "cuename43", "cuename44", "cuename45", "cuename46", "cuename47", "cuename48", "cuename49",
        "cuename50"
    };

    // level military rank
    public static string[] MilitaryRankName = 
    {
        "", // 0th is moot, level starts at 1
        "Private", "Private", "Private", "Private", "Private", 
        "Private 1st Class", "Private 1st Class", "Private 1st Class", "Private 1st Class", "Private 1st Class", 
        "Corporal", "Corporal", "Corporal", "Corporal", "Corporal", 
        "Sergeant", "Sergeant", "Sergeant", "Sergeant", "Sergeant", 
        "Staff Sergeant", "Staff Sergeant", "Staff Sergeant", "Staff Sergeant", "Staff Sergeant", 
        "Sergeant 1st Class", "Sergeant 1st Class", "Sergeant 1st Class", "Sergeant 1st Class", "Sergeant 1st Class", 
        "2nd Lieutenant", "2nd Lieutenant", "2nd Lieutenant", "2nd Lieutenant", "2nd Lieutenant", 
        "Lieutenant", "Lieutenant", "Lieutenant", "Lieutenant", "Lieutenant", 
        "Captain", "Captain", "Captain", "Captain", "Captain", 
        "Major", "Major", "Major", "Major", "Major", 
        "Lieutenant Colonel", "Lieutenant Colonel", "Lieutenant Colonel", "Lieutenant Colonel", "Lieutenant Colonel", 
        "Colonel", "Colonel", "Colonel", "Colonel", "Colonel", 
        "Brigadier General", "Brigadier General", "Brigadier General", "Brigadier General", "Brigadier General", 
        "Major General", "Major General", "Major General", "Major General", "Major General", 
        "Lieutenant General", "Lieutenant General", "Lieutenant General", "Lieutenant General", "Lieutenant General", 
        "General", "General", "General", "General", "General", 
        "Generalissimo", "Generalissimo", "Generalissimo", "Generalissimo", "Generalissimo", 
        "Marshall", "Marshall", "Marshall", "Marshall", "Marshall", 
        "Grand Marshall", "Grand Marshall", "Grand Marshall", "Grand Marshall", "Grand Marshall", 
        "King of 9 ball", "King of 9 ball", "King of 9 ball", "King of 9 ball", "King of 9 ball", 
        "God of 9 ball", "God of 9 ball", "God of 9 ball", "God of 9 ball", "God of 9 ball"
    };

    // this is for all the small and big stars, used in most cases
    public static string[] StarRankName = { "Bronze", "Silver", "Gold", "Red", "Bronze", "Silver", "Gold", "Red" };

    // this is the image name for the smaller stars and only used for cue buttons right now
    public static string[] CueRankName = { "bronze", "silver", "gold", "red" };

    // ngui pop tween curves
    public AnimationCurve NGUITweenPosition;
    public AnimationCurve NGUITweenScale;

    // finding people long music and other musics that will be played between different scenes
    public GameObject LookForPeopleMusic;

    // in game clock music
    public GameObject clockMusic;

    // placeholder curve
    public AnimationCurve PlaceHolder0;

    // slow and short curves
    public AnimationCurve Smooth0; public AnimationCurve Smooth1; public AnimationCurve Smooth2; public AnimationCurve Smooth3; public AnimationCurve Smooth4;

    // random and piece meal
    public AnimationCurve Random0; public AnimationCurve Random1; public AnimationCurve Random2; public AnimationCurve Random3; public AnimationCurve Random4;
    public AnimationCurve Random5; public AnimationCurve Random6; public AnimationCurve Random7; public AnimationCurve Random8; public AnimationCurve Random9;
    public AnimationCurve Random10; public AnimationCurve Random11; public AnimationCurve Random12; public AnimationCurve Random13; public AnimationCurve Random14;
    public AnimationCurve Random15; public AnimationCurve Random16; public AnimationCurve Random17; public AnimationCurve Random18; public AnimationCurve Random19;
    public AnimationCurve Random20; public AnimationCurve Random21; public AnimationCurve Random22; public AnimationCurve Random23; public AnimationCurve Random24;

    // shadow spin movements
    public AnimationCurve Spin0; public AnimationCurve Spin1; public AnimationCurve Spin2; public AnimationCurve Spin3; public AnimationCurve Spin4; 
    public AnimationCurve Spin5; public AnimationCurve Spin6; public AnimationCurve Spin7; public AnimationCurve Spin8; public AnimationCurve Spin9; 
    public AnimationCurve Spin10; public AnimationCurve Spin11; public AnimationCurve Spin12; public AnimationCurve Spin13; public AnimationCurve Spin14; 

    // cue pump curves
    public AnimationCurve CuePump0; public AnimationCurve CuePump1; public AnimationCurve CuePump2; public AnimationCurve CuePump3; public AnimationCurve CuePump4; 
    public AnimationCurve CuePump5; public AnimationCurve CuePump6; public AnimationCurve CuePump7; public AnimationCurve CuePump8; public AnimationCurve CuePump9; 

    // smart bot artificial lag, is freeze everything and start where we left off (this should be very very rare and very very very short hiccup)
    [System.NonSerialized]
    public bool SmartBotFreezeInPlaceFlag = false;
    [System.NonSerialized]
    public float SmartBotFreezeInPlaceDuration = 0.0f;
    [System.NonSerialized]
    public float SmartBotFreezeInPlaceIncreTimer = 0.0f;

    // smart bot artificial lag, is suddenly re-start at a previous position without doing any freeze (i can even tweak the positions a little bit)
    [System.NonSerialized]
    public bool SmartBotFreezeInThePastCountDown = false;
    [System.NonSerialized]
    public bool SmartBotFreezeInThePastKnockBack = false;
    [System.NonSerialized]
    public float SmartBotFreezeInThePastDuration = 0.0f;
    [System.NonSerialized]
    public float SmartBotFreezeInThePastIncreTimer = 0.0f;

    // smart bot fake freezes
    [System.NonSerialized]
    public bool SmartBotFreezeDueToInternet = false;
    [System.NonSerialized]
    public bool SmartBotFreezeDueToFinish = false;
    
    // smart bot chance thing
    public AnimationCurve SmartBotChanceOfFinding;

    // disconnection protection, in place for the pinging 8.8.8.8
    [System.NonSerialized]
    public float timeToDisplayReconnectBanner = 3.0f;
    [System.NonSerialized]
    public float timeToBreakOffConnection = 10.0f;
    [System.NonSerialized]
    public float timeSinceDisconnected = 0.0f;

    // ftue mouse cursor hack, dajiang hack, pretty bad hack
    [System.NonSerialized]
    public Vector3 ftueMouseCursorPositionHack = Vector3.zero;

    // ftue never seen frds selector
    [System.NonSerialized]
    public float ftueSeenFrdsSelector = 0.0f;

    // rematch related stuffz....
    [System.NonSerialized]
    public bool rematchCurrentMatchIsRematch = false; // this one tells you if the current match is a rematch
    [System.NonSerialized]
    public bool rematchSmartBotLevelAdded = false; // this one tells you if smart bot has been awarded a level in a series
    [System.NonSerialized]
    public bool rematchYouAreThePrevBreaker = false; // this one tells you if you were the break shooter in the previous game
    [System.NonSerialized]
    public bool rematchYouAreThePrevWinner = false; // this one tells you if you were the winner in the previous game
    [System.NonSerialized]
    public bool rematchSmartBotSeries = false; // this one is set true if its a smart bot game
    [System.NonSerialized]
    public bool rematchSelfWantToRematch = false; // this one is set true if you click on rematch button
    [System.NonSerialized]
    public bool rematchOppoWantToRematch = false; // this one is set true if you got rematch signal from opponent
    [System.NonSerialized]
    public string rematchPasscodeKey = ""; // this is the passcode key for rematch games
    [System.NonSerialized]
    public int rematchYourWinCount = 0;
    [System.NonSerialized]
    public int rematchOppoWinCount = 0;
    [System.NonSerialized]
    public float rematchPrevTPAScore = 0.0f;
    [System.NonSerialized]
    public bool rematchSmartBotWantToRematch = false;
    [System.NonSerialized]
    public int rematchSmartBotClickPhase = 0;




    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        Application.LoadLevel("NGUI_MENU");

		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ball-pool-867282.firebaseio.com/");
		Vungle.init ("59263dd625126db838000fd3", "Test_iOS", "Test_Windows");
    }

    public void Awake() // my DontDestroyOnLoad is probably weird, it is ALWAYS called every time we load the menu scene
    {
		
        instance = this;

        Application.targetFrameRate = targetFrameRate;

        // screen width and height ratio clamped between the curves available
        // right now, table outline ratio is 1.83f
        float ratio = 1.0f * Screen.width / Screen.height;
        float table_height = 0.0f;

        // need futher changes...
        if (ratio > GameManager_script.Table_Self_Ratio) // use height as primary factor
        {
            // side gap, we can tweak it later...
            Side_Gap = (Screen.width - Screen.height * Table_Screen_Ratio * Table_Self_Ratio) * 0.5f;

            // top and bot and mid gaps... important
            table_height = Screen.height * Table_Screen_Ratio;
        }
        else // use width as primary factor
        {
            // side gap, we can tweak it later...
            Side_Gap = (Screen.width - Screen.width * Table_Screen_Ratio) * 0.5f;

            // top and bot and mid gaps... important
            table_height = Screen.width * Table_Screen_Ratio / Table_Self_Ratio;
        }

        float top_gap_proposal = (Screen.height - table_height) * Table_Bot_Top_Ratio / (1.0f + Table_Bot_Top_Ratio);
        float top_gap_mandatody = Screen.width / Top_Width_Ratio; // cannot be bigger than this...

        if (top_gap_proposal > top_gap_mandatody)
        {
            float extra_padding = Screen.height - table_height - top_gap_mandatody;

            Top_Padding = extra_padding * 0.25f;
            Mid_Gap = extra_padding * 0.25f;
            Top_Gap = top_gap_mandatody;
            Bot_Gap = extra_padding * 0.50f;
        }
        else
        {
            Top_Padding = 0.0f;
            Top_Gap = top_gap_proposal;
            Mid_Gap = 0.0f;
            Bot_Gap = Screen.height - Top_Padding - Top_Gap - table_height - Mid_Gap; // this is all good...
        }

        // ginger id
        if (PlayerPrefs.GetString("Ginger_ID") != "")
        {
            Ginger_ID = PlayerPrefs.GetString("Ginger_ID");
        }
        else
        {
            Ginger_ID = CreateRandomUniqueKey();

            PlayerPrefs.SetString("Ginger_ID", Ginger_ID);
        }

#if UNITY_ANDROID
            platformNumber = "Android";
#elif UNITY_IPHONE
            platformNumber = "iOS";
#else
            platformNumber = "Debug";
#endif

        versionNumber = "2.20a";

        marketNumber = "China"; // could be China, China Mobile or something else

        vSettingInfo = new SettingInfo
        (
            "WindowSettingLanguage", "Change", ClickType.Look,
            "WindowSettingSound", "WindowSettingSoundButtonText", ClickType.Disable,
            "WindowSettingVibrate", "WindowSettingVibrate", ClickType.Disable,
            "WindowSettingHelp", "View", ClickType.Look,
            "WindowSettingMain_help", "View", ClickType.Look,
            "WindowSettingScore", "View", ClickType.Look,
            "WindowSettingProducer", "Ginger Ale Gaming", ClickType.Label,
            "WindowSettingProgrammer", "Dajiang Wei", ClickType.Label,
            "WindowSettingProgrammer", "Mengxiang Gu", ClickType.Label,
            "WindowSettingArtist2", "John Lerma", ClickType.Label,
            "WindowSettingArtist2", "Ben Burns", ClickType.Label,
            "WindowSettingArtist3", "Luo Zhao", ClickType.Label,
            "WindowSettingDesigner", "Chifeng Qu,Hecun Yan", ClickType.Label,
            "WindowSettingVersion", versionNumber + "_" + platformNumber, ClickType.Label
        );

        // name
        if (PlayerPrefs.GetString("First_Name") != "")
        {
            First_Name = PlayerPrefs.GetString("First_Name");
        }
        else
        {
            First_Name = Localization.Get("FirstName");

            PlayerPrefs.SetString("First_Name", First_Name);
        }

        if (PlayerPrefs.GetString("SoundEnabled") == "")
        {
            SoundEnabled = "e";

            PlayerPrefs.SetString("SoundEnabled", SoundEnabled);
        }
        else
        {
            SoundEnabled = PlayerPrefs.GetString("SoundEnabled");//声音
        }

        if (PlayerPrefs.GetString("VibeEnabled") == "")
        {
            VibeEnabled = "d";

            PlayerPrefs.SetString("VibeEnabled", VibeEnabled);
        }
        else
        {
            VibeEnabled = PlayerPrefs.GetString("VibeEnabled");//震动
        }

        // language
        Savelanguage = PlayerPrefs.GetString("Savelanguage");

        // other stuffz
		MadePurchaseEver = PlayerPrefs.GetFloat("MadePurchaseEver");//曾经购买
		ClaimedDailyBonusEver = PlayerPrefs.GetFloat("ClaimedDailyBonusEver");//每日获得奖金
		SetGameCoinEver = PlayerPrefs.GetFloat("SetGameCoinEver");//设置游戏硬币

        // load coin, if very new to game then 
        if (PlayerPrefs.GetFloat("Coin") <= 0.0f && SetGameCoinEver == 0.0f) // we are first time ever
        {
            CoinCount = 300.0f;

            PlayerPrefs.SetFloat("Coin", CoinCount);
        }
        else
        {
            CoinCount = PlayerPrefs.GetFloat("Coin");
        }

        // smart bot related stuffzzz
		PlayerPrefsX.PopulateFloatListFromArray(ref SmartBotGameLoseList, "SmartBotGameLoseList");
        PlayerPrefsX.PopulateFloatListFromArray(ref SmartBotLongShotTakenList, "SmartBotLongShotTakenList");
        PlayerPrefsX.PopulateFloatListFromArray(ref SmartBotLongShotMadeList, "SmartBotLongShotMadeList");

        // if smart bot related stuffz are empty, we should change up
        SmartBotArrayInitCheckUp();

        // load stats (all time)
		PlayerPrefsX.PopulateFloatListFromArray(ref GameWonList, "GameWonList");//游戏名单
        PlayerPrefsX.PopulateFloatListFromArray(ref ChipsWonList, "ChipsWonList");//
		Total_Games_Played = PlayerPrefs.GetFloat("Total_Games_Played");//玩游戏总数
		Total_Games_Won = PlayerPrefs.GetFloat("Total_Games_Won");//总游戏赢了
        Total_Chips_Lost = PlayerPrefs.GetFloat("Total_Chips_Lost");//输掉的总金币数
        Total_Chips_Won = PlayerPrefs.GetFloat("Total_Chips_Won");//赢到的总金币数

        // streak
		Current_Win_Streak = PlayerPrefs.GetFloat("Current_Win_Streak");//当前连胜
		Current_Lose_Streak = PlayerPrefs.GetFloat("Current_Lose_Streak");//当前连输

        // tpa related shiites
        PlayerPrefsX.PopulateFloatListFromArray(ref BallsPottedList, "BallsPottedList");
        PlayerPrefsX.PopulateFloatListFromArray(ref SnookeredSelfList, "SnookeredSelfList");
        PlayerPrefsX.PopulateFloatListFromArray(ref MissShotsList, "MissShotsList");
        PlayerPrefsX.PopulateFloatListFromArray(ref ScratchList, "ScratchList");
        Total_Balls_Potted = PlayerPrefs.GetFloat("Total_Balls_Potted"); // this is the ONLY success condition (numerator)
        Total_Snookered_Self = PlayerPrefs.GetFloat("Total_Snookered_Self"); // fail 1
        Total_Miss_Shots = PlayerPrefs.GetFloat("Total_Miss_Shots"); // fail 2
        Total_Scratch = PlayerPrefs.GetFloat("Total_Scratch"); // fail 3

        // load experience  加载经验
        if (PlayerPrefs.GetFloat("NineBallExperience") <= GameManager_script.LevelExperienceCounts[0])
        {
            NineBallExperience = GameManager_script.LevelExperienceCounts[0];

            PlayerPrefs.SetFloat("NineBallExperience", NineBallExperience);
        }
        else
        {
            NineBallExperience = PlayerPrefs.GetFloat("NineBallExperience");
        }

        // get level 获得等级
        GameManager_script.Instance().CurrentLevel = PlayerPrefs.GetFloat("CurrentLevel");

        // calculate level anyways
        GameManager_script.CalculateLevel();

        // get install # of days since beginning
		Install_Day_Since_Beginning = PlayerPrefs.GetInt("Install_Day_Since_Beginning");//自开始安装日

        // try to reset it if some goes wrong...
        if (Install_Day_Since_Beginning == 0)
        {
            if (SetGameCoinEver == 0.0f)
            {
                // never entered the game before, use current # of days
                Install_Day_Since_Beginning = (int)(System.DateTime.Now.Subtract(new System.DateTime(2014, 1, 1)).TotalDays);

                PlayerPrefs.SetInt("Install_Day_Since_Beginning", Install_Day_Since_Beginning);
            }
            else
            {
                // entered the game before, use a level deducted # of days
                Install_Day_Since_Beginning = (int)(System.DateTime.Now.Subtract(new System.DateTime(2014, 1, 1)).TotalDays) - (int)GameManager_script.Instance().CurrentLevel;

                PlayerPrefs.SetInt("Install_Day_Since_Beginning", Install_Day_Since_Beginning);
            }
        }

		// get equipped  得到装备
		AvatarEquipped/*头像装备*/ = PlayerPrefs.GetInt("AvatarEquipped");

        // set some avatar shiite
        if (AvatarEquipped >= AvatarAttributes.Length)
        {
            AvatarEquipped = AvatarAttributes.Length - 1;

            PlayerPrefs.SetInt("AvatarEquipped", AvatarEquipped);
        }
        else if (AvatarEquipped < 0)
        {
            AvatarEquipped = 0;

            PlayerPrefs.SetInt("AvatarEquipped", AvatarEquipped);
        }

        // get avatar new
        if (PlayerPrefsX.GetBoolArray("AvatarNew").Length == 0)//新的头像
        {
            AvatarNew = PlayerPrefsX.GetBoolArray("AvatarNew", false, AvatarAttributes.Length);
            
            PlayerPrefsX.SetBoolArray("AvatarNew", AvatarNew);
        }
        else
        {
            AvatarNew = PlayerPrefsX.GetBoolArray("AvatarNew");
        }

		// get avatar owned  获得头像
        if (PlayerPrefsX.GetBoolArray("AvatarOwned").Length == 0)//拥有的头像
        {
            AvatarOwned = PlayerPrefsX.GetBoolArray("AvatarOwned", false, AvatarAttributes.Length);
            AvatarOwned[0] = true; // by default the first one is owned.
            PlayerPrefsX.SetBoolArray("AvatarOwned", AvatarOwned);
        }
        else
        {
            AvatarOwned = PlayerPrefsX.GetBoolArray("AvatarOwned");
        }

        // get cue equipped  获得球杆装备
        CueEquipped = PlayerPrefs.GetInt("CueEquipped");

        // set some cue shiite
        if (CueEquipped >= CueAttributes.Length)
        {
            CueEquipped = CueAttributes.Length - 1;

            PlayerPrefs.SetInt("CueEquipped", CueEquipped);
        }
        else if (CueEquipped < 0)
        {
            CueEquipped = 0;

            PlayerPrefs.SetInt("CueEquipped", CueEquipped);
        }

        // get cue new 获得新的球杆
        if (PlayerPrefsX.GetBoolArray("CueNew").Length == 0)
        {
            CueNew = PlayerPrefsX.GetBoolArray("CueNew", false, CueAttributes.Length);
            
            PlayerPrefsX.SetBoolArray("CueNew", CueNew);
        }
        else
        {
            CueNew = PlayerPrefsX.GetBoolArray("CueNew");
        }

        // get cue owned  获得拥有的球杆
        if (PlayerPrefsX.GetBoolArray("CueOwned").Length == 0)
        {
            CueOwned = PlayerPrefsX.GetBoolArray("CueOwned", false, CueAttributes.Length);
			CueOwned[0] = true; // by default the first one is owned.  默认情况下，第一个是拥有的。
            PlayerPrefsX.SetBoolArray("CueOwned", CueOwned);
        }
        else
        {
            CueOwned = PlayerPrefsX.GetBoolArray("CueOwned");
        }

        // O.... menu visibility stack, look im fancy...
        MenuVisibilityStack = new List<GameObject>(0);

        // if player ever loaded a game in v2 before
        NeverLoadedGameBefore = PlayerPrefs.GetFloat("NeverLoadedGameBefore");

        // init daily bonus and custom rtl
        LastTimeDailyBonusOpened = PlayerPrefs.GetFloat("LastTimeDailyBonusOpened");

        // init daily deal calculation dates
        LastDayDailyDealCalculated = PlayerPrefs.GetFloat("LastDayDailyDealCalculated");

        // init daily deal seen
        newDailyDealPrStarterPackageSeen = PlayerPrefs.GetInt("newDailyDealPrStarterPackageSeen");

        // generate or get daily deal package
        if (PlayerPrefsX.GetFloatArray("DailyDealPopupInfoSave").Length == 0)
        {
            GenerateDailyDealPackage();

            PlayerPrefsX.SetFloatArray("DailyDealPopupInfoSave", DailyDealPopupInfoSave);
        }
        else
        {
            DailyDealPopupInfoSave = PlayerPrefsX.GetFloatArray("DailyDealPopupInfoSave");
        }

        // ftue seen frds selector or not
        ftueSeenFrdsSelector = PlayerPrefs.GetFloat("ftueSeenFrdsSelector");

        // finally set to 1.0f (dajiang hack, this must be called last in this function after all the other sets and gets)
        GameManager_script.Instance().SetGameCoinEver = 1.0f;
        PlayerPrefs.SetFloat("SetGameCoinEver", GameManager_script.Instance().SetGameCoinEver);

        // immediately start Android payment
        startAndroidPaymentActivities();

        // immediately start iOS payment
        startiOSPaymentActivities();
    }

    public void OnApplicationPause(bool inPaused)
    {
        if (!inPaused)
        {
            // do some payment stuffz
            startAndroidPaymentActivities();
            startiOSPaymentActivities();

            // we are un-pausing
            currentlyPaused = false;
        }
        else
        {
            // as long as we are not in any network related games, we disconnect
            if (!GameManager_script.Instance().NetworkGameSceneCurrentLoad)
            {
                // kill them connections
                PhotonNetwork.Disconnect();
            }

            // we are pausing
            currentlyPaused = true;
        }
    }

    [System.NonSerialized]
    public float findCount = 0.0f;
    [System.NonSerialized]
    public float cannotFindCount = 0.0f;
    [System.NonSerialized]
    public float findTime = 0.0f;

    // update whatever, mostly for coins
    public void Update()
    {
        // Android coinz update
        AndroidCoinzUpdate();

        // iOS coinz update
        IOSCoinzUpdate();

        // network disconnection check
        if (PhotonNetwork.connected)
        {
            timeSinceDisconnected = 0.0f;
        }
        else
        {
            timeSinceDisconnected += Time.deltaTime;
        }

        // network connectivity and photon connectivity every 1.0 second
        if (Time.realtimeSinceStartup - networkConnectionTimer > 1.0f)
        {
            networkConnectionTimer = Time.realtimeSinceStartup;

            KeepingNetworkConnected();
        }

        // ping product info every 30 seconds (android)
        if (Time.realtimeSinceStartup - AndroidPurchaseProductInfoCounter > 30.0f)
        {
            AndroidPurchaseProductInfoCounter = Time.realtimeSinceStartup;

            startAndroidPaymentActivities();
        }

        // ping product info every 30 seconds (at least check it)
        if (Time.realtimeSinceStartup - iOSPurchaseProductInfoCounter > 30.0f)
        {
            iOSPurchaseProductInfoCounter = Time.realtimeSinceStartup;

            startiOSPaymentActivities();
        }
        
        // ping parse on user data in general, every 1 minute
        if (Time.realtimeSinceStartup - parsePeriodicPingCounter > 60.0f)
        {
            parsePeriodicPingCounter = Time.realtimeSinceStartup;

            Analytic.OnlinePeriodicPing(CoinCount, GetMaxTPAScore(), CurrentLevel, Total_Games_Played, Total_Games_Won);
        }
    }

    // Android coinz update, lolz
    public void AndroidCoinzUpdate()
    {
#if UNITY_ANDROID
        float coinz = 0.0f;

        switch(AndroidPurchases.PackageName)
        {
            case "chippack1": // 1.99
                {
                    coinz = CoinCounts[5];
                    Analytic.BuyItemPing(MoneyCounts[5]);

                    break;
                }
            case "chippack2": // 4.99
                {
                    coinz = CoinCounts[3];
                    Analytic.BuyItemPing(MoneyCounts[3]);

                    break;
                }
            case "chippack3": // 9.99
                {
                    coinz = CoinCounts[1];
                    Analytic.BuyItemPing(MoneyCounts[1]);

                    break;
                }
            case "chippack4": // 19.99
                {
                    coinz = CoinCounts[4];
                    Analytic.BuyItemPing(MoneyCounts[4]);

                    break;
                }
            case "chippack5": // 49.99
                {
                    coinz = CoinCounts[2];
                    Analytic.BuyItemPing(MoneyCounts[2]);

                    break;
                }
            case "chippack6": // 99.99
                {
                    coinz = CoinCounts[0];
                    Analytic.BuyItemPing(MoneyCounts[0]);

                    break;
                }
//            case "dailydeal1": // 0.99 (starter pack)
//            case "dailydeal2": // 1.99 (daily pack)
//            case "dailydeal3": // 2.99 (daily pack)
//            case "dailydeal4": // 3.99 (daily pack)
//            case "dailydeal5": // 4.99 (daily pack)

		//GU
		case "newdeal1":
                {
                    coinz = DailyDealPopupInfoSave[2] + DailyDealPopupInfoSave[3];
                    Analytic.BuyItemPing(DailyDealPopupInfoSave[4]);

                    if (DailyDealPopupInfoSave[1] != -1.0f && DailyDealPopupInfoSave[1] < CueAttributes.Length)
                    {
                        SetCueStateAfterPurchase((int)DailyDealPopupInfoSave[1]);
                    }

                    break;
                }
        }

        // only call this when we have something to work with
        if (coinz != 0.0f)
        {
            // old money here
            float oldMoney = GameManager_script.Instance().CoinCount;

            // update coin count
            GameManager_script.Instance().UpdateCoinCount(coinz);

            // broadcast them coinz
            GameManager_script.Instance().UpdateWindowMoney(oldMoney);

            // purchased
            GameManager_script.Instance().MadePurchaseEver = 1.0f;
            PlayerPrefs.SetFloat("MadePurchaseEver", GameManager_script.Instance().MadePurchaseEver);

            // clean up
            AndroidPurchases.PackageName = "";
        }
#endif
    }

    // iOS coinz update, for symmetry purposes, no actual use for now
    public void IOSCoinzUpdate()
    {
#if UNITY_IPHONE
        // do nothing
#endif
    }

    public void OnEnable()
    {
#if UNITY_IPHONE
        StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
        StoreKitManager.productListReceivedEvent += productListReceivedEvent;
        StoreKitManager.productListRequestFailedEvent += productListRequestFailedEvent;
#endif
    }

    public void OnDisable()
    {
#if UNITY_IPHONE
        StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
        StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
        StoreKitManager.productListRequestFailedEvent -= productListRequestFailedEvent;
#endif
    }

#if UNITY_IPHONE
    void productListReceivedEvent(List<StoreKitProduct> productList)
    {
        // set we are good var and we are just good...
        iOSPurchaseProductInfoReceived = true;
    }
#endif

#if UNITY_IPHONE
    void productListRequestFailedEvent(string error)
    {
        // log a variable and try to request for the info again
        startiOSPaymentActivities();
    }
#endif

#if UNITY_IPHONE
    void purchaseSuccessfulEvent(StoreKitTransaction transaction)
    {
        float coinz = 0.0f;

        switch(transaction.productIdentifier)
        {
            case "chippack1": // 1.99
                {
                    coinz += GameManager_script.CoinCounts[5];
                    Analytic.BuyItemPing(GameManager_script.MoneyCounts[5]);

                    break;
                }
            case "chippack2": // 4.99
                {
                    coinz += GameManager_script.CoinCounts[3];
                    Analytic.BuyItemPing(GameManager_script.MoneyCounts[3]);

                    break;
                }
            case "chippack3": // 9.99
                {
                    coinz += GameManager_script.CoinCounts[1];
                    Analytic.BuyItemPing(GameManager_script.MoneyCounts[1]);

                    break;
                }
            case "chippack4": // 19.99
                {
                    coinz += GameManager_script.CoinCounts[4];
                    Analytic.BuyItemPing(GameManager_script.MoneyCounts[4]);

                    break;
                }
            case "chippack5": // 49.99
                {
                    coinz += GameManager_script.CoinCounts[2];
                    Analytic.BuyItemPing(GameManager_script.MoneyCounts[2]);

                    break;
                }
            case "chippack6": // 99.99
                {
                    coinz += GameManager_script.CoinCounts[0];
                    Analytic.BuyItemPing(GameManager_script.MoneyCounts[0]);

                    break;
                }
            case "newdeal1": // 0.99
            case "newdeal2": // 1.99
            case "newdeal3": // 2.99
            case "newdeal4": // 3.99
            case "newdeal5": // 4.99
                {
                    coinz += (DailyDealPopupInfoSave[2] + DailyDealPopupInfoSave[3]);
                    Analytic.BuyItemPing(DailyDealPopupInfoSave[4]);

                    if (DailyDealPopupInfoSave[1] != -1.0f && DailyDealPopupInfoSave[1] < CueAttributes.Length)
                    {
                        SetCueStateAfterPurchase((int)DailyDealPopupInfoSave[1]);
                    }

                    break;
                }
        }

        // only call this when we have something to work with
        if (coinz != 0.0f)
        {
            // old money here
            float oldMoney = GameManager_script.Instance().CoinCount;

            // update coin count
            GameManager_script.Instance().UpdateCoinCount(coinz);

            // broadcast them coinz
            GameManager_script.Instance().UpdateWindowMoney(oldMoney);

            // you purchased
            GameManager_script.Instance().MadePurchaseEver = 1.0f;
            PlayerPrefs.SetFloat("MadePurchaseEver", GameManager_script.Instance().MadePurchaseEver);
        }
    }
#endif

    // just do it once and we are good...
    public void startAndroidPaymentActivities()
    {
#if UNITY_ANDROID
        if (!AndroidPurchases.iabsetup)
        {
            AndroidPurchases.StartInAppBilling();
        }
#endif
    }

    // in update and checked every minute (also on product info retrieve fail)
    public void startiOSPaymentActivities()
    {
#if UNITY_IPHONE
        if (!iOSPurchaseProductInfoReceived)
        {
            StoreKitBinding.requestProductData(GameManager_script.IPhonePurchasableItems);
        }
#endif
    }

    // this function keeps network availability checked and photon connected at all times, if possible
    public void KeepingNetworkConnected()
    {
        // photon connection related stuffz (if we are not connected, try to connect).
        if (!PhotonNetwork.connected && !currentlyPaused)
        {
            // connect to best server out of the 3 (not 4, i removed Japan)
            PhotonNetwork.ConnectToBestCloudServer("random");
        }
    }

    public static GameManager_script Instance()
    {
        return instance;
    }

    public void GenerateDailyDealPackage()
    {
        DailyDealPopupInfoSave = new float[5];

        if (MadePurchaseEver == 0.0f)
        {
            // starter package
            DailyDealPopupInfoSave[0] = 1.0f;

            // cue generation
            List<int> rc = ReturnCueListAvailableForPurchase(0.20f);
            DailyDealPopupInfoSave[1] = rc.Count == 0 ? -1.0f : rc[Random.Range(0, rc.Count)];

            // coin counts (2X chips on this guy)
            DailyDealPopupInfoSave[2] = DailyDealPopupInfoSave[0] * 3000.0f;
            DailyDealPopupInfoSave[3] = DailyDealPopupInfoSave[0] * 3000.0f;

            // price generation
            DailyDealPopupInfoSave[4] = 0.99f;
        }
        else
        {
            int level = Random.Range(2, 6); // 2, 3, 4, 5 are the possible values

            // regular package
            DailyDealPopupInfoSave[0] = (float)level; // 2, 3, 4, 5

            // cue generation
            List<int> rc = ReturnCueListAvailableForPurchase((float)level * 0.20f); // so 0.4, 0.6, 0.8 and 1.0f
            DailyDealPopupInfoSave[1] = rc.Count == 0 ? -1.0f : rc[Random.Range(0, rc.Count)];

            // coin counts (2X chips on this guy)
            DailyDealPopupInfoSave[2] = (float)level * 3000.0f; // main thing
            DailyDealPopupInfoSave[3] = DetermineLotteryResult(0.50f) ? (float)level * 3000.0f * 0.50f : (float)level * 3000.0f * 0.25f; // side pot

            // price generation
            DailyDealPopupInfoSave[4] = (float)level - 0.01f;
        }

        // save
        PlayerPrefsX.SetFloatArray("DailyDealPopupInfoSave", DailyDealPopupInfoSave);
    }

    public static void IncreaseExperience(bool inWin)
    {
        GameManager_script.Instance().NineBallExperience += GetExperienceBasedOnWagerAndWin(GameManager_script.Instance().CurrentWager, inWin);

        PlayerPrefs.SetFloat("NineBallExperience", GameManager_script.Instance().NineBallExperience);
    }

    public static float GetExperienceBasedOnWagerAndWin(float inWager, bool inWin)
    {
        int inDex = 0;

        for (int i = 0; i < WagerLevels.Length; i++)
        {
            if (inWager == WagerLevels[i])
            {
                inDex = i;
            }
        }

        if (inWin)
        {
            return ExperienceIncreaseWin[inDex];
        }
        else
        {
            return ExperienceIncreaseLoss[inDex];
        }
    }

    // we need to cover where user blows through the final level completely, we do.
    public static float CalculateLevel()
    {
        float exp = GameManager_script.Instance().NineBallExperience;
        float lvl = GameManager_script.Instance().CurrentLevel;

        for (int i = 0; i < GameManager_script.LevelExperienceCounts.Length; i++)
        {
            if (GameManager_script.LevelExperienceCounts[i] > exp) // we are still progressing
            {
                GameManager_script.Instance().CurrentLevel = i;
                GameManager_script.Instance().CurrentLevelExperience = Mathf.Clamp01(exp / GameManager_script.LevelExperienceCounts[i]);

                break;
            }
            else if (GameManager_script.LevelExperienceCounts[i] < exp && i == GameManager_script.LevelExperienceCounts.Length - 1) // we blew it out of the water
            {
                GameManager_script.Instance().CurrentLevel = i + 1; // totally +1 level
                GameManager_script.Instance().CurrentLevelExperience = 1.0f; // totally no need to level anymore

                break;
            }
            else
            {
                exp -= GameManager_script.LevelExperienceCounts[i];
            }
        }

        // we need to take a look at the game and make sure we show FTUE to the rite ppl
        GameManager_script.Instance().CalculateFTUEBasedOnExistingValues();

        // we leveled up, need to return the good news?
        if (GameManager_script.Instance().CurrentLevel > lvl)
        {
            PlayerPrefs.SetFloat("CurrentLevel", GameManager_script.Instance().CurrentLevel);

            return (GameManager_script.Instance().CurrentLevel - lvl);
        }
        else
        {
            return 0.0f;
        }
    }

    public static void IncrementBallPocketed(bool inPOne, bool inPTwo)
    {
        if (inPOne)
        {
            GameManager_script.Instance().P_One_Balls_Potted++;
        }

        if (inPTwo)
        {
            GameManager_script.Instance().P_Two_Balls_Potted++;
        }
    }

    public static void DecrementBallPocketed(bool inPOne, bool inPTwo)
    {
        if (inPOne)
        {
            GameManager_script.Instance().P_One_Balls_Potted--;
        }

        if (inPTwo)
        {
            GameManager_script.Instance().P_Two_Balls_Potted--;
        }
    }

    public static void IncrementShotsMissed(bool inPOne, bool inPTwo)
    {
        if (inPOne)
        {
            GameManager_script.Instance().P_One_Miss_Shots++;
        }

        if (inPTwo)
        {
            GameManager_script.Instance().P_Two_Miss_Shots++;
        }
    }

    public static void IncrementSnookerSelf(bool inPOne, bool inPTwo)
    {
        if (inPOne)
        {
            GameManager_script.Instance().P_One_Snookered_Self++;
        }

        if (inPTwo)
        {
            GameManager_script.Instance().P_Two_Snookered_Self++;
        }
    }

    public static void IncrementScratch(bool inPOne, bool inPTwo)
    {
        if (inPOne)
        {
            GameManager_script.Instance().P_One_Scratch++;
        }

        if (inPTwo)
        {
            GameManager_script.Instance().P_Two_Scratch++;
        }
    }

    public List<int> ReturnAvatarListAvailableForPurchase()
    {
        List<int> ra= new List<int>(0);

        for (int i = 0; i < (int)(AvatarAttributes.Length * 1.0f); i++) // dajiang hack, (given the incoming money level, we should not always use the entire length of the array).
        {
            // we are not locked out and we don't own the avatar
            if (AvatarAttributes[i][(int)AvatarAttributesType.level] <= CurrentLevel && !AvatarOwned[i])
            {
                ra.Add(i);
            }
        }

        return ra;
    }

    public int GetNewAvatarCount() // menu containing avatar's parent badge is called upon (could be that it opened or it is called by its parent)
    {
        int avatarNewCount = 0;

        for (int i = 0; i < AvatarNew.Length; i++)
        {
            if (AvatarNew[i])
            {
                avatarNewCount++;
            }
        }

        return avatarNewCount;
    }

    public void ResetNewAvatarCount() // happens when avatar menu close button is clicked
    {
        for (int i = 0; i < AvatarNew.Length; i++)
        {
            AvatarNew[i] = false;
        }

        PlayerPrefsX.SetBoolArray("AvatarNew", AvatarNew);
    }

    public void IncreaseAvatarNewCount(float NumerOfLevels) // happens when we unlock
    {
        for (int i = 0; i < AvatarAttributes.Length; i++)
        {
            if (AvatarAttributes[i][0] <= CurrentLevel && AvatarAttributes[i][0] > CurrentLevel - NumerOfLevels)
            {
                AvatarNew[i] = true;
            }
        }

        PlayerPrefsX.SetBoolArray("AvatarNew", AvatarNew);
    }

    public GenericButtonState GetAvatarState(int inIndex)
    {
        if (inIndex == AvatarEquipped)
        {
            return GenericButtonState.equipped; // equipped
        }

        if (AvatarOwned.Length > inIndex && AvatarOwned[inIndex])
        {
            return GenericButtonState.owned; // owned but not equipped
        }

        if (AvatarAttributes.Length > inIndex && AvatarAttributes[inIndex][(int)AvatarAttributesType.level] > CurrentLevel)
        {
            return GenericButtonState.locked; // locked cannot buy
        }

        if (AvatarAttributes.Length > inIndex && AvatarAttributes[inIndex][(int)AvatarAttributesType.price] == 0)
        {
            return GenericButtonState.free;
        }

        if (AvatarAttributes.Length > inIndex && AvatarAttributes[inIndex][(int)AvatarAttributesType.price] > CoinCount)
        {
            return GenericButtonState.cantafford; // cannot afford
        }
        else
        {
            return GenericButtonState.payable; // cannot afford
        }
    }

    public void SetAvatarStateAfterPurchase(int inIndex)
    {
        // reset equipped ints, don't forget to physically change the game object in the code
        AvatarEquipped = inIndex;
        PlayerPrefs.SetInt("AvatarEquipped", AvatarEquipped);

        // update owner list
        if (AvatarOwned.Length > inIndex && !AvatarOwned[inIndex])
        {
            AvatarOwned[inIndex] = true;

            PlayerPrefsX.SetBoolArray("AvatarOwned", AvatarOwned);
        }
    }

    public List<int> ReturnCueListAvailableForPurchase(float inLEvel)
    {
        List<int> rc = new List<int>(0);

		for (int i = 0; i < (int)(CueAttributes.Length * inLEvel); i++) // dajiang hack, (given the incoming money level, we should not always use the entire length of the array).//大江黑客（给定钱币级别，我们不应该总是使用数组的整个长度）//Gu,这个应该是指，我们到达了相应的等级。这个等级上的球杆解锁，但是要有相应的金钱才能获得。
        {
            // we are not locked out and we don't own the cue  我们不锁
            if (CueAttributes[i][(int)CueAttributesType.level] <= CurrentLevel && !CueOwned[i])
            {
                rc.Add(i);
            }
        }

        return rc;
    }

    public int GetNewCueCount() // menu containing cue's parent badge is called upon (could be that it opened or it is called by its parent)
    {
        int cueNewCount = 0;

        for (int i = 0; i < CueNew.Length; i++)
        {
            if (CueNew[i])
            {
                cueNewCount++;
            }
        }

        return cueNewCount;
    }

	public void ResetNewCueCount() // happens when cue menu close button is clicked  当点击菜单关闭按钮被点击时发生
    {
        for (int i = 0; i < CueNew.Length; i++)
        {
            CueNew[i] = false;
        }

        PlayerPrefsX.SetBoolArray("CueNew", CueNew);
    }

    public void IncreaseCueNewCount(float NumerOfLevels) // we ONLY call this once when we certainly level up
    {
        for (int i = 0; i < CueAttributes.Length; i++)
        {
            if (CueAttributes[i][(int)CueAttributesType.level] <= CurrentLevel && CueAttributes[i][(int)CueAttributesType.level] > CurrentLevel - NumerOfLevels)
            {
                CueNew[i] = true;
            }
        }

        PlayerPrefsX.SetBoolArray("CueNew", CueNew);
    }

    public GenericButtonState GetCueState(int inIndex)
    {
        if (inIndex == CueEquipped)
        {
			return GenericButtonState.equipped; // equipped  装备
        }

        if (CueOwned.Length > inIndex && CueOwned[inIndex])
        {
			return GenericButtonState.owned; // owned but not equipped  拥有但没有装备
        }

        if (CueAttributes.Length > inIndex && CueAttributes[inIndex][(int)CueAttributesType.level] > CurrentLevel)
        {
			return GenericButtonState.locked; // locked cannot buy  锁定不能买
        }

        if (CueAttributes.Length > inIndex && CueAttributes[inIndex][(int)CueAttributesType.price] == 0)
        {
            return GenericButtonState.free;
        }

		if (CueAttributes.Length > inIndex /*&& 19 > inIndex*/ && CueAttributes[inIndex][(int)CueAttributesType.price] > CoinCount) // dajiang dajiang, hack the first 20 are coin based  //大江大江，前20级都是以金币为主  //Gu,修改为全都可以被金币购买
        {
			return GenericButtonState.cantafford; // cannot afford  买不起
        }
        else
        {
			return GenericButtonState.payable; // cannot afford  买不起
        }
    }

    public void SetCueStateAfterPurchase(int inIndex)
    {
		// reset equipped ints, don't forget to physically change the game object in the code   重置装备的ints，不要忘记在代码中物理地更改游戏对象
        CueEquipped = inIndex;
        PlayerPrefs.SetInt("CueEquipped", CueEquipped);

		// update owner list  更新所有者列表
        if (CueOwned.Length > inIndex && !CueOwned[inIndex])
        {
            CueOwned[inIndex] = true;

            PlayerPrefsX.SetBoolArray("CueOwned", CueOwned);
        }
    }

    public void UpdateCoinCount(float inDifference)
    {
        // calculate new amount
        GameManager_script.Instance().CoinCount += inDifference;

        // save to backend
        PlayerPrefs.SetFloat("Coin", CoinCount);
    }

    // check for special case that removes the "new banner"
    public void AllowPopupToHideOnPage()
    {
        // set that things are being shown
        PopupCurrentlyVisible = false;

        // special case when we remove a popup and the last thing on menu is main shop, we need to refresh the first button
        if (MenuVisibilityStack.Count > 0)
        {
            UIWindowPanel uiwp = MenuVisibilityStack[MenuVisibilityStack.Count - 1] == null ? null : MenuVisibilityStack[MenuVisibilityStack.Count - 1].GetComponent<UIWindowPanel>();

            if (uiwp && uiwp.GetComponentInChildren<ShopMainButton>())
            {
                ShopMainButton[] smb = uiwp.GetComponentsInChildren<ShopMainButton>(); // they are always found sequentially

                if (newDailyDealPrStarterPackageSeen == 0 && CurrentLevel > 1.0f) // 0 is not seen // have to kinda do this individually...
                {
                    smb[0].InitLockLabelText(true, "NEW");
                }
                else
                {
                    smb[0].InitLockLabelText(false, "");
                }
            }
        }
    }

    // make sure popup shows up on page
    public void AllowPopupToShowOnPage(GameObject inPopup)
    {
        if (inPopup)
        {
            if (inPopup.GetComponent<UIPopupWindowPanel>())
            {
                // set that things are being shown
                PopupCurrentlyVisible = true;

                // modify depth
                inPopup.GetComponent<UIPopupWindowPanel>().ChangeWindowDepth(GameManager_script.UIPopupLayerStart);

                // add to stack and activate
                inPopup.SetActive(true);
            }
        }
    }

    // menu visibility stack starts here....
    // Note this menu visibility ONLY works for full screen menus we have deriving from the 3 small buttons down under
    // Note that main menu should be lower in priority but NOT hidden at ANY GIVEN TIME, simply covered
    // Note that popups should be higher in priority and ALWAYS shown covering the full screen buttons, but not hiding

    // keep on searching and find the last good menu to display, maybe do a refresh when we display it here...
    public void RemoveMenuFromStack(GameObject inMenu)
    {
        if (MenuVisibilityStack.Count > 0 && MenuVisibilityStack[MenuVisibilityStack.Count - 1] == inMenu && inMenu != null)
        {
            // clear out whatever this menu is causing players
            RefreshCurrentMenuBeforeDying(inMenu);

            // remove the current menu
            MenuVisibilityStack.Remove(inMenu);
            MenuVisibilityStack.TrimExcess();

            // refresh depth refreshers
            GameManager_script.UIMenuLayerStart -= GameManager_script.UILayerIncrementStep;

            if ((MenuVisibilityStack.Count > 0) && MenuVisibilityStack[MenuVisibilityStack.Count - 1] != null)
            {
                // refresh the top most menu page
                RefreshPreviousMenuOnStack(MenuVisibilityStack[MenuVisibilityStack.Count - 1]);
            }
            else if (MenuVisibilityStack.Count == 0)
            {
                // time to refresh the main menu page, yes?
                RefreshMainMenuPage();
            }
        }
    }

    public void RefreshCurrentMenuBeforeDying(GameObject inGB)
    {
        UIWindowPanel uiwp = inGB.GetComponent<UIWindowPanel>();

        // the menu about to die is profile menu
        if (uiwp && uiwp.GetComponentInChildren<UIProfilePanel>())
        {
            // do nothing
        }

        // the menu about to die is avatar menu
        if (uiwp && uiwp.GetComponentInChildren<AvatarButton>())
        {
            ResetNewAvatarCount();
        }

        // the menu about to die is cue menu
        if (uiwp && uiwp.GetComponentInChildren<CueButton>())
        {
            ResetNewCueCount();
        }
    }

    public void RefreshMainMenuPage()
    {
        if (uiBottomRight != null)
        {
            uiBottomRight.ChangeHeadImage();
            uiBottomRight.ChangeProfileName();
            uiBottomRight.ChangeProfileSlider();

            uiBottomRight.ChangeProfileViolatorCount(GetProfileIconViolatorCount());
            uiBottomRight.ChangeShopViolatorCount(GetShopIconViolatorCount());
            uiBottomRight.ChangeSettingViolatorCount(GetSettingIconViolatorCount());
        }

        if (uiCenter != null)
        {
            uiCenter.ChangeMoney(GameManager_script.Instance().CoinCount);
        }
    }

    // all refreshes happens here, voilators, profiles, new deal available, etc etc
    public void RefreshPreviousMenuOnStack(GameObject inGB)
    {
        // change profile image
        UIWindowPanel uiwp = inGB.GetComponent<UIWindowPanel>();

        if (uiwp)
        {
            uiwp.ChangeMoney(GameManager_script.Instance().CoinCount);
        }

        if (uiwp && uiwp.GetComponentInChildren<UIProfilePanel>())
        {
            uiwp.GetComponentInChildren<UIProfilePanel>().ChangeHeadImage(AvatarEquipped);
        }

        if (uiwp && uiwp.GetComponentInChildren<ProfileButton>())
        {
            ProfileButton[] pb = uiwp.GetComponentsInChildren<ProfileButton>(); // they are always found sequentially

            // first button is stats...
            if (GetNewStatsCount() > 0)
            {
                pb[0].InitLockLabelText(true, "NEW");
            }
            else
            {
                pb[0].InitLockLabelText(false, "");
            }

            if (true) // right now its true....
            {
                pb[1].InitLockLabelText(true, "ComingSoon");
            }
            else
            {
                pb[1].InitLockLabelText(false, "");
            }
        }

        if (uiwp && uiwp.GetComponentInChildren<ShopMainButton>())
        {
            ShopMainButton[] smb = uiwp.GetComponentsInChildren<ShopMainButton>(); // they are always found sequentially

            if (newDailyDealPrStarterPackageSeen == 0 && CurrentLevel > 1.0f) // 0 is not seen // have to kinda do this individually...
            {
                smb[0].InitLockLabelText(true, "NEW");
            }
            else
            {
                smb[0].InitLockLabelText(false, "");
            }

            if (GetNewCueCount() > 0)
            {
                smb[1].InitLockLabelText(true, "NEW");
            }
            else
            {
                smb[1].InitLockLabelText(false, "");
            }

            if (GetNewCoinCount() > 0)
            {
                smb[2].InitLockLabelText(true, "NEW");
            }
            else
            {
                smb[2].InitLockLabelText(false, "");
            }

            if (GetNewAvatarCount() > 0)
            {
                smb[3].InitLockLabelText(true, "NEW");
            }
            else
            {
                smb[3].InitLockLabelText(false, "");
            }
        }
    }

    public GameObject GetLastMenuOnStack()
    {
        if (MenuVisibilityStack.Count > 0 && MenuVisibilityStack[MenuVisibilityStack.Count - 1] != null)
        {
            return MenuVisibilityStack[MenuVisibilityStack.Count - 1];
        }
        else
        {
            return null;
        }
    }

    // make all previous game objects invisible and add a new one on
    // some disclaimers, I am increasing the depth by 10 for each menu up to 100 (which is how deep the popup windows are)
    public void AddMenuToStack(GameObject inMenu)
    {
        if (inMenu)
        {
            if (inMenu.GetComponent<UIWindowPanel>())
            {
                // increment depth count
                GameManager_script.UIMenuLayerStart += GameManager_script.UILayerIncrementStep;

                // modify depth
                inMenu.GetComponent<UIWindowPanel>().ChangeWindowDepth(GameManager_script.UIMenuLayerStart);

                // add to stack and activate
                MenuVisibilityStack.Add(inMenu);
                inMenu.SetActive(true);
            }
        }
    }

    public float GenerateSmartBotTPAScore()
    {
        float playerRecentTPA = GetTPAScore(SignedNumberCounter(BallsPottedList), SignedNumberCounter(MissShotsList), SignedNumberCounter(SnookeredSelfList), SignedNumberCounter(ScratchList));
        float lowerBound = Mathf.Clamp(playerRecentTPA - 25.0f, 25.0f, 90.0f);
        float upperBound = Mathf.Clamp(playerRecentTPA + 25.0f, 30.0f, 95.0f);

        SmartBotTPAScore = Random.Range(lowerBound, upperBound);

        return SmartBotTPAScore;
    }

    public int GenerateSmartBotLevel()
    {
        if (CurrentLevel <= 5.0f)
        {
            // 1 to 5, we match them with 1 to 10
            SmartBotLevel = Random.Range(1, 10 + 1);
        }
        else if (CurrentLevel <= 10.0f)
        {
            // 5 to 10, we match them with 1 to 20
            SmartBotLevel = Random.Range(1, 20 + 1);
        }
        else if (CurrentLevel <= 20.0f)
        {
            // 11 to 20, we match them with 1 to 40
            SmartBotLevel = Random.Range(1, 40 + 1);
        }
        else
        {
            // above level 20, we can match them with whoever
            SmartBotLevel = Random.Range(1, 100 + 1);
        }

        return SmartBotLevel;
    }

    public int GenerateCueIndex()
    {
        if ((SmartBotLevel >= 0 && SmartBotLevel < 5 && DetermineLotteryResult(0.50f)) ||
            (SmartBotLevel >= 5 && SmartBotLevel < 10 && DetermineLotteryResult(0.30f)) ||
            (SmartBotLevel >= 10 && SmartBotLevel < 25 && DetermineLotteryResult(0.20f)) ||
            (SmartBotLevel >= 25 && SmartBotLevel < 50 && DetermineLotteryResult(0.10f)) ||
            (SmartBotLevel >= 50 && SmartBotLevel < 75 && DetermineLotteryResult(0.05f)))
        {
            return 0;
        }
        else
        {
            int totalNumberOfCuesAvailable = 0 + 1;

            for (int i = 0; i < CueAttributes.Length; i++)
            {
                if (CueAttributes[i][(int)CueAttributesType.level] > SmartBotLevel)
                {
                    totalNumberOfCuesAvailable = i + 1;

                    break;
                }
            }

            totalNumberOfCuesAvailable = Mathf.Clamp(totalNumberOfCuesAvailable, 0 + 1, CueAttributes.Length);

            return Random.Range(0, totalNumberOfCuesAvailable);
        }
    }

    public int GenerateAvatarIndex()
    {
        if ((SmartBotLevel >= 0 && SmartBotLevel < 5 && DetermineLotteryResult(0.20f)) ||
            (SmartBotLevel >= 5 && SmartBotLevel < 10 && DetermineLotteryResult(0.10f)) ||
            (SmartBotLevel >= 10 && SmartBotLevel < 25 && DetermineLotteryResult(0.05f)))
        {
            return 0;
        }
        else
        {
            int totalNumberOfAvatarsAvailable = 0 + 1;

            for (int i = 0; i < AvatarAttributes.Length; i++)
            {
                if (AvatarAttributes[i][(int)AvatarAttributesType.level] > SmartBotLevel)
                {
                    totalNumberOfAvatarsAvailable = i + 1;

                    break;
                }
            }

            totalNumberOfAvatarsAvailable = Mathf.Clamp(totalNumberOfAvatarsAvailable, 0 + 1, AvatarAttributes.Length);

            return Random.Range(0, totalNumberOfAvatarsAvailable);
        }
    }

    public int GenerateTotalGamesPlayed()
    {
        if (SmartBotLevel < 10)
        {
            SmartBotTotalGamesPlayed = Random.Range(4 * SmartBotLevel, 12 * SmartBotLevel);
        }
        else if (SmartBotLevel < 40)
        {
            SmartBotTotalGamesPlayed = Random.Range(8 * SmartBotLevel, 24 * SmartBotLevel);
        }
        else if (SmartBotLevel < 70)
        {
            SmartBotTotalGamesPlayed = Random.Range(12 * SmartBotLevel, 36 * SmartBotLevel);
        }
        else
        {
            SmartBotTotalGamesPlayed = Random.Range(16 * SmartBotLevel, 48 * SmartBotLevel);
        }

        return SmartBotTotalGamesPlayed;
    }

    public int GenerateTotalGamesWon()
    {
        if (SmartBotTPAScore > 80.0f)
        {
            SmartBotTotalGamesWon = Random.Range((int)(0.50f * SmartBotTotalGamesPlayed), (int)(0.65f * SmartBotTotalGamesPlayed));
        }
        else if (SmartBotTPAScore > 60.0f)
        {
            SmartBotTotalGamesWon = Random.Range((int)(0.45f * SmartBotTotalGamesPlayed), (int)(0.60f * SmartBotTotalGamesPlayed));
        }
        else
        {
            SmartBotTotalGamesWon = Random.Range((int)(0.40f * SmartBotTotalGamesPlayed), (int)(0.55f * SmartBotTotalGamesPlayed));
        }

        return SmartBotTotalGamesWon;
    }

    public int GenerateTotalStreak()
    {
        if (SmartBotTPAScore > 80.0f)
        {
            SmartBotTotalStreak = Random.Range(0, 12);
        }
        else if (SmartBotTPAScore > 60.0f)
        {
            SmartBotTotalStreak = Random.Range(0, 6);
        }
        else
        {
            SmartBotTotalStreak = Random.Range(0, 3);
        }

        SmartBotTotalStreak = Mathf.Clamp(SmartBotTotalStreak, 0, SmartBotTotalGamesWon);

        return SmartBotTotalStreak;
    }

    public float GenerateCoinCount()
    {
        return 0.0f;
    }

    public void GenerateOverallIntentOnThisGame()
    {
        // consts for if we want to win
        float winRatio = Total_Games_Played == 0.0f ? 0.50f : Total_Games_Won / Total_Games_Played; // use overall win ratio, assuming player's ability will not decrease over time and we want to bring the whole thing down to 50%
        float highStakeBias = (CurrentWager >= 10000.0f || CurrentWager / (CurrentWager + CoinCount) >= 0.80f) ? 0.05f : 0.00f; // if we are high stake or important enough

        // we will generate one score which tell me if i want to slay this opponent
        if (Total_Games_Won == 0)
        {
            SmartBotIntendToWinGame = false;
        }
        else if (Current_Win_Streak >= 4.0f || winRatio > 0.65f)
        {
            SmartBotIntendToWinGame = GameManager_script.DetermineLotteryResult(0.950f + highStakeBias) ? true : false;
        }
        else if (Current_Win_Streak >= 2.0f || winRatio > 0.55f)
        {
            SmartBotIntendToWinGame = GameManager_script.DetermineLotteryResult(0.750f + highStakeBias) ? true : false;
        }
        else if (Current_Lose_Streak >= 4.0f || winRatio < 0.35f)
        {
            SmartBotIntendToWinGame = GameManager_script.DetermineLotteryResult(0.300f + highStakeBias) ? true : false;
        }
        else if (Current_Lose_Streak >= 2.0f || winRatio < 0.45f)
        {
            SmartBotIntendToWinGame = GameManager_script.DetermineLotteryResult(0.425f + highStakeBias) ? true : false;
        }
        else
        {
            SmartBotIntendToWinGame = GameManager_script.DetermineLotteryResult(0.550f + highStakeBias) ? true : false;
        }

        // consts on if we want to power shot (all subject to further changes)
        float overStrengthMixer = Random.Range(0.0f, 1.0f);

        if (overStrengthMixer < 0.10f)
        {
            // 10% of times, we are mostly hard hitting 70/20/10
            SmartBotAngryPowerShotPercentage = 0.70f;
            SmartBotNormalPowerShotPercentage = 0.70f + 0.20f;
        }
        else if (overStrengthMixer < 0.65f)
        {
            // 55% of times, we do a random mixture of the 3 hits 25/50/25
            SmartBotAngryPowerShotPercentage = 0.25f;
            SmartBotNormalPowerShotPercentage = 0.25f + 0.50f;
        }
        else
        {
            // 35% of times, we do mostly mid to lower hits 05/45/50
            SmartBotAngryPowerShotPercentage = 0.05f;
            SmartBotNormalPowerShotPercentage = 0.05f + 0.45f;
        }

        // consts on if we want to quick shot (if the player's own level is lower, there is a better chance of seeing a slow bot)
        if (CurrentLevel <= 5.0f)
        {
            SmartBotFastDrawShotMood = GameManager_script.DetermineLotteryResult(0.80f) ? false : true;
        }
        else if (CurrentLevel <= 10.0f)
        {
            SmartBotFastDrawShotMood = GameManager_script.DetermineLotteryResult(0.60f) ? false : true;
        }
        else if (CurrentLevel <= 20.0f)
        {
            SmartBotFastDrawShotMood = GameManager_script.DetermineLotteryResult(0.40f) ? false : true;
        }
        else
        {
            SmartBotFastDrawShotMood = GameManager_script.DetermineLotteryResult(0.20f) ? false : true;
        }
    }
    
    public GameObject PlaySound(int index, bool loop, float volume)
    {
        if (SoundEnabled == "e")
        {
            AudioSource source = mListener.GetComponent<AudioSource>();

            if (source == null)
            {
                source = mListener.gameObject.AddComponent<AudioSource>();
            }

            GameObject Group = (GameObject)Instantiate(AudioPrefab);
            Group.transform.parent = gameObject.transform;
            Group.transform.localPosition = Vector3.zero;
            Group.transform.localScale = Vector3.one;
            Group.GetComponent<PlaySound>().clip = clips[index];
            Group.GetComponent<PlaySound>().loop = loop;
            Group.GetComponent<PlaySound>().volume = volume;

            return Group;
        }
        else
        {
            return null;
        }
    }

    public void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        if (VibeEnabled == "e")
        {
            Handheld.Vibrate();
        }
#endif
    }

    public string CharLength(string text, int inLength)
    {
        string chat = "";
        char[] c = text.ToCharArray();

        for (int i = 0; i < c.Length; i++)
        {
            chat = chat + c[i].ToString();

            if (i > inLength - 2)
            {
                chat = chat.Remove(i); // gotta remove one first
                chat = chat + "...";
                break;
            }
        }

        return chat;
    }

    public static string convertNumberIntoGoodStringFormat(float inNumber, string inType)
    {
        string countryFormat = "";
        string formatFormat = "";

        switch (Localization.language)
        {
            case "English": countryFormat = "en-US"; break;
            case "Español": countryFormat = "es-ES"; break;
            case "Deutsch": countryFormat = "de-DE"; break;
            case "Français": countryFormat = "fr-FR"; break;
            case "简体中文": countryFormat = "zh-CN"; break;
            case "繁體中文": countryFormat = "zh-CN"; break;
            case "日本語": countryFormat = "ja-JP"; break;
            default: countryFormat = "en-US"; break;
        }

        switch (inType)
        {
            case "realdollar": formatFormat = "C2"; break;
            case "gamecoinz": formatFormat = "N0"; break;
            case "percentage": formatFormat = "P2"; break;
            case "smallpercentage": formatFormat = "P1"; break;
            case "tinypercentage": formatFormat = "P0"; break;
            case "number": formatFormat = "N0"; break;
            case "mediumnumber": formatFormat = "N1"; break;
            case "largenumber": formatFormat = "N2"; break;
            default: formatFormat = "N0"; break;
        }

        switch (inType)
        {
            case "percentage":
            case "smallpercentage":
            {
                return (inNumber * 0.01f).ToString(formatFormat, new CultureInfo(countryFormat).NumberFormat);
            }
            case "gamecoinz":
            {
                return "$" + inNumber.ToString(formatFormat, new CultureInfo(countryFormat).NumberFormat);
            }
            default:
            {
                return inNumber.ToString(formatFormat, new CultureInfo(countryFormat).NumberFormat);
            }
        }
    }
    
    // violator stuff..... Really shouldn't be here man............
    public static int GetProfileIconViolatorCount()
    {
        int violatorCountTotal = 0;

        // multiple violators added together, later we have stats and achievements...
        violatorCountTotal += GameManager_script.Instance().GetNewStatsCount();

        return violatorCountTotal;
    }

    public static int GetShopIconViolatorCount()
    {
        int violatorCountTotal = 0;

        // multiple violators added together
        violatorCountTotal += GameManager_script.Instance().GetNewDealCount();
        violatorCountTotal += GameManager_script.Instance().GetNewCoinCount();
        violatorCountTotal += GameManager_script.Instance().GetNewCueCount();
        violatorCountTotal += GameManager_script.Instance().GetNewAvatarCount();

        return violatorCountTotal;
    }

    public static int GetSettingIconViolatorCount()
    {
        return 0;
    }

    public int GetNewDealCount()
    {
        if (newDailyDealPrStarterPackageSeen == 0 && CurrentLevel > 1.0f) // 0 is not seen
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public int GetNewCoinCount()
    {
        return 0; // this is most likely never more than 0...
    }

    public int GetNewStatsCount()
    {
        return 0; // this i donno. its 0 for now...
    }

    public void PopulateInterstitialStartScreen(bool inTrueIfIncoming, float inTotalTPAScore, float inGamesPlayed, float inGamesWon, float inRemainingCoins, float inStreak)
    {
        // declare
        interstitialPageInfo = new UIinterstitialWindowInfo();

        // common
        interstitialPageInfo.trueIfIncomingPage = inTrueIfIncoming;
        interstitialPageInfo.currentWager = CurrentWager;

        // player 1
        interstitialPageInfo.HeadImage_one = AvatarEquipped;
        interstitialPageInfo.PlayerName_one = First_Name;
        interstitialPageInfo.Star_one = new StarInfo((int)CurrentLevel, GetSelfStarType());

        // specific 1
        interstitialPageInfo.Total_TPAScore = inTotalTPAScore;
        interstitialPageInfo.Total_GamesPlayed = inGamesPlayed;
        interstitialPageInfo.Total_GamesWon = inGamesWon;
        interstitialPageInfo.remainingCoins = inRemainingCoins;
        interstitialPageInfo.streak = inStreak;
    }

    public void PopulateInterstitialEndScreen(bool inTrueIfIncoming, bool inTrueIfWin, bool inWinByDisconnect, bool inLoseByDisconnect, bool inTrueIfLevelUp, float inBallsPotted, float inBallsMissed, float inOutOfPosition, float inScratch, float inSingleTPAScore, float inTwoBallsPotted, float inTwoBallsMissed, float inTwoOutOfPosition, float inTwoScratch, float inTwoSingleTPAScore, GameOverType inGameOverType, string inPlayerOneName, string inPlayerTwoName, int inPlayerTwoHeadImage)
    {
        // declare
        interstitialPageInfo = new UIinterstitialWindowInfo();

        // common
        interstitialPageInfo.trueIfIncomingPage = inTrueIfIncoming;
        interstitialPageInfo.trueIfYouWin = inTrueIfWin;
        interstitialPageInfo.trueIfYouWinByDisconnect = inWinByDisconnect;
        interstitialPageInfo.trueIfYouLoseByDisconnect = inLoseByDisconnect;
        interstitialPageInfo.performLevelUp = inTrueIfLevelUp;
        interstitialPageInfo.currentWager = CurrentWager;

        // player 1
        interstitialPageInfo.HeadImage_one = AvatarEquipped;
        interstitialPageInfo.PlayerName_one = inPlayerOneName;
        interstitialPageInfo.Star_one = new StarInfo((int)CurrentLevel, GetSelfStarType());

        // specific 1
        interstitialPageInfo.balls_potted_one = inBallsPotted;
        interstitialPageInfo.balls_missed_one = inBallsMissed;
        interstitialPageInfo.out_of_position_one = inOutOfPosition;
        interstitialPageInfo.scratches_one = inScratch;
        interstitialPageInfo.single_TPAScore_one = inSingleTPAScore;

        // player 2
        interstitialPageInfo.HeadImage_two = inPlayerTwoHeadImage;
        interstitialPageInfo.PlayerName_two = inPlayerTwoName;

        // specific 2
        interstitialPageInfo.balls_potted_two = inTwoBallsPotted;
        interstitialPageInfo.balls_missed_two = inTwoBallsMissed;
        interstitialPageInfo.out_of_position_two = inTwoOutOfPosition;
        interstitialPageInfo.scratches_two = inTwoScratch;
        interstitialPageInfo.single_TPAScore_two = inTwoSingleTPAScore;

        // game over...
        interstitialPageInfo.game_over_type = inGameOverType;
    }

    // the info struct should have a tag on whether it has been populated, if not, we will not use it, etc etc etc
    public void PopulateOtherGameProfileInfo(string inName, int inCue, int inAvatar, int starColor, int starNumber, float TPAScore, int gamesPlayed, int gamesWon, float inCoins, int inStreak)
    {
        // this gives player the other dude's info
        otherGameProfileInfo = new ProfilePopupInfo();

        otherGameProfileInfo.HeadImage = inAvatar;
        otherGameProfileInfo.PlayerName = inName;
        otherGameProfileInfo.Star = new StarInfo(starNumber, (StarType)starColor);
        otherGameProfileInfo.cueEquipped = inCue;
        otherGameProfileInfo.streak = inStreak;
        otherGameProfileInfo.TPAScore = TPAScore;
        otherGameProfileInfo.gamesPlayed = gamesPlayed;
        otherGameProfileInfo.gamesWon = gamesWon;
        otherGameProfileInfo.CoinsBalance = inCoins;
    }

    // the info struct should have a tag on whether it has been populated, if not, we will not use it, etc etc etc
    public void PopulateSelfGameProfileInfo()
    {
        // this gives player the other dude's info
        selfGameProfileInfo = new ProfilePopupInfo();

        selfGameProfileInfo.HeadImage = AvatarEquipped;
        selfGameProfileInfo.PlayerName = First_Name;
        selfGameProfileInfo.Star = new StarInfo((int)CurrentLevel, GetSelfStarType());
        selfGameProfileInfo.cueEquipped = CueEquipped;
        selfGameProfileInfo.streak = Current_Win_Streak;
        selfGameProfileInfo.TPAScore = GetMaxTPAScore();
        selfGameProfileInfo.gamesPlayed = Total_Games_Played;
        selfGameProfileInfo.gamesWon = Total_Games_Won;
        selfGameProfileInfo.CoinsBalance = CoinCount;
    }

    public StarType GetSelfStarType() // needs to be the same as "GetStarBasedOnValue" and "GetBotStarColor"
    {
        float selfTPAScore = GetMaxTPAScore() * 0.01f;

        if (selfTPAScore <= GameManager_script.SkillsLevel[0])
        {
            return StarType.bronze;
        }
        else if (selfTPAScore <= GameManager_script.SkillsLevel[1])
        {
            return StarType.silver;
        }
        else if (selfTPAScore <= GameManager_script.SkillsLevel[2])
        {
            return StarType.gold;
        }
        else
        {
            return StarType.red;
        }
    }

    public int GetBotStarColor(float inTPA) // needs to be the same as "GetSelfStarType" and "GetStarBasedOnValue"
    {
        inTPA /= 100.0f;

        if (inTPA <= GameManager_script.SkillsLevel[0])
        {
            return 0;
        }
        else if (inTPA <= GameManager_script.SkillsLevel[1])
        {
            return 1;
        }
        else if (inTPA <= GameManager_script.SkillsLevel[2])
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    // spins can go from around 50% to a full 100%
    public static int getStarLevelOnSpin(float inSpin)
    {
        if (inSpin < 0.625f)
        {
            return 0;
        }
        else if (inSpin < 0.750f)
        {
            return 1;
        }
        else if (inSpin < 0.875f)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    // extension can go from 1.44 to 2.88 (2.75 inches to 5.5 inches)
    public static int getStarLevelOnExtension(float inExtension)
    {
        if (inExtension < 1.80f)
        {
            return 0;
        }
        else if (inExtension < 2.16f)
        {
            return 1;
        }
        else if (inExtension < 2.52f)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    // speed can go from 13 to 26 MPH
    public static int getStarLevelOnSpeed(float inSpeed)
    {
        if (inSpeed < 146.875f)
        {
            return 0;
        }
        else if (inSpeed < 176.25f)
        {
            return 1;
        }
        else if (inSpeed < 205.625f)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    public static string getTextOnSpin(float inSpin)
    {
        return Localization.Get("spin") + " " + convertNumberIntoGoodStringFormat((inSpin * 100.0f), "smallpercentage");
    }

    public static string getTextOnExtension(float inExtension)
    {
        float extend = inExtension * 3.00f / 1.58f * (GameManager_script.Instance().Savelanguage == "English" ? 1.0f : 2.54f);
        return Localization.Get("aim") + " " + convertNumberIntoGoodStringFormat(extend, "mediumnumber") + " " + Localization.Get("inches");
    }

    public static string getTextOnSpeed(float inSpeed)
    {
        float speed = inSpeed * 20.0f / 156.7f * (GameManager_script.Instance().Savelanguage == "English" ? 1.0f : 0.44704f);

        return Localization.Get("speed") + " " + convertNumberIntoGoodStringFormat(speed, "mediumnumber") + " " + Localization.Get("mph");
    }

    public void addStatsToLastTwentyGames(bool inWin)
    {
        // add new stuff (for actual stats)
        GameWonList.Add(inWin ? 1.0f : 0.0f);
        ChipsWonList.Add(inWin ? CurrentWager : -CurrentWager);
        BallsPottedList.Add(P_One_Balls_Potted);
        SnookeredSelfList.Add(P_One_Snookered_Self);
        MissShotsList.Add(P_One_Miss_Shots);
        ScratchList.Add(P_One_Scratch);

        // remove excessive ones (for actual stats)
        while (GameWonList.Count > 20)
        {
            GameWonList.RemoveAt(0);
            ChipsWonList.RemoveAt(0);
            BallsPottedList.RemoveAt(0);
            SnookeredSelfList.RemoveAt(0);
            MissShotsList.RemoveAt(0);
            ScratchList.RemoveAt(0);
        }

        // save to disc (for actual stats)
        PlayerPrefsX.SaveFloatListToArray(GameWonList, "GameWonList");
        PlayerPrefsX.SaveFloatListToArray(ChipsWonList, "ChipsWonList");
        PlayerPrefsX.SaveFloatListToArray(BallsPottedList, "BallsPottedList");
        PlayerPrefsX.SaveFloatListToArray(SnookeredSelfList, "SnookeredSelfList");
        PlayerPrefsX.SaveFloatListToArray(MissShotsList, "MissShotsList");
        PlayerPrefsX.SaveFloatListToArray(ScratchList, "ScratchList");

        // make sure we are in a smart bot game
        if (SmartBotInActionGame)
        {
            // add smart bot related stuffz
            SmartBotGameLoseList.Add(inWin ? 1.0f : 0.0f);
            SmartBotLongShotTakenList.Add(Smart_Bot_Single_Long_Shot_Taken);
            SmartBotLongShotMadeList.Add(Smart_Bot_Single_Long_Shot_Made);

            // remove excesses
            while (SmartBotGameLoseList.Count > SmartBotArrayLength)
            {
                SmartBotGameLoseList.RemoveAt(0);
                SmartBotLongShotTakenList.RemoveAt(0);
                SmartBotLongShotMadeList.RemoveAt(0);
            }

            // save to disc
            PlayerPrefsX.SaveFloatListToArray(SmartBotGameLoseList, "SmartBotGameLoseList");
            PlayerPrefsX.SaveFloatListToArray(SmartBotLongShotTakenList, "SmartBotLongShotTakenList");
            PlayerPrefsX.SaveFloatListToArray(SmartBotLongShotMadeList, "SmartBotLongShotMadeList");
        }
    }

    public void addStatsToAllTimeTally(bool inWin)
    {
        Total_Games_Played += 1.0f; // no matta waaah
        Total_Games_Won += inWin ? 1.0f : 0.0f;
        Total_Chips_Lost += inWin ? 0.0f : CurrentWager;
        Total_Chips_Won += inWin ? CurrentWager : 0.0f;
        Total_Balls_Potted += P_One_Balls_Potted;
        Total_Snookered_Self += P_One_Snookered_Self;
        Total_Miss_Shots += P_One_Miss_Shots;
        Total_Scratch += P_One_Scratch;

        Current_Win_Streak = inWin ? (Current_Win_Streak + 1.0f) : 0.0f;
        Current_Lose_Streak = inWin ? 0.0f : (Current_Lose_Streak + 1.0f);

        PlayerPrefs.SetFloat("Total_Games_Played", Total_Games_Played);
        PlayerPrefs.SetFloat("Total_Games_Won", Total_Games_Won);
        PlayerPrefs.SetFloat("Total_Chips_Lost", Total_Chips_Lost);
        PlayerPrefs.SetFloat("Total_Chips_Won", Total_Chips_Won);
        PlayerPrefs.SetFloat("Total_Balls_Potted", Total_Balls_Potted);
        PlayerPrefs.SetFloat("Total_Snookered_Self", Total_Snookered_Self);
        PlayerPrefs.SetFloat("Total_Miss_Shots", Total_Miss_Shots);
        PlayerPrefs.SetFloat("Total_Scratch", Total_Scratch);

        PlayerPrefs.SetFloat("Current_Win_Streak", Current_Win_Streak);
        PlayerPrefs.SetFloat("Current_Lose_Streak", Current_Lose_Streak);
    }

    public float SignedNumberCounter(List<float> inList, bool inSignPositive = true)
    {
        float sum = 0.0f;

        for (int i = 0; i < inList.Count; i++)
        {
            if (inSignPositive && inList[i] > 0)
            {
                sum += inList[i];
            }
            else if (!inSignPositive && inList[i] < 0)
            {
                sum += inList[i];
            }
        }

        return sum;
    }

    public float GetTPAScore(float inBallsPotted, float inShotsMissed, float inSnookerSelf, float inScratch)
    {
        if (inBallsPotted + inShotsMissed + inSnookerSelf + inScratch == 0.0f)
        {
            return 0.0f;
        }
        else
        {
            return 100.0f * (inBallsPotted) / (inBallsPotted + inShotsMissed + inSnookerSelf + inScratch);
        }
    }

    public float GetMaxTPAScore()
    {
        float recentTPA = GetTPAScore(SignedNumberCounter(BallsPottedList), SignedNumberCounter(MissShotsList), SignedNumberCounter(SnookeredSelfList), SignedNumberCounter(ScratchList));
        float allTimeTPA = GetTPAScore(Total_Balls_Potted, Total_Miss_Shots, Total_Snookered_Self, Total_Scratch);

        return recentTPA > allTimeTPA ? recentTPA : allTimeTPA;
    }

    public float GetWinRatio()
    {
        if (Total_Games_Played == 0.0f)
        {
            return 0.0f;
        }
        else
        {
            return Total_Games_Won / Total_Games_Played * 100.0f;
        }
    }

    public void resetSingleGameStats()
    {
        P_One_Balls_Potted = 0.0f;
        P_One_Snookered_Self = 0.0f;
        P_One_Miss_Shots = 0.0f;
        P_One_Scratch = 0.0f;

        Smart_Bot_Single_Long_Shot_Taken = 0.0f;
        Smart_Bot_Single_Long_Shot_Made = 0.0f;

        P_Two_Balls_Potted = 0.0f;
        P_Two_Snookered_Self = 0.0f;
        P_Two_Miss_Shots = 0.0f;
        P_Two_Scratch = 0.0f;
    }

    public IEnumerator PlayLookingForPlayerMusic()
    {
        yield return new WaitForSeconds(0.25f);

        int l_f_index = (int)MusicClip.Finding;
        float l_f_volume = 1.0f;

        LookForPeopleMusic = GameManager_script.Instance().PlaySound(l_f_index, true, l_f_volume);
    }

    public IEnumerator KillLookingForPlayerMusicAndPlayFoundOppoDing(bool inPlayDing = true)
    {
        if (inPlayDing)
        {
            float f_o_volume = 1.0f;
            int f_o_index = (int)MusicClip.Found_Match;

            GameManager_script.Instance().PlaySound(f_o_index, false, f_o_volume);
        }

        if (LookForPeopleMusic == null)
        {
            yield return new WaitForSeconds(0.25f);
        }

        if (LookForPeopleMusic != null)
        {
            Destroy(LookForPeopleMusic); // stop the sound

            LookForPeopleMusic = null;
        }

        yield return new WaitForSeconds(0.0f);
    }

    public void ChangeMenuMoney()
    {
        if (uiCenter != null)
        {
            uiCenter.ChangeMoney(GameManager_script.Instance().CoinCount);
        }
    }

    public float DetermineGoodSliderPosition()
    {
        int i = 0;

        for (i = 0; i < WagerLevels.Length; i++)
        {
            if (CoinCount <= WagerLevels[i])
            {
                break;
            }
        }

        return Mathf.Clamp01(-0.050f + (0.100f * i));
    }

    public void StopClockMusic()
    {
        if (clockMusic)
        {
            Destroy(clockMusic); // stop the sound

            clockMusic = null;
        }
    }

    // dajiang hack, this is the second way to change the update money, hopefully it works well with the normal hiarchecal way
    public void UpdateWindowMoney(float oldMoney)
    {
        if (gameRoot && gameRoot.gameObject)
        {
            gameRoot.gameObject.BroadcastMessage("AnimateUpdateMoneyBroadcast", oldMoney);
        }
    }

    // this is kinda useful
    public static bool DetermineLotteryResult(float inPercentage)
    {
        return Random.Range(0.0f, 1.0f) < Mathf.Clamp01(inPercentage) ? true : false;
    }

    // this will be kinda useful
    public static void KillRoomAndDisconnect()
    {
        // leave the room, destroy it when necessary
        if (PhotonNetwork.connected && PhotonNetwork.inRoom)
        {
            PhotonNetwork.room.open = false;
            PhotonNetwork.room.visible = false;
            PhotonNetwork.room.maxPlayers = 0; // everything will be kiled

            PhotonNetwork.LeaveRoom();
        }
    }

    // well... whatever
    public static void DestroyServerController()
    {
        if (ServerController.serverController)
        {
            GameObject.Destroy(ServerController.serverController.gameObject);
            PhotonNetwork.UnAllocateViewID(ServerController.serverController.myPhotonView.viewID);
            ServerController.serverController.myPhotonView = null;
            ServerController.serverController = null;
        }
    }

    // if smart bot related stuffz are empty, we should change up
    public void SmartBotArrayInitCheckUp()
    {
        if (SmartBotGameLoseList.Count == 0 || SmartBotLongShotTakenList.Count == 0 || SmartBotLongShotMadeList.Count == 0)
        {
            for (int i = 0; i < SmartBotArrayLength; i++)
            {
                // we want to achieve a 50% winning rate
                SmartBotGameLoseList.Add(i % 2);

                // we are assuming we made the shot half the time (1 shot each game)
                SmartBotLongShotMadeList.Add(i % 2);

                // we are assuming we took 1 shot in each game
                SmartBotLongShotTakenList.Add(1);
            }
        }
    }

    // FTUE related. this is calculated and updated on creation of the game (level 1) and pending ready status for second tier ftue (level 2)
    public void CalculateFTUEBasedOnExistingValues()
    {
        // get tpa score first
        float tpa = GetMaxTPAScore();

        // we want to get these numbers first, before calculating them again
        SeenSwipeAndPullEver = PlayerPrefs.GetFloat("SeenSwipeAndPullEver");
        SeenNetworkGameFlagEver = PlayerPrefs.GetFloat("SeenNetworkGameFlagEver");

        // as long as the player hasn't seen the ftue, we want to show it to them coz it gives chips and everyone loves chips
        if (SeenSwipeAndPullEver == 0.0f && SetGameCoinEver == 1.0f)
        {
            SeenSwipeAndPullEver = 1.0f;
            PlayerPrefs.SetFloat("SeenSwipeAndPullEver", SeenSwipeAndPullEver);
        }

        // if player has played a single network game, we set it to true automatically
        if (SeenNetworkGameFlagEver == 0.0f && Total_Games_Played > 0.0f)
        {
            SeenNetworkGameFlagEver = 1.0f;
            PlayerPrefs.SetFloat("SeenNetworkGameFlagEver", SeenNetworkGameFlagEver);
        }
    }

    public int StarTextFineTuneWidth(float inWidth, float inLevel)
    {
        if (inLevel > 99.0f && inLevel <= 999.0f) // dajiang hack, right now we only have level 100, if we do more levels, we need to modify this as well
        {
            return (int)(inWidth / 1.55f);
        }
        else
        {
            return (int)(inWidth / 1.9f);
        }
    }

    public int StarTextFineTuneHeight(float inHeight, float inLevel)
    {
        return (int)(inHeight / 1.9f);
    }

    public Vector3 StarTextFineTunePosition(Vector3 inPosition, float inWidth, float inHeight, float inLevel)
    {
        return new Vector3(inPosition.x, inPosition.y - inHeight * 0.2f, inPosition.z);
    }

    public string ShowLoadGameScreen(bool inNetworkGame)
    {
        string returnString = "";

        if (inNetworkGame)
        {
            if (NeverLoadedGameBefore == 0.0f)
            {
                returnString = Localization.Get("LoadContentFirstNetwork");
            }
            else
            {
                returnString = Localization.Get("LoadContentNetwork");
            }
        }
        else
        {
            if (NeverLoadedGameBefore == 0.0f)
            {
                returnString = Localization.Get("LoadContentFirst");
            }
            else
            {
                returnString = Localization.Get("LoadContent");
            }
        }

        // make sure we are set
        NeverLoadedGameBefore = 1.0f;
        PlayerPrefs.SetFloat("NeverLoadedGameBefore", NeverLoadedGameBefore);

        return returnString;
    }

    public string CreateRandomUniqueKey()
    {
        string outString = "";

        for (int i = 0; i < 16; i++)
        {
            outString += ("" + (int)Random.Range(0, 10));
        }

        return outString;
    }

    public void CleanUpRematchConsts()
    {
        rematchCurrentMatchIsRematch = false; // this one tells you if the current match is a rematch
        rematchSmartBotLevelAdded = false; // this one tells you if smart bot has been awarded a level in a series
        rematchYouAreThePrevBreaker = false; // this one tells you if you were the break shooter in the previous game
        rematchYouAreThePrevWinner = false; // this one tells you if you were the winner in the previous game
        rematchSmartBotSeries = false; // this one is set true if its a smart bot game
        rematchSelfWantToRematch = false; // this one is set true if you click on rematch button
        rematchOppoWantToRematch = false; // this one is set true if you got rematch signal from opponent
        rematchPasscodeKey = ""; // this is the passcode key for rematch games
        rematchYourWinCount = 0;
        rematchOppoWinCount = 0;
        rematchPrevTPAScore = 0.0f;
        rematchSmartBotWantToRematch = false;
        rematchSmartBotClickPhase = 0;
    }

    // if smart bot finishes the game, it will see if it itself wants to rematch with the player
    public void GenerateSmartBotIntentToRematch(int inShotCount, bool inBotIsWinner, int inBotTotalWinCount, int inPlayerTotalWinCount)
    {
        if (inShotCount < 7 && !inBotIsWinner)
        {
            // we definitely want to rematch coz you cheated! (watch out for player quitting the game)
            rematchSmartBotWantToRematch = (DetermineLotteryResult(0.80f)) ? true : false;
        }
        else if (inBotTotalWinCount == inPlayerTotalWinCount && inBotTotalWinCount > 0)
        {
            // really want to see who is the winner
            rematchSmartBotWantToRematch = (DetermineLotteryResult(0.70f)) ? true : false;
        }
        else if (inBotTotalWinCount + inPlayerTotalWinCount >= 2)
        {
            // its sort of like a series now
            rematchSmartBotWantToRematch = (DetermineLotteryResult(0.50f)) ? true : false;
        }
        else
        {
            // normal random games
            rematchSmartBotWantToRematch = (DetermineLotteryResult(0.20f)) ? true : false;
        }
    }

    // if smart bot is not intending to rematching with player but was specifically asked to do so, it will think about it one more time
    public void GenerateSmartBotIntentToRespondToRematch()
    {
        rematchSmartBotWantToRematch = (DetermineLotteryResult(0.50f)) ? true : rematchSmartBotWantToRematch;
    }

    // fake a click
    public IEnumerator SmartBotRematchFakeButtonClick(float inSecond, int inPhase)
    {
        yield return new WaitForSeconds(inSecond);

        if (rematchSmartBotClickPhase == inPhase)
        {
            if (PhotonNetwork.connected && rematchSmartBotWantToRematch && GameManager_script.Instance().CoinCount >= GameManager_script.Instance().CurrentWager)
            {
                // click on yes we want to rematch
                GameManager_script.Instance().rematchOppoWantToRematch = true;
                GameManager_script.Instance().rematchSmartBotSeries = GameManager_script.Instance().SmartBotInActionGame;

                if (GameManager_script.Instance().rematchSelfWantToRematch)
                {
                    // it is time to hide the room for all other players forever
                    GameManager_script.KillRoomAndDisconnect();

                    // for photon view bug?
                    GameManager_script.DestroyServerController();

                    GameManager_script.Instance().rematchCurrentMatchIsRematch = true;
                    GameManager_script.Instance().rematchSelfWantToRematch = false;
                    GameManager_script.Instance().rematchOppoWantToRematch = false;

                    GameManager_script.Instance().StartingOutAsANetWorkGame = true;
                    GameManager_script.Instance().CurrentlyInANetWorkGame = true;
                    GameManager_script.Instance().SmartBotInActionGame = false;
                    GameManager_script.Instance().StupidBotInActionGame = false;
                    GameManager_script.Instance().TrulySelfInActionGame = false;
                    GameManager_script.Instance().FTUEInActionGame = false;

                    GameManager_script.Instance().TableTextureIndex = (int)GameManager_script.Instance().CurrentWagerLevel;

                    GameManager_script.Instance().PopulateSelfGameProfileInfo();

                    GameManager_script.Instance().PopulateInterstitialStartScreen
                    (
                        true,
                        GameManager_script.Instance().GetMaxTPAScore(),
                        GameManager_script.Instance().Total_Games_Played,
                        GameManager_script.Instance().Total_Games_Won,
                        GameManager_script.Instance().CoinCount - GameManager_script.Instance().CurrentWager,
                        GameManager_script.Instance().Current_Win_Streak
                    );

                    GameManager_script.Instance().resetSingleGameStats();

                    GameManager_script.Instance().StartCoroutine(GameManager_script.Instance().PlayLookingForPlayerMusic());

                    GameManager_script.Instance().PopupCurrentlyVisible = false;

                    GameManager_script.Instance().NetworkGameSceneCurrentLoad = true;

                    Application.LoadLevel("GameStart");
                }
                else
                {
                    // display to self that opponent wants to rematch!
                    GameManager_script.Instance().ChangeRematchToolTipAbsolutePath(true, "RematchYes");
                }
            }
            else
            {
                // click on no we don't want to rematch
                GameManager_script.Instance().rematchOppoWantToRematch = false;

                // show self that the other person don't want to rematch (only when we are waiting)
                GameManager_script.Instance().ChangeRematchToolTipAbsolutePath(true, "RematchNo");
            }
        }
    }

    // change tooltipz
    public void ChangeRematchToolTipAbsolutePath(bool inShow, string inText)
    {
        if (GameObject.Find("Gameroot/UICamera/Gameoverpanel/GameOverGroup/Center/UIinterstitialWindowEnd")) // dajiang hack, absolute path here
        {
            GameObject.Find("Gameroot/UICamera/Gameoverpanel/GameOverGroup/Center/UIinterstitialWindowEnd").GetComponent<UIinterstitialWindowEnd>().ToggleRematchToolTip(inShow, inText);
        }
    }

    // change selector ftue shiite
    public void ChangeFrdsSelectorFtueAbsolutePath()
    {
        if (GameObject.Find("Gameroot/UICamera/PopupWindowPanel(Clone)/Center/Background/SelectorPanel(Clone)")) // dajiang hack, absolute path here
        {
            GameObject.Find("Gameroot/UICamera/PopupWindowPanel(Clone)/Center/Background/SelectorPanel(Clone)").GetComponent<UISelectorPanel>().ShowOrHideFtue(false);
        }
    }

    // language shiite
    public string ReturnLanguageStringInEnglish()
    {
        switch (Savelanguage)
        {
            case "English": return "English";
            case "Español": return "Spanish";
            case "Deutsch": return "German";
            case "Français": return "French";
            case "简体中文": return "Chinese";
            case "繁體中文": return "Taiwan";
            case "日本語": return "Japanese";
        }

        return "English";
    }
}
