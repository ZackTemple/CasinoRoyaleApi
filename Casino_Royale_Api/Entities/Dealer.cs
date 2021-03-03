using System.Collections.Generic;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Entities
{
    public class Dealer : CardHolder
    {
        public string Name { get; set; }

        public Dealer()
        {
            Name = "Daryl";
            Cards = new List<Card>();
            Score = 0;
        }
    }
}