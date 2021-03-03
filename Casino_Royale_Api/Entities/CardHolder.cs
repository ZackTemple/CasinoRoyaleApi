using System.Collections.Generic;
using Casino_Royale_Api.Models;

namespace Casino_Royale_Api.Entities
{
    public class CardHolder
    {
        public List<Card> Cards { get; set; }
        public int Score { get; set; }
    }
}