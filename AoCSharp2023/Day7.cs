namespace AoCSharp2023
{
    internal class Day7
    {
        public Day7()
        {
            string[] lines = File.ReadAllLines("./inputs/7.txt");
            Console.WriteLine(Part1(lines));
            //Console.WriteLine(Part2(lines));
        }

        long Part1(string[] lines)
        {
            var handsAndBids = lines.Select(l => ParseLine(l)).ToList();

            var orderedHandsAndBids = handsAndBids.OrderByDescending(l => l.Hand).ToList();
            long total = 0;
            for(int i = 0; i < orderedHandsAndBids.Count; i++)
            {
                total += orderedHandsAndBids[i].Bid * (i + 1);
            }
            return total;
        }

        (Hand Hand, int Bid) ParseLine(string line)
        {
            var split = line.Split(' ');
            var hand = new Hand(split[0]);
            var bid = int.Parse(split[1]);
            return (hand, bid);
        }
    }

    internal class Hand : IComparable<Hand>
    {
        public Hand(string hand)
        {
            if (hand.Length != 5) throw new ArgumentOutOfRangeException("Only accept hands of 5 cards");

            Cards = hand.Select(c => CharToCard(c)).ToArray();
        }
        Card[] Cards; 
        /// A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2
        HandType Type
        {
            get
            {
                var buckets = Cards.GroupBy(c => c);
                if (buckets.Count() == 1) return HandType.FiveOfAKind;
                if (buckets.Count() == 5) return HandType.HighCard;
                if (buckets.Count() == 4) return HandType.OnePair;
                if (buckets.Count() == 3)
                {
                    return buckets.Any(b => b.Count() == 3) ? HandType.ThreeOfAKind : HandType.TwoPair;
                }
                if (buckets.Count() == 2)
                {
                    return buckets.Any(b => b.Count() == 3) ? HandType.FullHouse : HandType.FourOfAKind;
                }
                throw new Exception("Should not reach this for a 5-card hand unless I'm making a mistake..");
            }
        }
        /// Five of a kind, where all five cards have the same label: AAAAA
        /// Four of a kind, where four cards have the same label and one card has a different label: AA8AA
        /// Full house, where three cards have the same label, and the remaining two cards share a different label: 23332
        /// Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
        /// Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
        /// One pair, where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
        /// High card, where all cards' labels are distinct: 23456
        /// 


        internal enum HandType
        {
            FiveOfAKind,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard,
        }

        internal enum Card
        {
            Ace,
            King,
            Queen,
            Jack,
            Ten,
            Nine,
            Eight,
            Seven,
            Six,
            Five,
            Four,
            Three,
            Two,
        }

        public int CompareTo(Hand other)
        {
            var typeComparison = Type.CompareTo(other.Type);
            if (typeComparison == 0)
            {
                for(int i = 0; i < 5; i++)
                {
                    var cardComparison = Cards[i].CompareTo(other.Cards[i]);
                    if(cardComparison != 0) return cardComparison;
                }
                return 0;
            }
            else return typeComparison;
        }

        internal Card CharToCard(char c)
        {
            return c switch
            {
                'A' => Card.Ace,
                'K' => Card.King,
                'Q' => Card.Queen,
                'J' => Card.Jack,
                'T' => Card.Ten,
                '9' => Card.Nine,
                '8' => Card.Eight,
                '7' => Card.Seven,
                '6' => Card.Six,
                '5' => Card.Five,
                '4' => Card.Four,
                '3' => Card.Three,
                '2' => Card.Two,
                _ => throw new NotImplementedException()
            };
        }
    }
}
