namespace Blackjack;
/*
    Blackjack in c# :)
    Pravidla jsem čerpal z http://www.reklamnikarty.cz/pravidla_blackjack.htm
*/
class Program
{
    public static readonly List<Card> CardsPack = new();

    public static void Main(string[] args)
    {
        //jinak se nezobrazujou typy karet v systémové konzoli
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // loopování hry
        while (true)
        {
            // hlavní menu
            Console.Clear();
            Console.WriteLine("\n=== Blackjack ===");
            if (CreateMenu("Hrát", "Ukončit") != 1) break;
            Console.Clear();

            // výběr počtu hráčů
            Console.WriteLine("\nKolik lidí bude hrát? (1-4)");
            short playerNum;
            while (true)
            {
                if (short.TryParse(Console.ReadLine(), out playerNum))
                {
                    if (playerNum < 1 || playerNum > 4) Console.WriteLine("Neplatný počet hráčů!");
                    else break;
                }
            }

            // inicializace herních prvků
            CreatePack();
            bool HasBlackjack = false;

            // dictionary hráč id, hodnota karet
            Dictionary<int, int> CardsValuePlayers = new();
            //  LOOP VŠECH HRÁČŮ
            for (int i = 0; i < playerNum; i++)
            {
                Dictionary<Card, short> CardsPlayer = new();
                int cardsValue = 0;
                for (int j = 0; j < 2; j++)
                {
                    var drawedCard = DrawCard(cardsValue, false, true);
                    CardsPlayer.Add(drawedCard.Key, drawedCard.Value);
                    cardsValue += (drawedCard.Value == 0) ? (int)drawedCard.Key.Value : drawedCard.Value;
                }
                if (cardsValue == 21) HasBlackjack = true;

                //  HRÁČ LOOP
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"--\nHráč{i+1} je na tahu!\n--\n\nTvé karty:\n-");
                    foreach (var card in CardsPlayer)
                    {
                        Console.WriteLine($"{((card.Key.Value.ToString().Length > 1) ? (int)card.Key.Value : card.Key.Value)}{card.Key.GetSymbolToString()}");
                    }
                    Console.WriteLine("--\nHodnota karet: "+cardsValue+"\n--");

                    //  EVENTY
                    // pokud má hráč blackjack
                    if (HasBlackjack)
                    {
                        Console.WriteLine("\n--\nMÁŠ BLACKJACK!\n--");
                        Console.WriteLine("\nStiskni enter pro pokračování...");
                        Console.ReadLine();
                        CardsValuePlayers.Add(i+1, 21);
                        HasBlackjack = false;
                        break;
                    }
                    //pokud jeho karty překročí limit
                    if (cardsValue > 21)
                    {
                        Console.WriteLine("\n--\nPROHRÁL JSI!\nHodnota tvých karet překročila 21!\n--");
                        Console.WriteLine("\nStiskni enter pro pokračování...");
                        Console.ReadLine();
                        CardsValuePlayers.Add(i+1, 0);
                        break;
                    }
                    //asi si nevezme další kartu pokud už má nejvyšší možnou hodnotu
                    else if (cardsValue == 21)
                    {
                        Console.WriteLine($"\n--\nTvůj tah byl ukončen!\n--");
                        Console.WriteLine("\nStiskni enter pro pokračování...");
                        Console.ReadLine();
                        CardsValuePlayers.Add(i+1, 21);
                        break;
                    }

                    //  AKCE hráče
                    Console.WriteLine();
                    if (CreateMenu("Vzít si kartu", "Ukončit tah") != 1)
                    {
                        Console.WriteLine($"\n--\nUkončil jsi svůj tah!\n--");
                        Console.WriteLine("\nStiskni enter pro pokračování...");
                        Console.ReadLine();
                        CardsValuePlayers.Add(i+1, cardsValue);
                        break;
                    }

                    // pokud neukončí tah, vezme si další kartu
                    var drawed_card = DrawCard(cardsValue);
                    CardsPlayer.Add(drawed_card.Key, drawed_card.Value);
                    cardsValue += (drawed_card.Value == 0) ? (int)drawed_card.Key.Value : drawed_card.Value;
                }
            }
            //  TAH DEALERA
            int cardsValueDealer = 0;
            Dictionary<Card, short> cardsDealer = new();
            for (int j = 0; j < 2; j++)
            {
                var drawed_card = DrawCard(cardsValueDealer, true);
                cardsDealer.Add(drawed_card.Key, drawed_card.Value);
                cardsValueDealer += (drawed_card.Value == 0) ? (int)drawed_card.Key.Value : drawed_card.Value;
            }
            if (cardsValueDealer == 21) HasBlackjack = true;
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"--\nDealer je na tahu!\n--\n\nJeho karty:\n-");
                foreach (var card in cardsDealer)
                {
                    Console.WriteLine($"{((card.Key.Value.ToString().Length > 1) ? (int)card.Key.Value : card.Key.Value)}{card.Key.GetSymbolToString()}");
                }
                Console.WriteLine("--\nHodnota karet: "+cardsValueDealer+"\n--");

                if (HasBlackjack)
                {
                    Console.WriteLine("\n--\nDEALER MÁ BLACKJACK!\n--");
                    break;
                }

                if (cardsValueDealer > 21)
                {
                    Console.WriteLine("\n--\nDEALER PROHRÁL!\nHodnota jeho karet překročila 21!\n--");
                    cardsValueDealer = 0;
                    break;
                }
                Thread.Sleep(1500);

                //real rules
                if (cardsValueDealer > 16)
                {
                    Console.WriteLine("\n--\nDealer ukončil svůj tah!\n--");
                    break;
                }
                else if (cardsValueDealer > 12 && cardsValueDealer < 17)
                {
                    Random rand = new();
                    if (rand.Next(0, 5) == 4) //20% šance na ukončení tahu
                    {
                        Console.WriteLine("\n--\nDealer ukončil svůj tah!\n--");
                        break;
                    }
                }
                var drawed_card = DrawCard(cardsValueDealer, true);
                cardsDealer.Add(drawed_card.Key, drawed_card.Value);
                cardsValueDealer += (drawed_card.Value == 0) ? (int)drawed_card.Key.Value : drawed_card.Value;
            }
            Thread.Sleep(2000);

            //  VÝSLEDEK HRY
            int MaxScore = 0;
            List<int> MaxScoresUsers = new();
            if (CardsValuePlayers.Count > 1)
            {
                var maxScores = (from entry in CardsValuePlayers orderby entry.Value descending select entry).ToArray();
                for (int i = 1; i < maxScores.Length; i++)
                {
                    if (maxScores[i-1].Value > maxScores[i].Value)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            MaxScoresUsers.Add(maxScores[j].Key);
                        }
                        MaxScore = maxScores[i-1].Value;
                        break;
                    }
                }
            }
            else
            {
                MaxScoresUsers.Add(CardsValuePlayers.First().Key);
                MaxScore = CardsValuePlayers.First().Value;
            }

            //  VÝPIS VÝSLEDKU
            Console.Clear();
            if (MaxScore > cardsValueDealer)
            {
                if (CardsValuePlayers.Count > 1)
                {
                    Console.WriteLine($"\n-----\nHráč {string.Join(",", MaxScoresUsers)} VYHRÁL!\n-----");
                }
                else
                {
                    Console.WriteLine($"\n-----\nVYHRÁL JSI!\n-----");
                }
            }
            else if (MaxScore == cardsValueDealer)
            {
                bool MultiWinners = MaxScoresUsers.Count > 1;
                if (CardsValuePlayers.Count > 1)
                {
                    Console.WriteLine($"\n-----\nHráč{(MultiWinners ? "i" : "")} {string.Join(",", MaxScoresUsers)} REMÍZOVAL{(MultiWinners ? "I" : "")} s Dealerem!\n-----");
                }
                else
                {
                    Console.WriteLine($"\n-----\nREMÍZOVAL JSI s Dealerem!\n-----");
                }
            }
            else
            {
                Console.WriteLine($"\n-----\nDEALER VYHRÁL!\n-----");
            }

            //vypsání tabulky hráčů s hodnotama jejich karet
            CardsValuePlayers.Add(10, cardsValueDealer);
            var scores = (from entry in CardsValuePlayers orderby entry.Value descending select entry).ToArray();
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i].Key == 10) Console.Write($"Dealer - ");
                else Console.Write($"Hráč {scores[i].Key} - ");
                Console.WriteLine(scores[i].Value == 0 ? "X" : scores[i].Value);
            }
            Console.WriteLine("--\n\nStiskni enter pro návrat do menu...");
            Console.ReadLine();
        }
        Console.Clear();
    }



    //vypisuje uživatelsky přívětivé menu
    public static byte CreateMenu(params string[] options)
    {
        Console.WriteLine("--");
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine(i + 1 + " - " + options[i]);
        }
        Console.WriteLine("--");
        while (true)
        {
            Console.WriteLine("Vyber si:");
            if (byte.TryParse(Console.ReadLine(), out byte choice))
            {
                if (choice < options.Length + 1) return choice;
            }
            Console.WriteLine("Neplatná hodnota!\n");
        }
    }

    private static void CreatePack()
    {
        CardsPack.Clear();
        foreach (Card.CardSymbols symb in Enum.GetValues<Card.CardSymbols>())
        {
            foreach (Card.CardValues val in Enum.GetValues<Card.CardValues>())
            {
                CardsPack.Add(new Card(val, symb));
            }
        }
    }

    private static KeyValuePair<Card, short> DrawCard(int cardsValue, bool dealerDrawing = false, bool forceHighA = false)
    {
        Random rand = new();
        Card drawed_card = CardsPack[rand.Next(0, CardsPack.Count)];
        short value_override = 0;

        if (drawed_card.Value == Card.CardValues.A)
        {
            value_override = OnDrawA(cardsValue, dealerDrawing, forceHighA);
        }
        else if (drawed_card.Value == Card.CardValues.J ||
        drawed_card.Value == Card.CardValues.Q ||
        drawed_card.Value == Card.CardValues.K) value_override = 10;

        CardsPack.Remove(drawed_card);
        return new KeyValuePair<Card, short>(drawed_card, value_override);
    }
    private static short OnDrawA(int cardsValue, bool dealerDrawing, bool forceHighA)
    {
        if (cardsValue + 11 > 21) return 1;
        else if (dealerDrawing || forceHighA) return 11;
        short choice;
        while (true)
        {
            Console.WriteLine($"\nHodnota esa 1 nebo 11? (tvůj současný součet: {cardsValue})");
            choice = Convert.ToInt16(Console.ReadLine());
            if (choice == 1 || choice == 11) break;
            else Console.WriteLine("Neplatná hodnota!");
        }
        return choice;
    }
}

class Card
{
    public readonly CardValues Value;
    public readonly CardSymbols Symbol;
    
    public enum CardValues
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, J, Q, K, A
    }
    public enum CardSymbols
    {
        Clubs, //♣
        Diamonds, //♦
        Hearts, //♥
        Spades //♠
    }
    public string GetSymbolToString()
    {
        return Symbol switch
        {
            CardSymbols.Clubs => "♣",
            CardSymbols.Diamonds => "♦",
            CardSymbols.Hearts => "♥",
            CardSymbols.Spades => "♠",
            _ => "?",
        };
    }

    public Card(CardValues value, CardSymbols symbol)
    {
        Value = value;
        Symbol = symbol;
    }
}