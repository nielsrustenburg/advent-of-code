﻿namespace AoCSharp2023
{
    internal class Day7
    {
        public Day7()
        {
            string[] lines = File.ReadAllLines("./inputs/7.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        long Part1(string[] lines)
        {
            var handsAndBids = lines.Select(l => ParseLine(l)).ToList();

            var orderedHandsAndBids = handsAndBids.OrderByDescending(l => l.Hand).ToList();
            long total = 0;
            for (int i = 0; i < orderedHandsAndBids.Count; i++)
            {
                total += orderedHandsAndBids[i].Bid * (i + 1);
            }
            return total;
        }

        long Part2(string[] lines)
        {
            var handsAndBids = lines.Select(l => ParseLine2(l)).ToList();

            var orderedHandsAndBids = handsAndBids.OrderByDescending(l => l.Hand).ToList();
            long total = 0;
            for (int i = 0; i < orderedHandsAndBids.Count; i++)
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

        (Hand Hand, int Bid) ParseLine2(string line)
        {
            var split = line.Split(' ');
            var hand = new Hand(split[0], true);
            var bid = int.Parse(split[1]);
            return (hand, bid);
        }
    }

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

    internal class Hand : IComparable<Hand>
    {
        public Hand(string hand, bool jokerRuleEnabled = false)
        {
            if (hand.Length != 5) throw new ArgumentOutOfRangeException("Only accept hands of 5 cards");

            Cards = hand.Select(c => CharToCard(c, jokerRuleEnabled)).ToArray();
        }
        Card[] Cards;
        HandType Type
        {
            get
            {
                IEnumerable<Card> cards;
                if(!Cards.All(c => c == Card.Joker))
                {
                    var bucketsWithoutJokers = Cards.Where(c => c != Card.Joker).GroupBy(c => c);
                    bucketsWithoutJokers = bucketsWithoutJokers.OrderByDescending(b => b.Count());
                    cards = Cards.Where(c => c != Card.Joker).Concat(Cards.Where(c => c == Card.Joker).Select(c => bucketsWithoutJokers.First()?.Key ?? Card.Joker));
                } else
                {
                    cards = Cards.ToList();
                }


                var buckets = cards.GroupBy(c => c);
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
            Joker,
        }

        public int CompareTo(Hand other)
        {
            var typeComparison = Type.CompareTo(other.Type);
            if (typeComparison == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    var cardComparison = Cards[i].CompareTo(other.Cards[i]);
                    if (cardComparison != 0) return cardComparison;
                }
                return 0;
            }
            else return typeComparison;
        }

        internal Card CharToCard(char c, bool jokerRuleEnabled = false)
        {
            return c switch
            {
                'A' => Card.Ace,
                'K' => Card.King,
                'Q' => Card.Queen,
                'J' => jokerRuleEnabled ? Card.Joker : Card.Jack,
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
